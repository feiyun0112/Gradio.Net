using System.Reflection;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// The second set comparison: every factory's parameter list against the parameters the Python
/// version declares for that block.
/// </summary>
/// <remarks>
/// <para>
/// The Python side is the manifest the build already reads for the generated parameter mapping
/// (<c>src/upstream/&lt;version&gt;/component-params.json</c>), so a parameter the Python
/// version adds is a parameter this comparison starts demanding without anyone remembering it.
/// </para>
/// <para>
/// The comparison is over the sequence, not the set. A named argument does not care about order,
/// but the port does: a reader going down a Python call and a C# call side by side is reading two
/// lists, and a list in a different order is a list that has to be read twice.
///
/// The spelling is the one thing that differs: the Python version writes `elem_id` and the factory
/// writes `elemId`, so each name on the Python side is read as the C# spelling of itself before the
/// two lists are compared.
/// </para>
/// </remarks>
public sealed class GrParametersTests
{
    [Fact]
    public void Every_factory_takes_upstreams_parameters_in_upstreams_order()
    {
        var compared = 0;
        var layouts = 0;

        foreach (var (block, parameters) in UpstreamBlocks())
        {
            if (FactoryOf(block) is not { } factory)
            {
                continue;
            }

            var names = factory.GetParameters().Select(parameter => parameter.Name!).ToArray();
            var expected = parameters.Select(CamelCase).ToArray();

            Assert.True(
                expected.SequenceEqual(names, StringComparer.Ordinal),
                $"{factory.Name} takes ({string.Join(", ", names)}) and the Python version's {block} takes "
                + $"({string.Join(", ", parameters)}), which in C# is ({string.Join(", ", expected)}). "
                + "The names, their order and their number are the Python version's.");

            compared++;

            if (typeof(BlockContext).IsAssignableFrom(factory.ReturnType))
            {
                layouts++;
            }
        }

        // A comparison that compared nothing would pass whatever the factories said.
        Assert.True(compared >= 2, $"Only {compared} factory(ies) were compared; the vertical slice is two blocks.");

        // The layouts are hand-written, so the one thing that keeps their signatures
        // honest is that this comparison reaches them: nine of the Python version's layouts, all of them.
        Assert.True(layouts >= 9, $"Only {layouts} layout(s) were compared; the Python version exports nine.");
    }

    [Fact]
    public void An_alias_takes_what_the_spelling_it_stands_for_takes()
    {
        // The Python version exports eight blocks twice, and a port writes whichever spelling it wrote in
        // Python. The block-driven comparison above reaches the first spelling of each and never the
        // second, so the aliases are compared against their primaries here: a forwarder that dropped
        // or reordered a parameter would otherwise be a signature nothing looks at.
        var compared = 0;

        foreach (var (alias, primary) in Aliases())
        {
            var aliased = GrFactories.Of(alias) ?? throw new InvalidDataException($"gr.{alias} does not exist");
            var original = GrFactories.Of(primary) ?? throw new InvalidDataException($"gr.{primary} does not exist");

            var aliasedNames = aliased.GetParameters().Select(parameter => parameter.Name!).ToArray();
            var originalNames = original.GetParameters().Select(parameter => parameter.Name!).ToArray();

            Assert.True(
                originalNames.SequenceEqual(aliasedNames, StringComparer.Ordinal),
                $"{alias} takes ({string.Join(", ", aliasedNames)}) and {primary} takes "
                + $"({string.Join(", ", originalNames)}). An alias is the same block, so it is the same signature.");

            compared++;
        }

        Assert.True(compared >= 8, $"Only {compared} alias(es) were compared; the Python version exports eight spellings twice.");
    }

    [Fact]
    public void A_factory_returns_the_block_upstream_names()
    {
        // The factory's return type is the core type, and it is the one the Python version's class of that name
        // is: a factory returning a wrapper, or the wrong block, would make the config comparison
        // pass while the page was wrong.
        //
        // Compared without regard to case because one of the Python version's class names is an acronym this
        // build spells the C# way: `HTML` is `Html`. Every other component's name is the same
        // either way, so the comparison still says what it was written to say.
        //
        // A layout is a block but not a component, and one of them is named after its layout rather
        // than after the Python version's class — `Step` is `WalkthroughStep` — so the name compared for a
        // layout is the one the block reports as its class, which is the Python version's.
        foreach (var (block, _) in UpstreamBlocks())
        {
            if (FactoryOf(block) is not { } factory)
            {
                continue;
            }

            if (typeof(Component).IsAssignableFrom(factory.ReturnType))
            {
                Assert.Equal(block, factory.ReturnType.Name, ignoreCase: true);

                continue;
            }

            Assert.True(
                typeof(BlockContext).IsAssignableFrom(factory.ReturnType),
                $"{factory.Name} returns {factory.ReturnType.Name}, which is neither a component nor a layout.");

            var layout = GrFactories.New(factory.ReturnType)!;

            Assert.Equal(block, layout.BlockClass, ignoreCase: true);
        }
    }

    [Fact]
    public void Every_factory_is_a_block_upstream_declares()
    {
        // `Blocks` is the exception and not an oversight: the Python version's `Blocks` is a block context
        // rather than a component, so the component manifest does not list it.
        //
        // Asked of the block the factory builds rather than of the factory's name, because the Python version
        // exports some of its components twice — `JSON` and `Json`, `Textbox` and `Text` — and the
        // second spelling is not a block name; and because one block is named after the layout it
        // belongs to rather than after the Python version's class — `Step` builds a `WalkthroughStep`, whose
        // class is `step`. What has to be a declared block is the thing built.
        var declared = UpstreamBlocks()
            .Select(entry => entry.Block.ToLowerInvariant())
            .ToHashSet(StringComparer.Ordinal);

        var unknown = Factories()
            .Where(factory => factory.ReturnType.Name != nameof(Blocks))
            .Select(factory => (factory.Name, Block: BlockClassOf(factory)))
            .Where(entry => !declared.Contains(entry.Block))
            .Select(entry => $"{entry.Name} -> {entry.Block}")
            .ToList();

        Assert.True(
            unknown.Count == 0,
            $"These factories build a block the Python version's component manifest does not declare: {string.Join(", ", unknown)}.");
    }

    [Fact]
    public void Every_block_upstream_exports_has_a_factory_or_a_row_in_the_ledger()
    {
        // The two comparisons have to agree about what is missing: this one walks the blocks the Python version
        // declares, the surface one walks the names it exports, and a block in neither table would be
        // a block nobody is going to build.
        //
        // Only exported blocks are counted: the manifest lists every block, including the ones
        // the Python version keeps to itself — the API component, the simple variants, the base classes — and
        // those are not names a ported file can write.
        var exported = GrSurface.UpstreamExports();
        var owed = GrSurfaceOwed.Components
            .Concat(GrSurfaceOwed.Layouts)
            .Concat(GrSurfaceOwed.TopLevel)
            .ToHashSet(StringComparer.Ordinal);

        var unowed = UpstreamBlocks()
            .Select(entry => entry.Block)
            .Where(exported.Contains)
            .Where(block => FactoryOf(block) is null && !owed.Contains(block))
            .ToList();

        Assert.True(
            unowed.Count == 0,
            $"These blocks have no factory and no row in the ledger: {string.Join(", ", unowed)}.");
    }

    private static IEnumerable<MethodInfo> Factories() => GrFactories.All;

    private static MethodInfo? FactoryOf(string block) =>
        Factories().FirstOrDefault(factory => string.Equals(factory.Name, block, StringComparison.Ordinal));

    /// <summary>The block a factory builds, as the config names it.</summary>
    /// <param name="factory">The factory.</param>
    /// <returns>The block's class, lower case, which is what the manifest is keyed by.</returns>
    private static string BlockClassOf(MethodInfo factory) =>
        GrFactories.New(factory.ReturnType)!.BlockClass.ToLowerInvariant();

    /// <summary>The spellings the Python version exports twice, and the one each of them stands
    /// for.</summary> <returns>Alias to primary, both spelled the way this layer spells a
    /// factory.</returns>
    /// <remarks>
    /// Three of these are assignments in the Python version rather than classes (<c>Text =
    /// Textbox</c>, <c>Highlight = HighlightedText</c>, <c>Mic = Microphone</c>, and <c>TabItem =
    /// Tab</c>), which is why the list cannot be derived from the manifest the way the primary
    /// spellings can.
    /// </remarks>
    private static IEnumerable<(string Alias, string Primary)> Aliases() =>
    [
        ("Text", "Textbox"),
        ("Json", "JSON"),
        ("DataFrame", "Dataframe"),
        ("Checkboxgroup", "CheckboxGroup"),
        ("Annotatedimage", "AnnotatedImage"),
        ("Highlightedtext", "HighlightedText"),
        ("Highlight", "HighlightedText"),
        ("Mic", "Microphone"),
        ("TabItem", "Tab"),
    ];

    private static IEnumerable<(string Block, IReadOnlyList<string> Parameters)> UpstreamBlocks() =>
        GrManifest.Blocks();

    /// <summary>Spells a Python parameter name the way the factories spell one.</summary>
    /// <param name="name">The name the Python version declares.</param>
    /// <returns>The name the factory's parameter carries.</returns>
    private static string CamelCase(string name)
    {
        // The same rule the generator applies: each part capitalized and the underscores gone, then
        // the first letter lowered — so a name the Python version writes with a leading underscore,
        // like `_activate`, arrives as `activate` rather than `Activate`.
        var builder = new System.Text.StringBuilder(name.Length);
        var upper = true;

        foreach (var character in name)
        {
            if (character == '_')
            {
                upper = true;
                continue;
            }

            builder.Append(upper ? char.ToUpperInvariant(character) : character);
            upper = false;
        }

        return builder.Length == 0 ? name : char.ToLowerInvariant(builder[0]) + builder.ToString(1, builder.Length - 1);
    }
}
