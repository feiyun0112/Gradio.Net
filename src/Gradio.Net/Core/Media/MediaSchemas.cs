using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The JSON schemas a file-carrying component describes itself with.
/// </summary>
/// <remarks>
/// <para>
/// The Python version derives these from its pydantic models, and the documents are served verbatim
/// in the config's <c>api_info</c> and in the API documentation, so a client reads them as the
/// contract for what an endpoint takes and returns. They are written out here rather than derived
/// because there is no pydantic in this framework: the model is <see cref="FileData"/>, and this is
/// the one place that says how it is spelled in a schema.
/// </para>
/// <para>
/// The shape is pydantic's, down to the <c>anyOf</c> spelling of an optional field and the
/// <c>$defs</c> reference to the metadata marker: a client that generates a call from this document
/// gets the same call from either implementation.
/// </para>
/// </remarks>
internal static class MediaSchemas
{
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

    /// <summary>The schema of one file, as <c>File</c> describes it.</summary>
    /// <returns>A new document.</returns>
    internal static JsonObject FileSchema() => new()
    {
        ["$defs"] = new JsonObject
        {
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
        },
        ["properties"] = FileProperties(),
        ["required"] = new JsonArray("path"),
        ["title"] = "FileData",
        ["type"] = "object",
        ["additional_description"] = FileDataDescription,
    };

    /// <summary>The schema of one file, as the upload button describes it.</summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// The same schema as <see cref="FileSchema"/> with the docstring in the key pydantic puts it
    /// in. The Python version's component base moves the model's <c>description</c> to
    /// <c>additional_description</c>; the upload button overrides the method and returns the
    /// model's own schema, so its document says <c>description</c> and has no
    /// <c>additional_description</c> at all. Two shapes because the Python version has two, and a
    /// client that reads the document sees the difference.
    /// </remarks>
    internal static JsonObject FileSchemaRaw()
    {
        var schema = FileSchema();
        schema.Remove("additional_description");
        schema["description"] = FileDataDescription;

        return schema;
    }

    /// <summary>The schema of a list of files, as the upload button describes it.</summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// The upload button's multiple-file form: the model's own schema, which has no
    /// <c>additional_description</c> key rather than one that is null.
    /// </remarks>
    internal static JsonObject ListFilesSchemaRaw()
    {
        var schema = ListFilesSchema();
        schema.Remove("additional_description");

        return schema;
    }

    /// <summary>The schema of a list of files, as a multiple-file <c>File</c> describes it.</summary>
    /// <returns>A new document.</returns>
    internal static JsonObject ListFilesSchema() => new()
    {
        ["$defs"] = new JsonObject
        {
            ["FileData"] = new JsonObject
            {
                ["description"] = FileDataDescription,
                ["properties"] = FileProperties(),
                ["required"] = new JsonArray("path"),
                ["title"] = "FileData",
                ["type"] = "object",
            },
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
        },
        ["items"] = new JsonObject { ["$ref"] = "#/$defs/FileData" },
        ["title"] = "ListFiles",
        ["type"] = "array",
        ["additional_description"] = null,
    };

    /// <summary>
    /// The schema of a multimodal textbox's value: a text and the files sent with it.
    /// </summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// The Python version derives this from the <c>MultimodalData</c> model, whose one required
    /// field is the text — a client may send the text alone, and the files it did not send are
    /// none. The <c>additional_description</c> key is there because the Python version writes the
    /// model's (absent) docstring into it rather than leaving it out, and a client generating a
    /// call from the document reads the same document either way.
    /// </remarks>
    internal static JsonObject MultimodalSchema() => new()
    {
        ["$defs"] = new JsonObject
        {
            ["FileData"] = new JsonObject
            {
                ["description"] = FileDataDescription,
                ["properties"] = FileProperties(),
                ["required"] = new JsonArray("path"),
                ["title"] = "FileData",
                ["type"] = "object",
            },
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
        },
        ["properties"] = new JsonObject
        {
            ["text"] = new JsonObject
            {
                ["title"] = "Text",
                ["type"] = "string",
            },
            ["files"] = new JsonObject
            {
                ["items"] = new JsonObject { ["$ref"] = "#/$defs/FileData" },
                ["title"] = "Files",
                ["type"] = "array",
            },
        },
        ["required"] = new JsonArray("text"),
        ["title"] = "MultimodalData",
        ["type"] = "object",
        ["additional_description"] = null,
    };

    /// <summary>The schema of one image, as <c>Image</c> describes it.</summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// An image is a file whose every field is optional and described, which is a different model
    /// from <see cref="FileData"/> rather than the same one with a different name — the metadata is a
    /// plain object here rather than a reference to the marker's definition.
    /// </remarks>
    internal static JsonObject ImageSchema() => new()
    {
        ["properties"] = ImageProperties(),
        ["title"] = "ImageData",
        ["type"] = "object",
        ["additional_description"] = "A local filepath or publicly available URL.",
    };

    /// <summary>The schema of an image editor's value: a background, its layers, and the composite.</summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// The Python version's <c>EditorData</c>. The <c>id</c> field is the editor's handle on a blob
    /// it has uploaded out of band — the browser sends large images to <c>accept_blobs</c> rather
    /// than inline — and it is absent from every value the component produces.
    /// </remarks>
    internal static JsonObject EditorSchema() => new()
    {
        ["$defs"] = new JsonObject
        {
            ["FileData"] = FileDefinition(),
            ["FileDataMeta"] = FileMetaDefinition(),
        },
        ["properties"] = new JsonObject
        {
            ["background"] = Nullable(new JsonObject { ["$ref"] = "#/$defs/FileData" }),
            ["layers"] = new JsonObject
            {
                ["default"] = new JsonArray(),
                ["items"] = new JsonObject { ["$ref"] = "#/$defs/FileData" },
                ["title"] = "Layers",
                ["type"] = "array",
            },
            ["composite"] = Nullable(new JsonObject { ["$ref"] = "#/$defs/FileData" }),
            ["id"] = Optional("string", "Id"),
        },
        ["title"] = "EditorData",
        ["type"] = "object",
        ["additional_description"] = null,
    };

    /// <summary>The schema of an image slider's value: exactly two images, or none.</summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// The Python version's <c>SliderData</c>, which is a root model over the pair: the document is
    /// the pair's schema rather than an object with a field holding it, which is why it has no
    /// <c>type</c> of its own.
    /// </remarks>
    internal static JsonObject SliderSchema() => new()
    {
        ["$defs"] = new JsonObject
        {
            ["ImageData"] = new JsonObject
            {
                ["description"] = "A local filepath or publicly available URL.",
                ["properties"] = ImageProperties(),
                ["title"] = "ImageData",
                ["type"] = "object",
            },
        },
        ["anyOf"] = new JsonArray(
            new JsonObject
            {
                ["maxItems"] = 2,
                ["minItems"] = 2,
                ["prefixItems"] = new JsonArray(
                    Nullable(new JsonObject { ["$ref"] = "#/$defs/ImageData" }, withDefault: false),
                    Nullable(new JsonObject { ["$ref"] = "#/$defs/ImageData" }, withDefault: false)),
                ["type"] = "array",
            },
            new JsonObject { ["type"] = "null" }),
        ["title"] = "SliderData",
        ["additional_description"] = null,
    };

    /// <summary>The schema of a gallery's value: a list of captioned images and videos.</summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// The Python version's <c>GalleryData</c>. An entry is either an image or a video, each with
    /// its own model and its own field name, which is what tells a client which of the two it is
    /// holding.
    /// </remarks>
    internal static JsonObject GallerySchema() => new()
    {
        ["$defs"] = new JsonObject
        {
            ["FileData"] = FileDefinition(),
            ["FileDataMeta"] = FileMetaDefinition(),
            ["GalleryImage"] = new JsonObject
            {
                ["properties"] = new JsonObject
                {
                    ["image"] = new JsonObject { ["$ref"] = "#/$defs/ImageData" },
                    ["caption"] = Optional("string", "Caption"),
                },
                ["required"] = new JsonArray("image"),
                ["title"] = "GalleryImage",
                ["type"] = "object",
            },
            ["GalleryVideo"] = new JsonObject
            {
                ["properties"] = new JsonObject
                {
                    ["video"] = new JsonObject { ["$ref"] = "#/$defs/FileData" },
                    ["caption"] = Optional("string", "Caption"),
                },
                ["required"] = new JsonArray("video"),
                ["title"] = "GalleryVideo",
                ["type"] = "object",
            },
            ["ImageData"] = new JsonObject
            {
                ["description"] = "A local filepath or publicly available URL.",
                ["properties"] = ImageProperties(),
                ["title"] = "ImageData",
                ["type"] = "object",
            },
        },
        ["items"] = new JsonObject
        {
            ["anyOf"] = new JsonArray(
                new JsonObject { ["$ref"] = "#/$defs/GalleryImage" },
                new JsonObject { ["$ref"] = "#/$defs/GalleryVideo" }),
        },
        ["title"] = "GalleryData",
        ["type"] = "array",
        ["additional_description"] = null,
    };

    /// <summary>The schema of an annotated image: a base image and its labelled masks.</summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// The Python version's <c>AnnotatedImageData</c>. Each annotation names a mask image and the
    /// label it is drawn in the legend as; the mask is a file rather than a set of coordinates
    /// because the component rasterizes whatever the application gave it before the value leaves
    /// the server.
    /// </remarks>
    internal static JsonObject AnnotatedImageSchema() => new()
    {
        ["$defs"] = new JsonObject
        {
            ["Annotation"] = new JsonObject
            {
                ["properties"] = new JsonObject
                {
                    ["image"] = new JsonObject { ["$ref"] = "#/$defs/FileData" },
                    ["label"] = new JsonObject
                    {
                        ["title"] = "Label",
                        ["type"] = "string",
                    },
                },
                ["required"] = new JsonArray("image", "label"),
                ["title"] = "Annotation",
                ["type"] = "object",
            },
            ["FileData"] = FileDefinition(),
            ["FileDataMeta"] = FileMetaDefinition(),
        },
        ["properties"] = new JsonObject
        {
            ["image"] = new JsonObject { ["$ref"] = "#/$defs/FileData" },
            ["annotations"] = new JsonObject
            {
                ["items"] = new JsonObject { ["$ref"] = "#/$defs/Annotation" },
                ["title"] = "Annotations",
                ["type"] = "array",
            },
        },
        ["required"] = new JsonArray("image", "annotations"),
        ["title"] = "AnnotatedImageData",
        ["type"] = "object",
        ["additional_description"] = null,
    };

    /// <summary>The schema of a file explorer's value: a list of paths, each split into segments.</summary>
    /// <returns>A new document.</returns>
    /// <remarks>
    /// The Python version's <c>FileExplorerData</c>, which is a list of lists of strings: the outer
    /// list is what is selected and the inner one is a path, one segment per entry. Splitting it
    /// that way is what lets the frontend show a selection as a tree of folders without the server
    /// having to say which part of a path is a folder.
    /// </remarks>
    internal static JsonObject FileExplorerSchema() => new()
    {
        ["items"] = new JsonObject
        {
            ["items"] = new JsonObject { ["type"] = "string" },
            ["type"] = "array",
        },
        ["title"] = "FileExplorerData",
        ["type"] = "array",
        ["additional_description"] = null,
    };

    /// <summary>The properties of a file, as every schema that names one spells them.</summary>
    private static JsonObject FileProperties() => new()
    {
        ["path"] = new JsonObject
        {
            ["title"] = "Path",
            ["type"] = "string",
        },
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
    };

    /// <summary>The properties of an image, as every schema that names one spells them.</summary>
    private static JsonObject ImageProperties() => new()
    {
        ["path"] = Optional("string", "Path", "Path to a local file"),
        ["url"] = Optional("string", "Url", "Publicly available url or base64 encoded image"),
        ["size"] = Optional("integer", "Size", "Size of image in bytes"),
        ["orig_name"] = Optional("string", "Orig Name", "Original filename"),
        ["mime_type"] = Optional("string", "Mime Type", "mime type of image"),
        ["is_stream"] = new JsonObject
        {
            ["default"] = false,
            ["description"] = "Can always be set to False",
            ["title"] = "Is Stream",
            ["type"] = "boolean",
        },
        ["meta"] = new JsonObject
        {
            ["additionalProperties"] = true,
            ["default"] = new JsonObject { ["_type"] = FileData.FileMetaType },
            ["title"] = "Meta",
            ["type"] = "object",
        },
    };

    /// <summary>The definition of a file, as a schema that refers to one spells it.</summary>
    private static JsonObject FileDefinition() => new()
    {
        ["description"] = FileDataDescription,
        ["properties"] = FileProperties(),
        ["required"] = new JsonArray("path"),
        ["title"] = "FileData",
        ["type"] = "object",
    };

    /// <summary>The definition of a file's metadata marker.</summary>
    private static JsonObject FileMetaDefinition() => new()
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
    };

    /// <summary>A field that is either the given definition or null.</summary>
    /// <param name="definition">The definition.</param>
    /// <param name="withDefault">
    /// Whether to write the null default. A field of a model carries one, because pydantic gives an
    /// optional field a default; an entry of a tuple does not, because a tuple entry is not a field
    /// and has nothing to default to.
    /// </param>
    private static JsonObject Nullable(JsonObject definition, bool withDefault = true)
    {
        var field = new JsonObject
        {
            ["anyOf"] = new JsonArray(
                definition,
                new JsonObject { ["type"] = "null" }),
        };

        if (withDefault)
        {
            field["default"] = null;
        }

        return field;
    }

    /// <summary>A field that may be null, as pydantic spells one.</summary>
    /// <param name="type">The JSON type of the value when it is there.</param>
    /// <param name="title">The field's title.</param>
    /// <param name="description">The field's description, when it has one.</param>
    private static JsonObject Optional(string type, string title, string? description = null)
    {
        var property = new JsonObject
        {
            ["anyOf"] = new JsonArray(
                new JsonObject { ["type"] = type },
                new JsonObject { ["type"] = "null" }),
            ["default"] = null,
        };

        if (description is not null)
        {
            property["description"] = description;
        }

        property["title"] = title;

        return property;
    }
}
