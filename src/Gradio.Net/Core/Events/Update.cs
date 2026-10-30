using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// What a handler answers with when it changes some of an output's props rather than replacing its
/// value: the Python version's <c>gr.update(...)</c>, with the props named and typed.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>gr.update</c> takes keyword arguments, and every keyword it accepts is a
/// prop some component carries. C# has no keyword splat, so the props are properties of this record
/// — spelled the C# way, checked by the compiler, and written to the config by the same rule the
/// rest of the props are written by.
/// </para>
/// <para>
/// A property left <see langword="null"/> is not part of the update: the client merges what it is
/// given and leaves the rest alone. That is why every property is nullable, and why
/// <c>new Update()</c> means "change nothing" — the same document as <see cref="gr.Skip"/>.
/// </para>
/// <para>
/// This is one of the three shapes an output can be answered with. Returning the component itself
/// carries every prop the application gave it at construction; returning this carries only what the
/// handler named. A chat needs the second to empty a field without also resetting the placeholder
/// the field was drawn with.
/// </para>
/// </remarks>
public sealed record Update
{
    /// <summary>Stands for "write this prop as null" rather than leaving it out of the update.</summary>
    /// <remarks>
    /// A property left <see langword="null"/> is not part of the update, so a handler that wants an
    /// output emptied has to say so: <c>Value = Update.Clear</c> writes <c>"value": null</c>, which
    /// is what the client reads as "no value". The Python version draws the same line between
    /// <c>gr.update()</c> and <c>gr.update(value=None)</c>.
    /// </remarks>
    public static readonly object Clear = new ClearMarker();

    /// <summary>The type of the sentinel, so that a record cannot be built from a look-alike.</summary>
    private sealed class ClearMarker
    {
    }

    /// <summary>The value the component carries.</summary>
    public object? Value { get; init; }

    /// <summary>The label drawn above the component.</summary>
    public string? Label { get; init; }

    /// <summary>Whether the component is drawn at all, and how much room it gives up when it is not.</summary>
    /// <remarks>
    /// <see cref="Visibility"/> converts from a boolean, so <c>Visible = true</c>, <c>Visible = false</c>
    /// and <c>Visible = Visibility.Hidden</c> are all this one property — the three states the
    /// Python version's <c>visible</c> accepts.
    /// </remarks>
    public Visibility? Visible { get; init; }

    /// <summary>Whether the user can change the component.</summary>
    public bool? Interactive { get; init; }

    /// <summary>The options a component offers.</summary>
    public IReadOnlyList<Choice>? Choices { get; init; }

    /// <summary>The smallest value a range accepts.</summary>
    public double? Minimum { get; init; }

    /// <summary>The largest value a range accepts.</summary>
    public double? Maximum { get; init; }

    /// <summary>The increment a range moves by.</summary>
    public double? Step { get; init; }

    /// <summary>The hint drawn inside an empty field.</summary>
    public string? Placeholder { get; init; }

    /// <summary>The note drawn under the component.</summary>
    public string? Info { get; init; }

    /// <summary>Whether the label is drawn.</summary>
    public bool? ShowLabel { get; init; }

    /// <summary>Whether the component is drawn inside a container.</summary>
    public bool? Container { get; init; }

    /// <summary>The share of the space a component in a row takes.</summary>
    public double? Scale { get; init; }

    /// <summary>The narrowest the component may be drawn.</summary>
    public double? MinWidth { get; init; }

    /// <summary>Whether a disclosure is open.</summary>
    public bool? Open { get; init; }

    /// <summary>The height the component is drawn at.</summary>
    public double? Height { get; init; }

    /// <summary>The width the component is drawn at.</summary>
    public double? Width { get; init; }

    /// <summary>How many lines a text field shows.</summary>
    public int? LineCount { get; init; }

    /// <summary>How many lines a text field may grow to.</summary>
    public int? MaxLines { get; init; }

    /// <summary>What kind of value the component carries.</summary>
    public string? Type { get; init; }

    /// <summary>The format a value is written in.</summary>
    public string? Format { get; init; }

    /// <summary>The column names a table carries.</summary>
    public object? Headers { get; init; }

    /// <summary>The column types a table carries.</summary>
    public object? Datatype { get; init; }

    /// <summary>How many rows a table shows.</summary>
    public int? RowCount { get; init; }

    /// <summary>How many columns a table shows.</summary>
    public int? ColumnCount { get; init; }

    /// <summary>Whether a layout draws the blocks written into it.</summary>
    public bool? RenderChildren { get; init; }

    /// <summary>Whether a chat scrolls to its last message.</summary>
    public bool? Autoscroll { get; init; }

    /// <summary>Whether markup a component is given is sanitised before it is drawn.</summary>
    public bool? SanitizeHtml { get; init; }

    /// <summary>Whether a code block offers its copy button.</summary>
    public bool? ShowCopyButton { get; init; }

    /// <summary>Which visual variant a button is drawn in.</summary>
    public string? Variant { get; init; }

    /// <summary>Which size a component is drawn at.</summary>
    public string? Size { get; init; }

    /// <summary>The icon a button is drawn with.</summary>
    public string? Icon { get; init; }

    /// <summary>The address a link or a button points at.</summary>
    public string? Link { get; init; }

    /// <summary>The element id the component is drawn with.</summary>
    public string? ElemId { get; init; }

    /// <summary>The classes the component is drawn with.</summary>
    public object? ElemClasses { get; init; }

    /// <summary>The update as the protocol carries it: the props, and the marker that says so.</summary>
    /// <returns>The document a handler's answer becomes.</returns>
    /// <remarks>
    /// Each prop is written by <see cref="PropWriter.Serialize"/>, which is the rule the config
    /// itself uses: a value this framework has a type for keeps its wire form, so a visibility stays
    /// the word the frontend reads and a list of choices stays the pairs the config carries.
    /// </remarks>
    public JsonObject ToJson()
    {
        List<(string Prop, JsonNode? Value)> props = [];

        void Add(string prop, object? value)
        {
            if (value is null)
            {
                return;
            }

            props.Add((prop, ReferenceEquals(value, Clear) ? null : PropWriter.Serialize(value)));
        }

        Add("value", Value);
        Add("label", Label);
        Add("visible", Visible);
        Add("interactive", Interactive);
        Add("choices", Choices);
        Add("minimum", Minimum);
        Add("maximum", Maximum);
        Add("step", Step);
        Add("placeholder", Placeholder);
        Add("info", Info);
        Add("show_label", ShowLabel);
        Add("container", Container);
        Add("scale", Scale);
        Add("min_width", MinWidth);
        Add("open", Open);
        Add("height", Height);
        Add("width", Width);
        Add("line_count", LineCount);
        Add("max_lines", MaxLines);
        Add("type", Type);
        Add("format", Format);
        Add("headers", Headers);
        Add("datatype", Datatype);
        Add("row_count", RowCount);
        Add("column_count", ColumnCount);
        Add("render_children", RenderChildren);
        Add("autoscroll", Autoscroll);
        Add("sanitize_html", SanitizeHtml);
        Add("show_copy_button", ShowCopyButton);
        Add("variant", Variant);
        Add("size", Size);
        Add("icon", Icon);
        Add("link", Link);
        Add("elem_id", ElemId);
        Add("elem_classes", ElemClasses);

        return PropertyUpdate.Partial([.. props]);
    }
}
