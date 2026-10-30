using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A button that sets the components it was given back to their empty values when it is clicked.
/// </summary>
/// <remarks>
/// <para>
/// The clearing itself is a fragment of JavaScript the button binds to its own <c>click</c> event:
/// the values it writes are the ones its components answer to <c>postprocess(None)</c>, which is
/// what "empty" means for each of them — nothing for a textbox, <see langword="false"/> for a
/// checkbox, the bottom of the range for a slider. No handler runs on the server, so the button
/// clears a page even when the queue is busy.
/// </para>
/// <para>
/// A state is the exception, because its value is not the client's: clearing one means writing the
/// value the session started from, which only the server can do. So a clear button with a state in
/// its list binds a second dependency that runs on the server and writes the initial values back,
/// and that is why the Python version has two bindings here rather than one.
/// </para>
/// <para>
/// <c>api_name</c> and <c>api_visibility</c> are props rather than binding parameters because the
/// Python version declares them as constructor parameters of this button and writes them into its
/// props; they are also what the two bindings are named and exposed under.
/// </para>
/// </remarks>
[GradioBlock("clearbutton")]
public sealed partial class ClearButton : Button
{
    internal ClearButton()
    {
    }

    /// <summary>The components the button clears, in the order they were given.</summary>
    /// <value>
    /// Empty for a button that clears nothing, which binds nothing either — the Python version's
    /// rule, so that an interface can create one before it has the components to give it.
    /// </value>
    /// <remarks>
    /// Not a prop of the config, and marked so: the Python version takes it as a constructor
    /// parameter but never stores it, so the config of a clear button says nothing about what it
    /// clears.
    /// </remarks>
    [OmitFromConfig]
    public IReadOnlyList<Component> Components { get; set; } = [];

    /// <summary>The name the clearing endpoint is exposed under, or <see langword="null"/> for none.</summary>
    public string? ApiName { get; set; }

    /// <summary>
    /// How far the state-clearing endpoint is exposed: <c>public</c>, <c>private</c> or
    /// <c>undocumented</c>.
    /// </summary>
    /// <value>
    /// <c>undocumented</c>, which is the Python version's default: the endpoint is reachable and
    /// left out of the API documentation.
    /// </value>
    public string ApiVisibility { get; set; } = "undocumented";

    /// <summary>Whether the block is drawn by its parent's component.</summary> <remarks>The Python
    /// version marks this button a template: the frontend draws it with the button component, and
    /// the config names that component rather than this class.</remarks>
    protected override bool IsTemplate => true;

    /// <summary>The text drawn when the application names none.</summary>
    protected override string? DefaultValue => "Clear";

    /// <inheritdoc />
    protected internal override void OnAddedTo(Blocks root)
    {
        base.OnAddedTo(root);

        if (Components.Count > 0)
        {
            BindComponents(Components);
        }
    }

    /// <summary>Adds components to the list the button clears, and binds the clearing of them.</summary>
    /// <param name="components">The components to clear.</param>
    /// <returns>This button, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="components"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The same thing the constructor does in the Python version, exposed because a button is often
    /// given its components one at a time. Binding here rather than at the next render is
    /// deliberate: a dependency is numbered when it is created, so a caller that adds components
    /// after binding other events gets the order it wrote. A button that has not been added to an
    /// application yet has no binding to make, so the components wait for it.
    /// </remarks>
    public ClearButton Add(params Component[] components)
    {
        ArgumentNullException.ThrowIfNull(components);

        if (components.Length == 0)
        {
            return this;
        }

        var list = new List<Component>(Components);
        list.AddRange(components);
        Components = list;

        if (Id != UnrenderedId)
        {
            BindComponents(components);
        }

        return this;
    }

    /// <summary>
    /// The value the button receives from what a handler returned.
    /// </summary>
    /// <param name="value">The value the handler returned.</param>
    /// <returns>The value as it was given, or <see langword="null"/> for none.</returns>
    /// <remarks>
    /// Unlike a plain button, this one passes the value through rather than turning it into text:
    /// the Python version's clear button overrides the conversion to leave it alone, which is what
    /// makes it the one button whose label can be set to nothing.
    /// </remarks>
    public override JsonNode? Postprocess(object? value) =>
        value is null ? null : JsonValue.Create(value.ToString());

    /// <summary>Binds the click that clears the given components.</summary>
    /// <param name="components">The components to clear.</param>
    private void BindComponents(IReadOnlyList<Component> components)
    {
        // The values a component is cleared to, as the frontend writes them: the same answer each
        // component gives to being postprocessed from nothing.
        var cleared = new JsonArray();
        foreach (var component in components)
        {
            cleared.Add(component.Postprocess(null));
        }

        Click(
            fn: null,
            inputs: [],
            outputs: [.. components],
             js: "() => " + PythonJson.Dump(cleared), apiName: ApiName, apiVisibility: Gradio.Net.ApiVisibility.Private);

        // A state is not the client's to clear: the value it goes back to lives on the server, so
        // clearing one is a handler that returns the values the session started from.
        var states = components.OfType<IStatefulComponent>().ToArray();
        if (states.Length == 0)
        {
            return;
        }

        var initial = states.Select(state => state.CreateState()?.DeepClone()).ToArray();

        Click(
            fn: (Func<object?>)(() => initial.Length == 1 ? initial[0] : new InitialStates(initial)),
            inputs: [],
            outputs: [.. states],
             apiName: ApiName, apiVisibility: VisibilityOf(ApiVisibility));
    }

    /// <summary>The spelling of a visibility as the binding parameters take it.</summary>
    /// <param name="visibility">The spelling the component carries as a prop.</param>
    /// <returns>The value the binding is served with.</returns>
    /// <exception cref="ArgumentException"><paramref name="visibility"/> is not one of the three.</exception>
    private static ApiVisibility VisibilityOf(string visibility) => visibility switch
    {
        "public" => Gradio.Net.ApiVisibility.Public,
        "private" => Gradio.Net.ApiVisibility.Private,
        "undocumented" => Gradio.Net.ApiVisibility.Undocumented,
        _ => throw new ArgumentException(
            $"A clear button's api_visibility must be public, private or undocumented; got '{visibility}'.",
            nameof(visibility)),
    };

    /// <summary>The values several states are reset to, as one result.</summary>
    /// <param name="values">One value per state, in the order the states were given.</param>
    /// <remarks>
    /// A result is spread across a binding's outputs only when it is a tuple, and how many states a
    /// button clears is not known until it is built — so this is a tuple of the length that was
    /// asked for rather than one of the fixed arities <see cref="ValueTuple"/> offers.
    /// </remarks>
    private sealed class InitialStates(object?[] values) : ITuple
    {
        /// <inheritdoc />
        public int Length => values.Length;

        /// <inheritdoc />
        public object? this[int index] => values[index];
    }
}
