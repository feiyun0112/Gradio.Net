using System.Collections;
namespace Gradio.Net;
/// <summary>
/// One side of an <see cref="Interface"/>: a component, the name of one, or a list of either.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>str | Component | Sequence[str | Component] | None</c>. A type rather than an
/// <see cref="object"/> so that a mistake stays a compile error where it can be one, and a
/// collection so that a list reads the way a list reads: <c>inputs: [name, count]</c>.
/// </para>
/// <para>
/// Every spelling of a component or a name works here — <c>inputs: "text"</c>,
/// <c>inputs: new Textbox()</c>, <c>inputs: [name, count]</c>, <c>inputs: ["text", "number"]</c> —
/// and a name becomes the component that name means, with every default, through the same table the
/// rest of the framework resolves names with. A name that is not a component is refused where it is
/// written, which is the moment the Python version would have built the component too.
/// </para>
/// <para>
/// It enumerates <see cref="object"/> rather than <see cref="Component"/> on purpose. A collection
/// expression takes its element type from the sequence the target implements, so a type that
/// enumerated components would accept only components in a list — <c>["text", "number"]</c> would
/// not compile, because a name is not a component until it is resolved. What an
/// <see cref="Interface"/> keeps afterwards is its own list of components, which is
/// <see cref="Components"/>; a sequence already built is spread into one —
/// <c>inputs: [.. components]</c> — because C# allows a user-defined conversion neither to nor from
/// an interface.
/// </para>
/// </remarks>
public sealed class ComponentCollection : IEnumerable<object>
{
    private readonly List<Component> _components;

    /// <summary>Creates an empty list.</summary>
    /// <remarks>
    /// What a collection expression starts from: <c>inputs: [a, b]</c> is this, then two
    /// <see cref="Add"/> calls.
    /// </remarks>
    public ComponentCollection() => _components = [];

    private ComponentCollection(IEnumerable<Component> components) => _components = [.. components];

    /// <summary>The components the entries named, in the order they were given.</summary>
    public IReadOnlyList<Component> Components => _components;

    /// <summary>Reads a component as a list of one.</summary>
    /// <param name="component">The component.</param>
    public static implicit operator ComponentCollection(Component component) => new([component]);

    /// <summary>Reads the name of a component as the component it names.</summary>
    /// <param name="shortcut">The name, as the Python version spells it: <c>"text"</c>,
    /// <c>"image"</c>.</param> <exception cref="ArgumentException">The name is not one of the
    /// components.</exception>
    public static implicit operator ComponentCollection(string shortcut) =>
        new([ComponentShortcuts.Resolve(shortcut)]);

    /// <summary>Adds a component, or the name of one, to the list.</summary>
    /// <param name="element">The component, or its name.</param>
    /// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The entry is neither a component nor the name of one.</exception>
    /// <remarks>
    /// Takes an <see cref="object"/> rather than a <see cref="Component"/> so that a list may mix
    /// the two spellings — <c>inputs: [textbox, "number"]</c> — which is a thing the Python
    /// version's own documentation does. A collection expression works out one element type for the
    /// whole list, and only <see cref="object"/> is a type that both a component and a name are.
    /// </remarks>
    public void Add(object element)
    {
        ArgumentNullException.ThrowIfNull(element);

        _components.Add(ComponentShortcuts.Resolve(element));
    }

    /// <summary>Walks the components, in the order they were given.</summary>
    /// <returns>The components.</returns>
    public IEnumerator<object> GetEnumerator() =>
        _components.Select(component => (object)component).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
