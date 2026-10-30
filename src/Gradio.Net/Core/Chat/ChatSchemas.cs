using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The JSON schema a chat component describes itself with.
/// </summary>
/// <remarks>
/// <para>
/// The Python version derives this from its pydantic models, and the document is served verbatim in
/// the config's <c>api_info</c> and in the OpenAPI document, so a client reads it as the contract
/// for what an endpoint takes and returns. It is written out here rather than derived because there
/// is no pydantic in this framework: the models are <see cref="ChatMessage"/> and its parts, and
/// this is the one place that says how they are spelled in a schema.
/// </para>
/// <para>
/// The shape is pydantic's, down to the <c>anyOf</c> spelling of an optional field, the
/// <c>$defs</c> the nested models are referenced from, and the docstrings the Python version's
/// dictionaries carry as descriptions. A client that generates a call from this document gets the
/// same call from either implementation.
/// </para>
/// </remarks>
internal static class ChatSchemas
{
    /// <summary>The docstring the Python version's <c>MetadataDict</c> carries, which its schema
    /// reports.</summary>
    private const string MetadataDescription =
        "A typed dictionary to represent metadata for a message in the Chatbot component. An\n"
        + "instance of this dictionary is used for the `metadata` field in a ChatMessage when\n"
        + "the chat message should be displayed as a thought.\nParameters:\n"
        + "    title: The title of the \"thought\" message. Required if the message is to be displayed as a thought.\n"
        + "    id: The ID of the message. Only used for nested thoughts. Nested thoughts can be nested by setting the parent_id to the id of the parent thought.\n"
        + "    parent_id: The ID of the parent message. Only used for nested thoughts.\n"
        + "    log: A string message to display next to the thought title in a subdued font.\n"
        + "    duration: The duration of the message in seconds. Appears next to the thought title in a subdued font inside a parentheses.\n"
        + "    status: if set to `\"pending\"`, a spinner appears next to the thought title and the accordion is initialized open.  If `status` is `\"done\"`, the thought accordion is initialized closed. If `status` is not provided, the thought accordion is initialized open and no spinner is displayed.";

    /// <summary>The docstring the Python version's <c>OptionDict</c> carries, which its schema
    /// reports.</summary>
    private const string OptionDescription =
        "A typed dictionary to represent an option in a ChatMessage. A list of these\n"
        + "dictionaries is used for the `options` field in a ChatMessage.\nParameters:\n"
        + "    value: The value to return when the option is selected.\n"
        + "    label: The text to display in the option, if different from the value.";

    /// <summary>The docstring the Python version's <c>FileData</c> carries, which its schema
    /// reports.</summary>
    private const string FileDataDescription =
        "The FileData class is a subclass of the GradioModel class that represents a file object "
        + "within a Gradio interface. It is used to store file data and metadata when a file is "
        + "uploaded.\n\nAttributes:\n    path: The server file path where the file is stored.\n    "
        + "url: The normalized server URL pointing to the file.\n    size: The size of the file in "
        + "bytes.\n    orig_name: The original filename before upload.\n    mime_type: The MIME type "
        + "of the file.\n    is_stream: Indicates whether the file is a stream.\n    meta: Additional "
        + "metadata used internally (should not be changed).";

    /// <summary>The schema of a conversation, as a chat component describes it.</summary>
    /// <returns>A new document.</returns>
    internal static JsonObject ChatbotSchema() => new()
    {
        ["$defs"] = new JsonObject
        {
            ["ComponentMessage"] = ComponentMessageSchema(),
            ["FileData"] = FileDataSchema(),
            ["FileDataMeta"] = new JsonObject
            {
                ["properties"] = new JsonObject
                {
                    ["_type"] = new JsonObject
                    {
                        ["const"] = FileData.FileMetaType,
                        ["title"] = "Type",
                        ["type"] = "string",
                    },
                },
                ["required"] = new JsonArray("_type"),
                ["title"] = "FileDataMeta",
                ["type"] = "object",
            },
            ["FileMessage"] = new JsonObject
            {
                ["properties"] = new JsonObject
                {
                    ["file"] = new JsonObject { ["$ref"] = "#/$defs/FileData" },
                    ["alt_text"] = new JsonObject
                    {
                        ["anyOf"] = new JsonArray(
                            new JsonObject { ["type"] = "string" },
                            new JsonObject { ["type"] = "null" }),
                        ["default"] = null,
                        ["title"] = "Alt Text",
                    },
                    ["type"] = TypeMarker("file"),
                },
                ["required"] = new JsonArray("file"),
                ["title"] = "FileMessage",
                ["type"] = "object",
            },
            ["Message"] = MessageSchema(),
            ["MetadataDict"] = new JsonObject
            {
                ["description"] = MetadataDescription,
                ["properties"] = new JsonObject
                {
                    ["title"] = new JsonObject { ["title"] = "Title", ["type"] = "string" },
                    ["id"] = NumberOrString("Id"),
                    ["parent_id"] = NumberOrString("Parent Id"),
                    ["log"] = new JsonObject { ["title"] = "Log", ["type"] = "string" },
                    ["duration"] = new JsonObject { ["title"] = "Duration", ["type"] = "number" },
                    ["status"] = new JsonObject
                    {
                        ["enum"] = new JsonArray(MessageStatus.Pending, MessageStatus.Done),
                        ["title"] = "Status",
                        ["type"] = "string",
                    },
                },
                ["title"] = "MetadataDict",
                ["type"] = "object",
            },
            ["OptionDict"] = new JsonObject
            {
                ["description"] = OptionDescription,
                ["properties"] = new JsonObject
                {
                    ["value"] = new JsonObject { ["title"] = "Value", ["type"] = "string" },
                    ["label"] = new JsonObject { ["title"] = "Label", ["type"] = "string" },
                },
                ["required"] = new JsonArray("value"),
                ["title"] = "OptionDict",
                ["type"] = "object",
            },
            ["TextMessage"] = new JsonObject
            {
                ["properties"] = new JsonObject
                {
                    ["text"] = new JsonObject { ["title"] = "Text", ["type"] = "string" },
                    ["type"] = TypeMarker("text"),
                },
                ["required"] = new JsonArray("text"),
                ["title"] = "TextMessage",
                ["type"] = "object",
            },
        },
        ["items"] = new JsonObject { ["$ref"] = "#/$defs/Message" },
        ["title"] = "ChatbotDataMessages",
        ["type"] = "array",
        ["additional_description"] = null,
    };

    /// <summary>The schema of one message.</summary>
    private static JsonObject MessageSchema() => new()
    {
        ["properties"] = new JsonObject
        {
            ["role"] = new JsonObject { ["title"] = "Role", ["type"] = "string" },
            ["metadata"] = new JsonObject
            {
                ["anyOf"] = new JsonArray(
                    new JsonObject { ["$ref"] = "#/$defs/MetadataDict" },
                    new JsonObject { ["type"] = "null" }),
                ["default"] = null,
            },
            ["content"] = new JsonObject
            {
                ["items"] = new JsonObject
                {
                    ["anyOf"] = new JsonArray(
                        new JsonObject { ["$ref"] = "#/$defs/TextMessage" },
                        new JsonObject { ["$ref"] = "#/$defs/FileMessage" },
                        new JsonObject { ["$ref"] = "#/$defs/ComponentMessage" }),
                },
                ["title"] = "Content",
                ["type"] = "array",
            },
            ["options"] = new JsonObject
            {
                ["anyOf"] = new JsonArray(
                    new JsonObject
                    {
                        ["items"] = new JsonObject { ["$ref"] = "#/$defs/OptionDict" },
                        ["type"] = "array",
                    },
                    new JsonObject { ["type"] = "null" }),
                ["default"] = null,
                ["title"] = "Options",
            },
        },
        ["required"] = new JsonArray("role", "content"),
        ["title"] = "Message",
        ["type"] = "object",
    };

    /// <summary>The schema of a message that holds a component.</summary>
    private static JsonObject ComponentMessageSchema() => new()
    {
        ["properties"] = new JsonObject
        {
            ["component"] = new JsonObject { ["title"] = "Component", ["type"] = "string" },

            // The value has no type: a component's value is whatever that component carries, which
            // is a schema of its own.
            ["value"] = new JsonObject { ["title"] = "Value" },
            ["constructor_args"] = new JsonObject
            {
                ["additionalProperties"] = true,
                ["title"] = "Constructor Args",
                ["type"] = "object",
            },
            ["props"] = new JsonObject
            {
                ["additionalProperties"] = true,
                ["title"] = "Props",
                ["type"] = "object",
            },
            ["type"] = TypeMarker("component"),
        },
        ["required"] = new JsonArray("component", "value", "constructor_args", "props"),
        ["title"] = "ComponentMessage",
        ["type"] = "object",
    };

    /// <summary>The schema of the file a message holds.</summary>
    private static JsonObject FileDataSchema() => new()
    {
        ["description"] = FileDataDescription,
        ["properties"] = new JsonObject
        {
            ["path"] = new JsonObject { ["title"] = "Path", ["type"] = "string" },
            ["url"] = Optional("string", "Url"),
            ["size"] = Optional("integer", "Size"),
            ["orig_name"] = Optional("string", "Orig Name"),
            ["mime_type"] = Optional("string", "Mime Type"),
            ["is_stream"] = new JsonObject
            {
                ["default"] = false,
                ["title"] = "Is Stream",
                ["type"] = "boolean",
            },
            ["meta"] = new JsonObject { ["$ref"] = "#/$defs/FileDataMeta" },
        },
        ["required"] = new JsonArray("path"),
        ["title"] = "FileData",
        ["type"] = "object",
    };

    /// <summary>The field that says which block this is, which is a constant rather than an enum.</summary>
    private static JsonObject TypeMarker(string type) => new()
    {
        ["const"] = type,
        ["default"] = type,
        ["title"] = "Type",
        ["type"] = "string",
    };

    /// <summary>A field that may be a number or a string, as the Python version's ids
    /// are.</summary>
    private static JsonObject NumberOrString(string title) => new()
    {
        ["anyOf"] = new JsonArray(
            new JsonObject { ["type"] = "integer" },
            new JsonObject { ["type"] = "string" }),
        ["title"] = title,
    };

    /// <summary>A field that may be null, as pydantic spells one.</summary>
    private static JsonObject Optional(string type, string title) => new()
    {
        ["anyOf"] = new JsonArray(
            new JsonObject { ["type"] = type },
            new JsonObject { ["type"] = "null" }),
        ["default"] = null,
        ["title"] = title,
    };
}
