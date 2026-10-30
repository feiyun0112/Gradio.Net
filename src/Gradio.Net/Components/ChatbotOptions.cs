using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// One of the delimiters a chat renders LaTeX between.
/// </summary>
/// <remarks>
/// The Python version's <c>latex_delimiters</c> is a list of these dictionaries, and the frontend
/// hands them to KaTeX: text between <see cref="Left"/> and <see cref="Right"/> is a formula, and
/// <see cref="Display"/> says whether it is set on a line of its own. An empty list turns LaTeX
/// rendering off, which is why the component's default is one delimiter rather than none.
/// </remarks>
public sealed class LatexDelimiter : IConfigValue
{
    /// <summary>Creates a delimiter, defaulting to the one the Python version starts
    /// with.</summary>
    internal LatexDelimiter()
    {
    }

    /// <summary>Creates a delimiter.</summary>
    /// <param name="left">The text a formula opens with.</param>
    /// <param name="right">The text a formula closes with.</param>
    /// <param name="display">Whether the formula is set on a line of its own.</param>
    public LatexDelimiter(string left, string right, bool display = true)
    {
        Left = left;
        Right = right;
        Display = display;
    }

    /// <summary>The text a formula opens with.</summary>
    public string Left { get; set; } = "$$";

    /// <summary>The text a formula closes with.</summary>
    public string Right { get; set; } = "$$";

    /// <summary>Whether the formula is set on a line of its own.</summary>
    public bool Display { get; set; } = true;

    /// <inheritdoc />
    public JsonNode ToConfigValue() => new JsonObject
    {
        ["left"] = Left,
        ["right"] = Right,
        ["display"] = Display,
    };
}

/// <summary>
/// A pair of tags whose content a chat lifts out of a message and shows as reasoning.
/// </summary>
/// <remarks>
/// A model that thinks out loud writes its thoughts between tags; with this declared, the component
/// moves each such span into a collapsible message of its own, so the answer is not buried in the
/// reasoning. The wire shape is a two-element array, which is the Python version's.
/// </remarks>
public sealed class ReasoningTag : IConfigValue
{
    /// <summary>Creates a pair of tags.</summary>
    /// <param name="open">The tag a thought opens with.</param>
    /// <param name="close">The tag a thought closes with.</param>
    /// <exception cref="ArgumentNullException">Either tag is <see langword="null"/>.</exception>
    public ReasoningTag(string open, string close)
    {
        ArgumentNullException.ThrowIfNull(open);
        ArgumentNullException.ThrowIfNull(close);

        Open = open;
        Close = close;
    }

    /// <summary>The tag a thought opens with.</summary>
    public string Open { get; }

    /// <summary>The tag a thought closes with.</summary>
    public string Close { get; }

    /// <inheritdoc />
    public JsonNode ToConfigValue() => new JsonArray(Open, Close);
}

/// <summary>
/// The two pictures a chat shows next to its messages: one for the user, one for the assistant.
/// </summary>
/// <remarks>
/// <para>
/// Either may be absent, which is what an application that leaves them alone gets: the config then
/// carries <c>[null, null]</c>, and the frontend draws no avatars.
/// </para>
/// <para>
/// A picture the application holds is copied into the application's cache when the config is built,
/// for the same reason a component's default value is: the browser fetches it from the file
/// endpoint, which serves the cache and not the working directory. A picture that names a location
/// is passed through as it is.
/// </para>
/// </remarks>
public sealed class AvatarImages : IConfigValue
{
    /// <summary>Creates the pair from the paths or addresses the application wrote.</summary>
    /// <param name="user">The user's picture, or <see langword="null"/> for none.</param>
    /// <param name="assistant">The assistant's picture, or <see langword="null"/> for none.</param>
    public AvatarImages(string? user = null, string? assistant = null)
    {
        User = user;
        Assistant = assistant;
    }

    private AvatarImages(FileData? user, FileData? assistant)
    {
        UserFile = user;
        AssistantFile = assistant;
    }

    /// <summary>The user's picture, as the application wrote it, or <see langword="null"/> for none.</summary>
    public string? User { get; }

    /// <summary>The assistant's picture, as the application wrote it, or <see langword="null"/> for none.</summary>
    public string? Assistant { get; }

    /// <summary>The user's picture as a served file, once it has been resolved.</summary>
    internal FileData? UserFile { get; }

    /// <summary>The assistant's picture as a served file, once it has been resolved.</summary>
    internal FileData? AssistantFile { get; }

    /// <summary>The pair with both pictures turned into served files.</summary>
    /// <param name="describe">How a path becomes a file the browser can fetch.</param>
    /// <returns>The resolved pair.</returns>
    internal AvatarImages Resolved(Func<string, FileData> describe)
    {
        ArgumentNullException.ThrowIfNull(describe);

        return new AvatarImages(
            User is null ? null : describe(User),
            Assistant is null ? null : describe(Assistant));
    }

    /// <inheritdoc />
    public JsonNode ToConfigValue() => new JsonArray(
        Entry(UserFile, User),
        Entry(AssistantFile, Assistant));

    /// <summary>One side of the pair: the resolved file, or the path treated as a location.</summary>
    private static JsonObject? Entry(FileData? file, string? path)
    {
        if (file is not null)
        {
            return file.ToJson();
        }

        return path is null ? null : MediaFiles.At(path).ToJson();
    }
}

/// <summary>
/// One of the examples a chat offers before the conversation starts.
/// </summary>
/// <remarks>
/// Clicking one fills the chat with <see cref="Text"/> and <see cref="Files"/>; what the user reads
/// in the example box is <see cref="DisplayText"/>, or the text itself when that is not set. The
/// icon and the files are copied into the application's cache when the config is built, so that the
/// browser can fetch them.
/// </remarks>
public sealed class ChatExample : IConfigValue
{
    /// <summary>The text the example puts in the chat.</summary>
    public string? Text { get; set; }

    /// <summary>The text shown in the example box, or <see langword="null"/> to show <see cref="Text"/>.</summary>
    public string? DisplayText { get; set; }

    /// <summary>The picture shown beside the example, or <see langword="null"/> for none.</summary>
    public string? Icon { get; set; }

    /// <summary>The files the example puts in the chat, or <see langword="null"/> for none.</summary>
    public IReadOnlyList<string>? Files { get; set; }

    /// <summary>The icon as a served file, once it has been resolved.</summary>
    internal FileData? IconFile { get; private init; }

    /// <summary>
    /// Whether the example is drawn with a badge rather than with a picture, because it has neither.
    /// </summary>
    /// <remarks>
    /// A multimodal example that carries no file is still drawn with something above its text, and
    /// the Python version's something is a file object whose media type is <c>text</c> — a marker
    /// rather than a file. It is a flag rather than a path because the object it produces names no
    /// file: a resolver handed an empty path would go looking for one.
    /// </remarks>
    internal bool TextBadge { get; private init; }

    /// <summary>The files as served files, once they have been resolved.</summary>
    internal IReadOnlyList<FileData>? DescribedFiles { get; private init; }

    /// <summary>The example with its icon and files turned into served files.</summary>
    /// <param name="describe">How a path becomes a file the browser can fetch.</param>
    /// <returns>The resolved example.</returns>
    internal ChatExample Resolved(Func<string, FileData> describe)
    {
        ArgumentNullException.ThrowIfNull(describe);

        return new ChatExample
        {
            Text = Text,
            DisplayText = DisplayText,
            Icon = Icon,
            Files = Files,
            TextBadge = TextBadge,
            IconFile = Icon is null ? null : describe(Icon),
            DescribedFiles = Files?.Select(describe).ToArray(),
        };
    }

    /// <summary>The same example, drawn with the badge a fileless multimodal sample gets.</summary>
    /// <returns>The example.</returns>
    internal ChatExample WithTextBadge() => new()
    {
        Text = Text,
        DisplayText = DisplayText,
        Icon = Icon,
        Files = Files,
        TextBadge = true,
    };

    /// <inheritdoc />
    public JsonNode ToConfigValue()
    {
        var example = new JsonObject();

        if (Icon is not null)
        {
            example["icon"] = IconFile?.ToJson() ?? MediaFiles.At(Icon).ToJson();
        }
        else if (TextBadge)
        {
            // The marker the Python version writes for a multimodal sample with no picture: a file object
            // that names no file, with the media type it uses to say "this one is text".
            example["icon"] = new JsonObject
            {
                ["path"] = string.Empty,
                ["url"] = null,
                ["orig_name"] = null,
                ["mime_type"] = "text",
                ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
            };
        }

        if (DisplayText is not null)
        {
            example["display_text"] = DisplayText;
        }

        if (Text is not null)
        {
            example["text"] = Text;
        }

        // An example whose files have not been resolved still says which files it offers: the
        // config is built from a resolved example, and a caller reading this value directly gets
        // the addresses rather than nothing.
        var files = new JsonArray();
        if (DescribedFiles is not null)
        {
            foreach (var file in DescribedFiles)
            {
                files.Add(file.ToJson());
            }
        }
        else if (Files is not null)
        {
            foreach (var file in Files)
            {
                files.Add(MediaFiles.At(file).ToJson());
            }
        }

        if (files.Count > 0)
        {
            example["files"] = files;
        }

        return example;
    }
}
