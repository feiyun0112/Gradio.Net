using System.Reflection;
using Gradio.Net;
using Gradio.Net.Tests.GrLayer;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The third set comparison: what each block raises against what the Python version declares.
/// </summary>
/// <remarks>
/// <para>
/// A block's trigger set is the Python version's list — a textbox raises eight signals, an HTML
/// block raises all thirty-four, a button raises two, a tab raises one — and the surface a port
/// binds against is that list. Written by hand it appeared three times per block (a trigger, a raw
/// binding, a doc comment), and the way such a copy fails is by being short: <c>Html</c> declared
/// nine of its thirty-four, and nothing noticed because the only comparison was a person reading the
/// Python version's class body.
/// </para>
/// <para>
/// The trigger sets and the raw bindings are generated from the recorded list, so this is the
/// comparison that says the generation kept it — and, since the recorded list is the Python
/// version's, the comparison that says the layer raises what the Python version raises.
/// </para>
/// <para>
/// Both halves of the tree are compared. A layout raises events as well — an accordion opens, a tab
/// is selected — and the Python version declares <c>EVENTS</c> for it, so the same generation
/// answers for both and the same comparison has to hold it to the same list.
/// </para>
/// </remarks>
public sealed class EventSurfaceTests
{
    /// <summary>The kinds of block whose events the Python version declares.</summary>
    /// <remarks>
    /// Two runs of one comparison rather than two tests: the question is the same, and only the
    /// table it walks differs.
    /// </remarks>
    public static TheoryData<string, int> Kinds => new()
    {
        { "component", 50 },
        { "layout", 6 },
    };

    [Theory]
    [MemberData(nameof(Kinds))]
    public void Every_block_raises_the_events_upstream_declares(string kind, int least)
    {
        var exported = Exports();
        var events = DeclaredEvents();
        var compared = 0;

        foreach (var type in BlockTypes())
        {
            if (GrFactories.New(type) is not { } block
                || !exported.TryGetValue(block.BlockClass.ToLowerInvariant(), out var exportedAs)
                || !string.Equals(exportedAs, kind, StringComparison.Ordinal))
            {
                continue;
            }

            // The block's own name, not the type's: a step is a `WalkthroughStep` in C# and a
            // `Step` in the Python version, and the manifests are keyed by the Python version's
            // name, spelled in lower case.
            if (!events.TryGetValue(block.BlockClass.ToLowerInvariant(), out var declared))
            {
                continue;
            }

            var raised = TriggersOf(block);

            // Deduplicated on the Python version's side: its audio component lists `pause` twice, and one
            // signal is one trigger however many times a list names it.
            var expected = declared.Events.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();

            Assert.True(
                expected.SequenceEqual(raised.Order(StringComparer.Ordinal)),
                $"{type.Name} raises ({string.Join(", ", raised.Order(StringComparer.Ordinal))}) and the Python version's "
                + $"{declared.Block} raises ({string.Join(", ", expected)}).");

            compared++;
        }

        // A comparison that compared nothing would pass whatever the blocks said.
        Assert.True(compared >= least, $"Only {compared} {kind}(s) were compared; the layer has more.");
    }

    [Theory]
    [MemberData(nameof(Kinds))]
    public void A_block_raises_nothing_it_does_not_declare(string kind, int least)
    {
        // The other direction, and the one a generated set can get wrong by generating too much: a
        // trigger nothing the Python version raises is an affordance the frontend never sends, so binding it
        // is a handler that never runs.
        var exported = Exports();
        var events = DeclaredEvents();

        var unknown = new List<string>();
        var compared = 0;

        foreach (var type in BlockTypes())
        {
            if (GrFactories.New(type) is not { } block
                || !exported.TryGetValue(block.BlockClass.ToLowerInvariant(), out var exportedAs)
                || !string.Equals(exportedAs, kind, StringComparison.Ordinal)
                || !events.TryGetValue(block.BlockClass.ToLowerInvariant(), out var declared))
            {
                continue;
            }

            var raised = TriggersOf(block);

            unknown.AddRange(raised
                .Except(declared.Events, StringComparer.Ordinal)
                .Select(name => $"{type.Name}.{name}"));

            compared++;
        }

        Assert.True(compared >= least, $"Only {compared} {kind}(s) were compared; the layer has more.");
        Assert.True(
            unknown.Count == 0,
            $"These blocks raise an event the Python version does not declare: {string.Join(", ", unknown)}.");
    }

    /// <summary>The kinds the Python version exports, by the name a block reports.</summary>
    /// <returns>Each export's kind, keyed the way <see cref="Block.BlockClass"/> spells a block —
    /// in lower case.</returns>
    /// <remarks>
    /// Grouped rather than copied: the Python version exports two spellings of a few names
    /// (<c>AnnotatedImage</c> and <c>Annotatedimage</c>, <c>Dataframe</c> and <c>DataFrame</c>), and
    /// a block reports the lower-cased name of its class, so the two spellings are one key here.
    /// </remarks>
    private static Dictionary<string, string> Exports() =>
        UpstreamDeclaration.Exports()
            .GroupBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key.ToLowerInvariant(),
                group => group.First().Value,
                StringComparer.Ordinal);

    /// <summary>The events the Python version declares, by the name a block reports.</summary>
    /// <returns>Each block's class name and events, keyed the way <see cref="Block.BlockClass"/>
    /// spells a block.</returns>
    private static Dictionary<string, (string Block, IReadOnlyList<string> Events)> DeclaredEvents() =>
        UpstreamDeclaration.Events().Values
            .GroupBy(entry => entry.Block, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key.ToLowerInvariant(),
                group => group.First(),
                StringComparer.Ordinal);

    /// <summary>The names of the triggers a block's event set exposes.</summary>
    /// <param name="block">The block.</param>
    /// <returns>Each trigger's name, spelled the way the Python version spells it.</returns>
    /// <remarks>
    /// Read from the properties rather than from a list, because that is what a caller sees: a
    /// trigger the set does not expose is a signal nobody can bind, however it is spelled. The two
    /// the framework itself asks about — <c>Submit</c> and <c>Stream</c> — answer <see langword="null"/>
    /// on a component that raises neither, which is how they are left out.
    /// </remarks>
    private static IReadOnlyList<string> TriggersOf(Block block) =>
    [
        .. block.Events.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetIndexParameters().Length == 0)
            .Where(property => property.GetValue(block.Events) is EventTrigger)
            .Select(property => Snake(property.Name)),
    ];

    private static IEnumerable<Type> BlockTypes() =>
        typeof(Block).Assembly.GetTypes()
            .Where(type => type.IsPublic && !type.IsAbstract && typeof(Block).IsAssignableFrom(type))
            .OrderBy(type => type.Name, StringComparer.Ordinal);

    /// <summary>Spells a trigger's name the way the Python version spells an event: <c>KeyUp</c> is
    /// <c>key_up</c>.</summary>
    private static string Snake(string name)
    {
        var builder = new System.Text.StringBuilder(name.Length + 4);

        foreach (var character in name)
        {
            if (char.IsUpper(character) && builder.Length > 0)
            {
                builder.Append('_');
            }

            builder.Append(char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }
}
