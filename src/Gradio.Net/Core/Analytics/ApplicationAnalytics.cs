using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The five events the Python version reports, built at the moment each happens and handed to the
/// sink.
/// </summary>
/// <remarks>
/// <para>
/// This is a port of the reporting half of the Python version's <c>analytics.py</c> and nothing
/// else: every function here composes the payload the Python version composes, and then stops —
/// where the Python version starts a thread that posts it, this hands it to an
/// <see cref="IAnalyticsSink"/>, whose default does nothing. The five are a version check, the
/// application being constructed, the application being launched, the editor being turned on, and
/// an error.
/// </para>
/// <para>
/// Every function asks the application's own switch first, which is the Python version's
/// arrangement: the switch is the application's, so an application that leaves it alone reports
/// nothing even though the framework is willing to compose the payload. That is also why the events
/// are composed here rather than by the sink: what an application switched off is the reporting,
/// not the decision of what an event looks like.
/// </para>
/// <para>
/// The five do not all happen at one moment, but two of them moved. The Python version reports
/// <c>initiated</c> when a <c>Blocks</c> object finishes being constructed and <c>launched</c> when
/// <c>launch()</c> returns; here the application object is created empty by the container and
/// written afterwards, so "constructed" is not a moment anybody can name until the host is about to
/// serve it. The construction reports are therefore emitted as the host starts and before its
/// startup work, the launch report after it — the Python version's order, and the order that makes
/// a failed start say the application was built and never launched. The editor's event comes with
/// the launch one, because a host that starts with the editor on is the moment the editor was
/// started; the error event keeps its own moment, which is a run that raised.
/// </para>
/// </remarks>
internal static class ApplicationAnalytics
{
    /// <summary>The mode an interface reports, which is the one whose inputs and outputs are its own.</summary>
    private const string InterfaceMode = "interface";

    /// <summary>Reports that the application wanted to know whether a newer release exists.</summary>
    /// <param name="blocks">The application reporting.</param>
    /// <param name="sink">Where the event goes.</param>
    /// <remarks>
    /// The payload is empty because the Python version's check carries none: it is a <c>GET</c>
    /// whose answer names the newest release, and the version being checked is the library's own.
    /// Inventing a field for it here would make the event a different shape from the one a
    /// migrating implementation is comparing against, which is the one thing this port promises
    /// about it.
    /// </remarks>
    internal static void VersionCheck(Blocks blocks, IAnalyticsSink sink)
    {
        if (!blocks.AnalyticsEnabled)
        {
            return;
        }

        Report(sink, AnalyticsTopics.VersionCheck, new JsonObject());
    }

    /// <summary>Reports that the application was constructed.</summary>
    /// <param name="blocks">The application reporting.</param>
    /// <param name="sink">Where the event goes.</param>
    internal static void Initiated(Blocks blocks, IAnalyticsSink sink)
    {
        if (!blocks.AnalyticsEnabled)
        {
            return;
        }

        Report(sink, AnalyticsTopics.Initiated, new JsonObject
        {
            ["mode"] = blocks.Mode,
            ["version"] = GradioVersion.Upstream,
        });
    }

    /// <summary>Reports that the application was launched: what it is made of, and how it is served.</summary>
    /// <param name="blocks">The application reporting.</param>
    /// <param name="sink">Where the event goes.</param>
    /// <remarks>
    /// <para>
    /// The document is the Python version's two dictionaries merged in the Python version's order:
    /// the launch parameters it collected in <c>launch()</c> first, then the description of the
    /// application it walks the blocks for. The keys that name a thing this framework cannot be — a
    /// Colab session, a Space, a share tunnel, a hosted notebook — are present and false rather
    /// than absent, because a consumer that reads a key and finds nothing would treat a missing
    /// answer as an unknown one.
    /// </para>
    /// <para>
    /// The lists are built the way the Python version builds them: every rendered block by name,
    /// then the targets, events, inputs and outputs of every binding in the order the bindings were
    /// made. A binding's inputs and outputs are narrowed to the blocks that are actually rendered,
    /// which is the Python version's <c>if y in blocks.blocks</c>: a block that was reserved but
    /// never placed is not on the page, so a report that counted it would describe an application
    /// nobody is serving.
    /// </para>
    /// </remarks>
    internal static void Launched(Blocks blocks, IAnalyticsSink sink)
    {
        if (!blocks.AnalyticsEnabled)
        {
            return;
        }

        var rendered = new HashSet<int>(blocks.Registered.Select(block => block.Id));
        var targets = new JsonArray();
        var events = new JsonArray();
        var inputs = new JsonArray();
        var outputs = new JsonArray();

        foreach (var dependency in blocks.Dependencies)
        {
            foreach (var target in dependency.Targets)
            {
                if (target.Block is not { } block || !rendered.Contains(block.Id))
                {
                    continue;
                }

                targets.Add(block.BlockClass);
                events.Add(target.EventName);
            }

            foreach (var input in dependency.Inputs)
            {
                if (rendered.Contains(input.Id))
                {
                    inputs.Add(input.BlockClass);
                }
            }

            foreach (var output in dependency.Outputs)
            {
                if (rendered.Contains(output.Id))
                {
                    outputs.Add(output.BlockClass);
                }
            }
        }

        var telemetry = new JsonArray();
        var custom = new JsonArray();

        foreach (var block in blocks.Registered)
        {
            telemetry.Add(block.BlockClass);

            if (IsCustom(block))
            {
                custom.Add(block.BlockClass);
            }
        }

        Report(sink, AnalyticsTopics.Launched, new JsonObject
        {
            ["launch_method"] = "inline",
            ["is_google_colab"] = false,
            ["is_sharing_on"] = false,
            ["is_space"] = false,
            ["mode"] = blocks.Mode,
            ["custom_css"] = blocks.Css is not null,
            ["theme"] = blocks.Theme.Name,
            ["is_custom_theme"] = IsCustomTheme(blocks.Theme),
            ["custom_js"] = false,
            ["custom_head"] = false,
            ["custom_css_paths"] = blocks.CssPaths is { Count: > 0 },
            ["custom_head_paths"] = false,
            ["version"] = GradioVersion.Upstream,
            ["is_hosted_notebook"] = false,
            ["using_auth"] = blocks.Auth is not null,
            ["dev_mode"] = false,
            ["inputs"] = InterfaceComponents(blocks, inputs: true, fallback: inputs),
            ["outputs"] = InterfaceComponents(blocks, inputs: false, fallback: outputs),
            ["targets"] = targets,
            ["blocks"] = telemetry,
            ["events"] = events,
            ["using_custom_component"] = custom.Count > 0,
            ["custom_components"] = custom,
        });
    }

    /// <summary>Reports that the Vibe editor is on, for an application that turned it on.</summary>
    /// <param name="blocks">The application reporting.</param>
    /// <param name="sink">Where the event goes.</param>
    /// <remarks>
    /// <para>
    /// The Python version posts this from its command line, at the moment the editor is started;
    /// here the editor has no command of its own — it is a switch and a set of routes — so the
    /// moment is the host starting with the switch on. An application that never turns the editor
    /// on never reports it, which is the same statement either way.
    /// </para>
    /// <para>
    /// The payload is the Python version's one key. The word is the command that started the editor
    /// the Python version, kept as it is because a consumer grouping these events is grouping them
    /// by that word.
    /// </para>
    /// </remarks>
    internal static void Vibe(Blocks blocks, IAnalyticsSink sink)
    {
        if (!blocks.AnalyticsEnabled || !blocks.VibeMode)
        {
            return;
        }

        Report(sink, AnalyticsTopics.Vibe, new JsonObject { ["command"] = "vibe" });
    }

    /// <summary>Reports an error the framework caught while serving.</summary>
    /// <param name="blocks">The application reporting.</param>
    /// <param name="sink">
    /// Where the event goes, or <see langword="null"/> for a queue or a route that was built without
    /// one — which is how a test builds them, and how an application that never registered a sink
    /// looks from the inside.
    /// </param>
    /// <param name="message">What went wrong, in the words the log carries.</param>
    /// <remarks>
    /// <para>
    /// The Python version's payload, and its one key. Its own only call site is a tunnel it could
    /// not set up — a capability this framework does not have — so the moment is this framework's:
    /// a run whose handler raised, which is the error the framework actually catches and reports to
    /// the operator's log. The event says the same thing to a sink that the log says to the
    /// operator, and it is the run's own message rather than a stack trace, because a payload is a
    /// report and not a dump.
    /// </para>
    /// <para>
    /// A validator's refusal is not an error in this sense: it is the binding answering "no", which
    /// is the application working. Only a handler that raised is reported.
    /// </para>
    /// </remarks>
    internal static void Failed(Blocks blocks, IAnalyticsSink? sink, string message)
    {
        if (sink is null || !blocks.AnalyticsEnabled)
        {
            return;
        }

        Report(sink, AnalyticsTopics.Error, new JsonObject { ["error"] = message });
    }

    /// <summary>Hands one event to the sink, and lets nothing the sink does escape.</summary>
    /// <param name="sink">Where the event goes.</param>
    /// <param name="topic">Where the Python version would have posted it.</param>
    /// <param name="payload">What the Python version would have posted.</param>
    /// <remarks>
    /// Swallowed rather than reported, which is the Python version's arrangement: its own reporting
    /// runs on a thread whose exception nobody would see, and a framework that failed a request
    /// because a reporting sink was down would have made telemetry a dependency of serving.
    /// </remarks>
    private static void Report(IAnalyticsSink sink, string topic, JsonObject payload)
    {
        try
        {
            sink.Send(new AnalyticsEvent(topic, payload));
        }
        catch (Exception)
        {
            // There is nothing left to do about it, and the failure that matters is the one being
            // reported.
        }
    }

    /// <summary>Whether a block is the application's own rather than one of the framework's.</summary>
    /// <param name="block">The block to ask about.</param>
    /// <returns><see langword="true"/> for a block whose class this assembly does not define.</returns>
    /// <remarks>
    /// The Python version's test is "the class's module starts with <c>gradio.</c>", which is the
    /// same question asked of a Python package: is this one of ours. Here the answer is which
    /// assembly defined the class, because a custom component is a subclass an application wrote —
    /// and a name list would go stale the moment a component was added.
    /// </remarks>
    private static bool IsCustom(Block block) => block.GetType().Assembly != typeof(Block).Assembly;

    /// <summary>Whether a theme is one of the presets rather than one the application built.</summary>
    /// <param name="theme">The theme to ask about.</param>
    /// <returns><see langword="true"/> when no preset has the same variables.</returns>
    /// <remarks>
    /// The Python version compares the theme's whole document with each built-in theme's, so a
    /// theme that was built by hand but happens to set nothing is not custom — which is the useful
    /// answer: what a consumer wants to know is whether the application looks different from the
    /// twelve presets, not whether it was constructed by name.
    /// </remarks>
    private static bool IsCustomTheme(Theme theme)
    {
        var document = theme.ToDictionary();

        foreach (var name in ThemePresets.BuiltInNames)
        {
            if (JsonNode.DeepEquals(ThemePresets.Get(name).ToDictionary(), document))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>The components an interface was given, or the bindings' own for anything else.</summary>
    /// <param name="blocks">The application.</param>
    /// <param name="inputs">Which of the two lists to answer with.</param>
    /// <param name="fallback">What the bindings named, for an application that is not an interface.</param>
    /// <returns>The names, or <see langword="null"/> for an interface with none of that kind.</returns>
    /// <remarks>
    /// The Python version answers an interface with the components it was constructed from and
    /// everything else with the components its bindings happen to name, because an interface knows
    /// its own columns and a page of blocks does not. An interface given no components of a kind
    /// answers <see langword="null"/> rather than an empty list, which is the Python version's
    /// <c>if components else None</c>.
    /// </remarks>
    private static JsonArray? InterfaceComponents(Blocks blocks, bool inputs, JsonArray fallback)
    {
        if (!string.Equals(blocks.Mode, InterfaceMode, StringComparison.Ordinal))
        {
            return fallback;
        }

        var components = inputs ? blocks.InputComponents : blocks.OutputComponents;

        if (components is not { Count: > 0 })
        {
            return null;
        }

        var names = new JsonArray();

        foreach (var component in components)
        {
            names.Add(component.BlockClass);
        }

        return names;
    }
}
