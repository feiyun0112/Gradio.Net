using System.Reflection;
using System.Text.Json.Nodes;
using Gradio.Net;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// Reaches the factories by reflection, so that a comparison can walk the whole surface rather than
/// the ones a test remembered to name.
/// </summary>
/// <remarks>
/// <para>
/// The factories are generated, which is exactly why the comparisons here are written
/// against <see cref="gr"/> rather than against a list: a component that arrives in the layer
/// arrives in every comparison with it, and one that disappears takes its rows with it.
/// </para>
/// <para>
/// Extension methods are left out: they mirror the Python version's methods on an object
/// (<c>demo.launch()</c>), not the Python version's top-level exports.
/// </para>
/// </remarks>
internal static class GrFactories
{
    /// <summary>Every factory in the layer.</summary>
    /// <returns>The public static methods of <see cref="gr"/> that build a block and are not extensions.</returns>
    /// <remarks>
    /// <para>
    /// "Builds a block" rather than "is a public static method": the layer has members that are not
    /// factories — <c>gr.On</c> binds, <c>gr.Skip</c> and <c>gr.Update</c> answer a handler — and a
    /// comparison about blocks that walked them would be comparing a binding to a block.
    /// </para>
    /// <para>
    /// Extension methods are left out for the other reason: they mirror the Python version's
    /// methods on an object (<c>demo.launch()</c>), not the Python version's top-level exports.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<MethodInfo> All { get; } =
    [
        .. typeof(gr)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => !method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), inherit: false))
            .Where(method => typeof(Block).IsAssignableFrom(method.ReturnType))
            .OrderBy(method => method.Name, StringComparer.Ordinal),
    ];

    /// <summary>The factory that builds a block the Python version names.</summary>
    /// <param name="block">The block's class name, as the manifest spells it.</param>
    /// <returns>The factory, or <see langword="null"/> when the layer has none.</returns>
    internal static MethodInfo? Of(string block) =>
        All.FirstOrDefault(factory => string.Equals(factory.Name, block, StringComparison.OrdinalIgnoreCase));

    /// <summary>Calls a factory without arguments, which is what "the defaults" means.</summary>
    /// <param name="factory">The factory to call.</param>
    /// <returns>The block it built.</returns>
    /// <remarks>
    /// A parameter with a default is still a parameter: reflection fills it from
    /// <see cref="Type.Missing"/> only when the call says it may, which is what
    /// <see cref="BindingFlags.OptionalParamBinding"/> is for.
    /// </remarks>
    internal static Block Invoke(MethodInfo factory) =>
        (Block)factory.Invoke(
            null,
            BindingFlags.OptionalParamBinding,
            binder: null,
            [.. factory.GetParameters().Select(_ => Type.Missing)],
            culture: null)!;

    /// <summary>Whether a block may appear more than once on a page.</summary>
    /// <param name="type">The block's type.</param>
    /// <returns><see langword="false"/> for a block the render refuses to see twice.</returns>
    /// <remarks>
    /// Asked of the type rather than listed by name: a comparison that builds every block on one page
    /// cannot hold two of these, and the next block to declare the rule has to be excluded by the
    /// same sentence rather than by someone remembering.
    /// </remarks>
    internal static bool Repeats(Type type) => !(New(type) ?? throw new InvalidOperationException($"No factory returns {type.Name}.")).SinglePerPage;

    /// <summary>Builds the block a factory returns, with nothing set.</summary>
    /// <param name="type">The type a factory returns.</param>
    /// <returns>The block, with the factory's defaults and nothing else set.</returns>
    /// <remarks>
    /// The factory is the only way to build a block, so a test that needs one of each type asks
    /// the factory with no arguments rather than reaching for the constructor — which is internal,
    /// and would be the second way the framework exists to avoid.
    /// </remarks>
    internal static Block? New(Type type)
    {
        var factory = typeof(gr)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(method => method.ReturnType == type)
            .OrderBy(method => method.GetParameters().Length)
            .FirstOrDefault();

        return factory is null ? null : Invoke(factory);
    }

    /// <summary>Renders one page and returns its config.</summary>
    /// <param name="build">Writes the page.</param>
    /// <returns>The document a browser boots from, without the application's own identity.</returns>
    /// <remarks>
    /// The page is a core application, opened by hand: the blocks the caller writes are added with
    /// <see cref="Blocks.Add{TBlock}"/>, and a factory builds without placing because there is no
    /// layer application to place into. <see cref="LayerConfigOf"/> is the other shape.
    /// </remarks>
    internal static JsonObject ConfigOf(Action<Blocks> build)
    {
        using var demo = new Blocks(new RenderContextAccessor(), new BlockContainerFactory());

        using (demo.BeginRender())
        {
            build(demo);
        }

        return WithoutIdentity(demo);
    }

    /// <summary>Renders one page written the layer's way and returns its config.</summary>
    /// <param name="build">Writes the page, with no application to pass around.</param>
    /// <returns>The document a browser boots from, without the application's own identity.</returns>
    /// <remarks>
    /// Nothing between the braces opens a render or adds a block: <c>gr.Blocks()</c> opens the
    /// application, and every factory places what it builds on it — which is the shape a ported file
    /// has, and the one this helper exists to make easy to assert against.
    /// </remarks>
    internal static JsonObject LayerConfigOf(Action build)
    {
        Blocks demo;

        // The scope ends before the config is read: leaving it is what groups what was written, so a
        // document read while the page is still being written is not the document a browser gets.
        using (demo = gr.Blocks())
        {
            build();
        }

        return WithoutIdentity(demo);
    }

    /// <summary>Renders one page written the layer's way, for a caller that needs the application.</summary>
    /// <param name="build">Writes the page, given the application for the members that are its own.</param>
    /// <returns>The document a browser boots from, without the application's own identity.</returns>
    /// <remarks>
    /// The application is passed in for the things that are not a block's to say — a binding
    /// (<c>demo.Load(...)</c>), a page (<c>demo.Route(...)</c>), a setting — not for adding: a
    /// factory places what it builds.
    /// </remarks>
    internal static JsonObject LayerConfigOf(Action<Blocks> build)
    {
        Blocks demo;

        using (demo = gr.Blocks())
        {
            build(demo);
        }

        return WithoutIdentity(demo);
    }

    /// <summary>The config, without the field that identifies the application.</summary>
    /// <param name="demo">The application.</param>
    /// <returns>The document, with <c>app_id</c> removed.</returns>
    /// <remarks>
    /// The one field left out of every comparison here: two applications are two applications, and
    /// the value is drawn per instance.
    /// </remarks>
    internal static JsonObject WithoutIdentity(Blocks demo)
    {
        var config = demo.GetConfig();
        config.Remove("app_id");

        return config;
    }

    /// <summary>The config entry of the one block a page holds.</summary>
    /// <param name="build">Writes the page.</param>
    /// <returns>The entry, in the shape two blocks can be compared in.</returns>
    internal static JsonObject EntryOf(Action<Blocks> build) =>
        Comparable(ConfigOf(build)["components"]!.AsArray()[0]!.AsObject());

    /// <summary>The config entry of the one block a page written by the layer holds.</summary>
    /// <param name="build">Writes the page.</param>
    /// <returns>The entry, in the shape two blocks can be compared in.</returns>
    internal static JsonObject EntryOf(Action build) =>
        Comparable(LayerConfigOf(build)["components"]!.AsArray()[0]!.AsObject());

    /// <summary>The config entry of the one block a factory puts on a page.</summary>
    /// <param name="factory">The factory to call, with no arguments.</param>
    /// <returns>The entry, in the shape two blocks can be compared in.</returns>
    /// <remarks>
    /// The factory is called <em>inside</em> the page, so what it builds is placed by the layer
    /// rather than added by hand: the entry is the one a ported line would produce.
    /// </remarks>
    internal static JsonObject EntryOf(MethodInfo factory) =>
        Comparable(LayerConfigOf(() => Invoke(factory))["components"]!.AsArray()[0]!.AsObject());

    /// <summary>A config entry reduced to what two blocks of the same kind must agree on.</summary>
    /// <param name="entry">The entry.</param>
    /// <returns>A copy without the two keys a render assigns per instance.</returns>
    /// <remarks>
    /// <c>id</c> is the block's place in the page, which two blocks never share, and <c>elem_id</c>
    /// is an id a component invents for itself when the application names none (a share button
    /// counts its instances) — both are facts about this page rather than about the block's
    /// parameters, and comparing them would compare two renders rather than two spellings.
    /// </remarks>
    internal static JsonObject Comparable(JsonObject entry)
    {
        var copy = entry.DeepClone().AsObject();
        copy.Remove("id");
        copy["props"]?.AsObject().Remove("elem_id");

        return copy;
    }
}
