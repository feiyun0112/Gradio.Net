using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The JSON schemas the data components describe themselves with.
/// </summary>
/// <remarks>
/// <para>
/// The Python version derives these from its pydantic models, and the documents are served verbatim
/// in the config's <c>api_info</c> and in the API documentation, so a client reads them as the
/// contract for what an endpoint takes and returns. They are written out here rather than derived
/// because there is no pydantic in this framework: the model is a shape the component produces, and
/// this is the one place that says how it is spelled in a schema.
/// </para>
/// <para>
/// The shape is pydantic's, down to the <c>anyOf</c> spelling of an optional field and the
/// <c>additional_description</c> key the component base moves the model's docstring into. The
/// schemas of the components that carry a file are next to the file model instead
/// (<see cref="MediaSchemas"/>), because they name it.
/// </para>
/// </remarks>
internal static class DataSchemas
{
    /// <summary>The schema of a table: headers, rows, and whatever styling came with them.</summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// The Python version's <c>DataframeData</c>. The headers and the rows are required and untyped
    /// — a cell may be text, a number or a boolean, and the component's <c>datatype</c> is what
    /// says which — while the metadata is optional and only a styled dataframe produces it.
    /// </remarks>
    internal static JsonObject DataframeSchema() => new()
    {
        ["properties"] = new JsonObject
        {
            ["headers"] = new JsonObject
            {
                ["items"] = new JsonObject(),
                ["title"] = "Headers",
                ["type"] = "array",
            },
            ["data"] = new JsonObject
            {
                ["items"] = new JsonObject
                {
                    ["items"] = new JsonObject(),
                    ["type"] = "array",
                },
                ["title"] = "Data",
                ["type"] = "array",
            },
            ["metadata"] = new JsonObject
            {
                ["anyOf"] = new JsonArray(
                    new JsonObject
                    {
                        ["additionalProperties"] = new JsonObject
                        {
                            ["anyOf"] = new JsonArray(
                                new JsonObject
                                {
                                    ["items"] = new JsonObject(),
                                    ["type"] = "array",
                                },
                                new JsonObject { ["type"] = "null" }),
                        },
                        ["type"] = "object",
                    },
                    new JsonObject { ["type"] = "null" }),
                ["default"] = null,
                ["title"] = "Metadata",
            },
        },
        ["required"] = new JsonArray("headers", "data"),
        ["title"] = "DataframeData",
        ["type"] = "object",
        ["additional_description"] = null,
    };

    /// <summary>The schema of a highlighted text: a list of tokens, each with its category.</summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// The Python version's <c>HighlightedTextData</c>, which is a root model over the token list:
    /// the document is the list's schema rather than an object with a field holding it. A token's
    /// category is a string for a named category and a number for a confidence, which is why it is
    /// a union.
    /// </remarks>
    internal static JsonObject HighlightedTextSchema() => new()
    {
        ["$defs"] = new JsonObject
        {
            ["HighlightedToken"] = new JsonObject
            {
                ["properties"] = new JsonObject
                {
                    ["token"] = new JsonObject
                    {
                        ["title"] = "Token",
                        ["type"] = "string",
                    },
                    ["class_or_confidence"] = new JsonObject
                    {
                        ["anyOf"] = new JsonArray(
                            new JsonObject { ["type"] = "string" },
                            new JsonObject { ["type"] = "number" },
                            new JsonObject { ["type"] = "null" }),
                        ["default"] = null,
                        ["title"] = "Class Or Confidence",
                    },
                },
                ["required"] = new JsonArray("token"),
                ["title"] = "HighlightedToken",
                ["type"] = "object",
            },
        },
        ["items"] = new JsonObject { ["$ref"] = "#/$defs/HighlightedToken" },
        ["title"] = "HighlightedTextData",
        ["type"] = "array",
        ["additional_description"] = null,
    };

    /// <summary>The schema of a classification: the top label and the scores below it.</summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// The Python version's <c>LabelData</c>. A label may be text or a number, because a
    /// regression's answer is a number, and both fields are optional because a bare answer carries
    /// no scores at all.
    /// </remarks>
    internal static JsonObject LabelSchema() => new()
    {
        ["$defs"] = new JsonObject
        {
            ["LabelConfidence"] = new JsonObject
            {
                ["properties"] = new JsonObject
                {
                    ["label"] = LabelField("Label"),
                    ["confidence"] = new JsonObject
                    {
                        ["anyOf"] = new JsonArray(
                            new JsonObject { ["type"] = "number" },
                            new JsonObject { ["type"] = "null" }),
                        ["default"] = null,
                        ["title"] = "Confidence",
                    },
                },
                ["title"] = "LabelConfidence",
                ["type"] = "object",
            },
        },
        ["properties"] = new JsonObject
        {
            ["label"] = LabelField("Label"),
            ["confidences"] = new JsonObject
            {
                ["anyOf"] = new JsonArray(
                    new JsonObject
                    {
                        ["items"] = new JsonObject { ["$ref"] = "#/$defs/LabelConfidence" },
                        ["type"] = "array",
                    },
                    new JsonObject { ["type"] = "null" }),
                ["default"] = null,
                ["title"] = "Confidences",
            },
        },
        ["title"] = "LabelData",
        ["type"] = "object",
        ["additional_description"] = null,
    };

    /// <summary>The schema of a plot: the library that drew it, and the drawing.</summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// The Python version's <c>PlotData</c>. The library is an enumeration of the four the Python
    /// version draws, and the drawing is text — a library's own JSON, or an image's base64 — so
    /// nothing in the schema says what is inside it.
    /// </remarks>
    internal static JsonObject PlotSchema() => new()
    {
        ["properties"] = new JsonObject
        {
            ["type"] = new JsonObject
            {
                ["enum"] = new JsonArray("altair", "bokeh", "plotly", "matplotlib"),
                ["title"] = "Type",
                ["type"] = "string",
            },
            ["plot"] = new JsonObject
            {
                ["title"] = "Plot",
                ["type"] = "string",
            },
        },
        ["required"] = new JsonArray("type", "plot"),
        ["title"] = "PlotData",
        ["type"] = "object",
        ["additional_description"] = null,
    };

    /// <summary>A field that is text, a whole number, a number, or nothing.</summary>
    /// <param name="title">The field's title.</param>
    /// <returns>The field's schema.</returns>
    private static JsonObject LabelField(string title) => new()
    {
        ["anyOf"] = new JsonArray(
            new JsonObject { ["type"] = "string" },
            new JsonObject { ["type"] = "integer" },
            new JsonObject { ["type"] = "number" },
            new JsonObject { ["type"] = "null" }),
        ["default"] = null,
        ["title"] = title,
    };
}
