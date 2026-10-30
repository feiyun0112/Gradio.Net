using System.Globalization;
using System.Reflection;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// What a factory's optional parameters mean when they are left out.
/// </summary>
/// <remarks>
/// <para>
/// "What happens when the argument is omitted" is the part of a port that goes wrong silently: a
/// default of 160 where the Python version has 200 compiles, runs, and draws a slightly different
/// page. Most factories are generated from the recorded Python signature, so for those the question
/// this file asks is whether the generation kept it. The layouts are written by hand — their
/// factory and the scope that enters them are one feature — and for those the question is the one a
/// hand-written signature always raises, which is why they are walked by the same loop rather than
/// trusted to review.
/// </para>
/// <para>
/// A parameter whose default C# cannot spell — a list, a visibility, a document — is declared
/// <see langword="null"/> and written only when it was given. What makes that safe is that the
/// block's own default is the Python version's: <see cref="GrFactoryTests"/> holds the two ways of
/// building a block to the same config, so a member whose default drifted fails there rather than
/// here.
/// </para>
/// </remarks>
public sealed class GrDefaultsTests
{
    [Fact]
    public void Every_parameter_carries_upstreams_default_where_csharp_can_spell_it()
    {
        var examined = 0;
        var compared = 0;
        var spelt = 0;

        foreach (var (block, parameters) in GrManifest.Blocks())
        {
            if (GrFactories.Of(block) is not { } factory)
            {
                continue;
            }

            var defaults = GrManifest.Defaults(block);
            var declared = factory.GetParameters();

            for (var index = 0; index < parameters.Count; index++)
            {
                examined++;

                var parameter = declared[index];
                var upstream = defaults.TryGetValue(parameters[index], out var recorded) ? recorded : null;

                if (upstream is null)
                {
                    // The Python version's default is "nothing", which is what a C# parameter defaults to.
                    Assert.True(
                        parameter.DefaultValue is null,
                        $"{factory.Name}({parameter.Name}) defaults to {Show(parameter.DefaultValue)} and the Python version "
                        + "defaults to nothing.");

                    continue;
                }

                if (parameter.DefaultValue is not null)
                {
                    Assert.True(
                        Same(parameter.DefaultValue, upstream),
                        $"{factory.Name}({parameter.Name}) defaults to {Show(parameter.DefaultValue)} and the Python version "
                        + $"defaults to {upstream.ToJsonString()}.");

                    spelt++;
                }

                compared++;
            }
        }

        // A comparison that compared nothing would pass whatever the factories said, and one that
        // only ever compared the nulls would not be about defaults at all.
        Assert.True(examined >= 1200, $"Only {examined} parameter(s) were examined; the surface is far larger.");
        Assert.True(compared >= 450, $"Only {compared} parameter(s) have a default the Python version; the rest cannot.");
        Assert.True(spelt >= 340, $"Only {spelt} default(s) are spelled in a signature; the rest are not defaults.");
    }

    [Fact]
    public void A_factory_with_no_arguments_builds_the_block_the_core_api_builds()
    {
        // Every component, one page, both ways: the entry a factory produces and the entry the core
        // type produces have to be the same block, down to the order of the props. One request, so
        // the comparison covers the whole surface rather than the components a test remembered.
        // Paired by the id a render assigns rather than by position: a page groups a run of form
        // components into a form, so the entries a page holds are not the blocks it was given.
        var pairs = new List<(string Name, int Factory, int Core)>();

        // Both sides are built before the page is opened, so that a factory is only ever a factory
        // here: with no page being written there is nothing to place, and the two blocks are the two
        // ways of building one thing rather than two visits to the same tree.
        var built = GrManifest.Blocks()
            // A browser state's key and secret are drawn at random per instance, which is the whole
            // of what the Python version does with them: two of them are never the same block.
            .Where(entry => entry.Block != "BrowserState")
            // A block that may appear once per page cannot be compared this way either: the page
            // holds every block twice, once from each side, and the second navbar is refused. Its
            // own page is where it is exercised.
            .Where(entry => GrFactories.Of(entry.Block) is not { } factory || GrFactories.Repeats(factory.ReturnType))
            .Select(entry => (entry.Block, Factory: GrFactories.Of(entry.Block)))
            .Where(entry => entry.Factory is not null)
            .Select(entry => (
                entry.Block,
                FromFactory: GrFactories.Invoke(entry.Factory!),
                FromCore: GrFactories.New(entry.Factory!.ReturnType)))
            .ToList();

        var config = GrFactories.ConfigOf(demo =>
        {
            foreach (var (block, fromFactory, fromCore) in built)
            {
                pairs.Add((block, demo.Add(fromFactory).Id, demo.Add(fromCore!).Id));
            }
        });

        var entries = config["components"]!.AsArray()
            .OfType<JsonObject>()
            .ToDictionary(entry => entry["id"]!.GetValue<int>());

        var mismatched = new List<string>();
        foreach (var (name, factory, core) in pairs)
        {
            var fromFactory = GrFactories.Comparable(entries[factory]);
            var fromCore = GrFactories.Comparable(entries[core]);

            if (fromFactory.ToJsonString() != fromCore.ToJsonString())
            {
                mismatched.Add($"{name}: {Difference(fromCore, fromFactory)}");
            }
        }

        Assert.True(
            mismatched.Count == 0,
            "These factories build a block the core API does not build: " + string.Join(" | ", mismatched));
    }

    [Fact]
    public void An_alias_builds_what_the_other_spelling_builds()
    {
        // The Python version exports five components twice and three of them are assignments rather than
        // classes (`Text = Textbox`, `Highlight = HighlightedText`, `Mic = Microphone`). A port
        // writes whichever spelling it wrote in Python, and the two have to be the same block.
        var aliases = new (string Alias, string Primary)[]
        {
            ("Text", "Textbox"),
            ("Json", "JSON"),
            ("DataFrame", "Dataframe"),
            ("Checkboxgroup", "CheckboxGroup"),
            ("Annotatedimage", "AnnotatedImage"),
            ("Highlightedtext", "HighlightedText"),
            ("Highlight", "HighlightedText"),
            ("Mic", "Microphone"),
        };

        foreach (var (alias, primary) in aliases)
        {
            var fromAlias = GrFactories.EntryOf(GrFactories.Of(alias)!);
            var fromPrimary = GrFactories.EntryOf(GrFactories.Of(primary)!);

            Assert.True(
                fromAlias.ToJsonString() == fromPrimary.ToJsonString(),
                $"{alias} and {primary} build different blocks: {Difference(fromPrimary, fromAlias)}");
        }
    }

    /// <summary>Whether a C# default is the recorded one, as the two shapes can be
    /// compared.</summary> <param name="spelt">The parameter's default, as the compiled signature
    /// carries it.</param> <param name="recorded">The default the Python version's signature
    /// declares.</param> <returns>Whether they are the same value.</returns>
    /// <remarks>
    /// The recorded side is JSON, so every number arrives as a <see cref="JsonValue"/> holding a
    /// double: a whole number is compared as one, and a <see cref="double"/> parameter is compared
    /// as one, because a signature may spell either.
    /// </remarks>
    private static bool Same(object spelt, JsonNode recorded)
    {
        if (recorded is not JsonValue value)
        {
            return false;
        }

        if (spelt is bool flag)
        {
            return value.TryGetValue<bool>(out var expected) && expected == flag;
        }

        if (spelt is string text)
        {
            return value.TryGetValue<string>(out var expected) && string.Equals(expected, text, StringComparison.Ordinal);
        }

        if (!value.TryGetValue<double>(out var number))
        {
            return false;
        }

        return spelt switch
        {
            int whole => whole == number,
            long wide => wide == number,
            double real => real.Equals(number),
            _ => false,
        };
    }

    private static string Show(object? value) => value switch
    {
        null => "null",
        string text => $"\"{text}\"",
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? "null",
    };

    private static string Difference(JsonObject core, JsonObject factory)
    {
        var coreProps = core["props"] as JsonObject ?? [];
        var factoryProps = factory["props"] as JsonObject ?? [];

        var differences = coreProps
            .Concat(factoryProps)
            .Select(pair => pair.Key)
            .Distinct(StringComparer.Ordinal)
            .Where(key => coreProps[key]?.ToJsonString() != factoryProps[key]?.ToJsonString())
            .Select(key => $"{key} {coreProps[key]?.ToJsonString() ?? "<absent>"} vs {factoryProps[key]?.ToJsonString() ?? "<absent>"}");

        return string.Join(", ", differences);
    }
}
