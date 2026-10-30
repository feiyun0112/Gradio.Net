using System.Reflection;
using Gradio.Net;
using Gradio.Net.Tests.GrLayer;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The third set comparison: what each component raises against what the Python version declares.
/// </summary>
/// <remarks>
/// <para>
/// A component's trigger set is the Python version's list — a textbox raises eight signals, an HTML
/// block raises all thirty-four, a button raises two — and the surface a port binds against is that
/// list. Written by hand it appeared three times per component (a trigger, a raw binding, a doc
/// comment), and the way such a copy fails is by being short: <c>Html</c> declared nine of its
/// thirty-four, and nothing noticed because the only comparison was a person reading the Python
/// version's class body.
/// </para>
/// <para>
/// The trigger sets and the raw bindings are generated from the recorded list, so this is the
/// comparison that says the generation kept it — and, since the recorded list is the Python
/// version's, the comparison that says the layer raises what the Python version raises.
/// </para>
/// </remarks>
public sealed class EventSurfaceTests
{
    [Fact]
    public void Every_component_raises_the_events_upstream_declares()
    {
        var exported = UpstreamDeclaration.Exports();
        var compared = 0;

        foreach (var (block, declared) in UpstreamDeclaration.Events())
        {
            // Only the components, and only the ones the layer builds a block for: a layout's
            // events arrive with the batch that builds its scope, and a name the Python version exports that
            // this layer has no type for is the surface comparison's business rather than this one.
            if (!exported.TryGetValue(declared.Block, out var kind)
                || !string.Equals(kind, "component", StringComparison.Ordinal)
                || ComponentTypeOf(declared.Block) is not { } type)
            {
                continue;
            }

            if (GrFactories.New(type) is not Component component)
            {
                continue;
            }

            var raised = TriggersOf(component);

            // Deduplicated on the Python version's side: its audio component lists `pause` twice, and one
            // signal is one trigger however many times a list names it.
            var expected = declared.Events.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();

            Assert.True(
                expected.SequenceEqual(raised.Order(StringComparer.Ordinal)),
                $"{type.Name} raises ({string.Join(", ", raised.Order(StringComparer.Ordinal))}) and the Python version's "
                + $"{declared.Block} raises ({string.Join(", ", expected)}).");

            compared++;
        }

        // A comparison that compared nothing would pass whatever the components said.
        Assert.True(compared >= 50, $"Only {compared} component(s) were compared; the layer has more.");
    }

    [Fact]
    public void A_component_raises_nothing_it_does_not_declare()
    {
        // The other direction, and the one a generated set can get wrong by generating too much: a
        // trigger nothing the Python version raises is an affordance the frontend never sends, so binding it
        // is a handler that never runs.
        var events = UpstreamDeclaration.Events();
        var exported = UpstreamDeclaration.Exports();

        var unknown = new List<string>();

        foreach (var type in ComponentTypes())
        {
            if (!exported.TryGetValue(type.Name, out var kind)
                || !string.Equals(kind, "component", StringComparison.Ordinal))
            {
                continue;
            }

            var declared = events.Values
                .FirstOrDefault(entry => string.Equals(entry.Block, type.Name, StringComparison.Ordinal));

            if (declared.Block is null)
            {
                continue;
            }

            if (GrFactories.New(type) is not Component component)
            {
                continue;
            }

            var raised = TriggersOf(component);

            unknown.AddRange(raised
                .Except(declared.Events, StringComparer.Ordinal)
                .Select(name => $"{type.Name}.{name}"));
        }

        Assert.True(
            unknown.Count == 0,
            $"These components raise an event the Python version does not declare: {string.Join(", ", unknown)}.");
    }

    /// <summary>The names of the triggers a component's event set exposes.</summary>
    /// <param name="events">The set.</param>
    /// <returns>Each trigger's name, spelled the way the Python version spells it.</returns>
    /// <remarks>
    /// Read from the properties rather than from a list, because that is what a caller sees: a
    /// trigger the set does not expose is a signal nobody can bind, however it is spelled. The two
    /// the framework itself asks about — <c>Submit</c> and <c>Stream</c> — answer <see langword="null"/>
    /// on a component that raises neither, which is how they are left out.
    /// </remarks>
    private static IReadOnlyList<string> TriggersOf(Component component) =>
    [
        .. component.Events.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetIndexParameters().Length == 0)
            .Where(property => property.GetValue(component.Events) is EventTrigger)
            .Select(property => Snake(property.Name)),
    ];

    private static Type? ComponentTypeOf(string name) =>
        ComponentTypes().FirstOrDefault(type => string.Equals(type.Name, name, StringComparison.OrdinalIgnoreCase));

    private static IEnumerable<Type> ComponentTypes() =>
        typeof(Component).Assembly.GetTypes()
            .Where(type => type.IsPublic && !type.IsAbstract && typeof(Component).IsAssignableFrom(type))
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
