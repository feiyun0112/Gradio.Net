using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A browser over a directory of the server's file system, where the user picks files
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. Two of them are a path rather than a value: the
/// directory the component browses is made absolute when it is set, because a relative one would
/// mean whatever the process's working directory happened to be when the request arrived, and the
/// config carries the absolute path the Python version carries.
/// </para>
/// <para>
/// The component is the first whose behaviour is a function the frontend calls back into: the
/// listing a user opens a folder with is <see cref="List"/>, declared as a server function, which
/// the config names in <c>server_fns</c> and the browser reaches through
/// <c>/gradio_api/component_server</c>.
/// </para>
/// </remarks>
[GradioBlock("fileexplorer")]
[HandlerValue("object?", "change", "input", "select")]
public sealed partial class FileExplorer : Component
{
    internal FileExplorer()
    {
    }

    /// <summary>The pattern that matches everything, which is what the component shows by default.</summary>
    public const string Everything = "**/*";

    private string _rootDirectory = Path.GetFullPath(".");
    private string _fileCount = "multiple";
    private object? _value;

    /// <summary>The glob the component shows files by.</summary>
    public string Glob { get; set; } = Everything;

    /// <summary>The file or files selected when the component loads.</summary>
    /// <value>A path, a list of paths, or <see langword="null"/> for nothing selected.</value>
    public object? Value
    {
        get => Postprocess(_value);
        set => _value = value;
    }

    /// <summary>Whether the user may select one file or several.</summary>
    /// <value><c>single</c> or <c>multiple</c>.</value>
    public string FileCount
    {
        get => _fileCount;
        set
        {
            if (value is not ("single" or "multiple"))
            {
                throw new ArgumentException(
                    $"A file explorer's file_count must be single or multiple; got '{value}'.",
                    nameof(value));
            }

            _fileCount = value;
        }
    }

    /// <summary>The directory the component browses, as an absolute path.</summary>
    /// <exception cref="ArgumentException">The path is not a directory that exists.</exception>
    public string RootDir
    {
        get => _rootDirectory;
        set
        {
            var absolute = Path.GetFullPath(value);

            if (!Directory.Exists(absolute))
            {
                throw new ArgumentException(
                    System.IO.File.Exists(absolute)
                        ? $"The specified root_dir is not a directory: {value}"
                        : $"The specified root_dir does not exist: {value}",
                    nameof(value));
            }

            _rootDirectory = absolute;
        }
    }

    /// <summary>The pattern that hides files from the listing, or <see langword="null"/> for none.</summary>
    public string? IgnoreGlob { get; set; }

    /// <summary>The label drawn above the component.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the component next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the component wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>The height of the component, in pixels or as a CSS length.</summary>
    public object? Height { get; set; }

    /// <summary>The tallest the component is drawn, in pixels or as a CSS length.</summary>
    public object? MaxHeight { get; set; } = 500;

    /// <summary>The shortest the component is drawn, in pixels or as a CSS length.</summary>
    public object? MinHeight { get; set; }

    /// <summary>The buttons drawn in the component's corner.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <inheritdoc />
    public override JsonObject ApiInfo => MediaSchemas.FileExplorerSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonArray(new JsonArray("gradio", "app.py"));

    /// <inheritdoc />
    protected internal override IReadOnlyList<string> ServerFunctionNames => ["ls"];

    /// <summary>Calls one of this component's server functions.</summary>
    /// <param name="request">The function's name, and the folder the browser asked about.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The listing, or <see langword="null"/> for a function this component does not have.</returns>
    /// <remarks>
    /// The one function is the listing itself: the browser opens a folder by asking for its
    /// contents, and the argument is the folder as the segments the component's value uses.
    /// </remarks>
    protected internal override ValueTask<JsonNode?> InvokeServerFunctionAsync(
        ComponentServerCall request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.FunctionName != "ls")
        {
            return base.InvokeServerFunctionAsync(request, cancellationToken);
        }

        var segments = request.Data is JsonArray folder
            ? folder.Select(JsonValues.Text).OfType<string>().ToList()
            : null;

        return ValueTask.FromResult<JsonNode?>(List(segments));
    }

    /// <summary>
    /// The value a handler reads from this component.
    /// </summary>
    /// <param name="value">What the client sent: a list of entries, each a path split into segments.</param>
    /// <returns>A path for a single-file component, a list of paths for a multiple one, or <see langword="null"/>.</returns>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is not JsonArray selection)
        {
            return null;
        }

        var paths = new List<string>();
        foreach (var entry in selection)
        {
            if (entry is JsonArray segments)
            {
                paths.Add(Join([.. segments.Select(JsonValues.Text).OfType<string>()]));
            }
        }

        if (FileCount == "single")
        {
            return paths.Count switch
            {
                0 => null,
                1 => paths[0],
                _ => throw new InvalidOperationException(
                    $"Expected only one file, but {paths.Count} were selected."),
            };
        }

        return paths;
    }

    /// <summary>
    /// The value this component receives from what a handler returned.
    /// </summary>
    /// <param name="value">A path or a list of paths.</param>
    /// <returns>The selection as the protocol carries it, or <see langword="null"/> for none.</returns>
    public override JsonNode? Postprocess(object? value)
    {
        if (value is null)
        {
            return null;
        }

        string[] files = value switch
        {
            string path => [path],
            JsonNode node => [.. (JsonValues.ToObject(node) as object[] ?? []).OfType<string>()],
            System.Collections.IEnumerable items => [.. items.Cast<object?>().OfType<string>()],
            _ => throw new NotSupportedException(
                $"A file explorer takes a path or a list of paths; it was handed a {value.GetType()}."),
        };

        var root = new JsonArray();
        foreach (var file in files)
        {
            var segments = new JsonArray();
            foreach (var segment in Strip(file).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
            {
                segments.Add(segment);
            }

            root.Add(segments);
        }

        return root;
    }

    /// <summary>Lists one directory, which is what the browser asks for when a folder is opened.</summary>
    /// <param name="subdirectory">The folder, as the segments the browser holds.</param>
    /// <returns>One entry per item: its name, whether it is a file or a folder, and whether it is selectable.</returns>
    /// <remarks>
    /// Folders come first, each list is sorted, and an item is shown when it matches
    /// <see cref="Glob"/> and not <see cref="IgnoreGlob"/> — the Python version's rules, including
    /// the fact that a folder is listed whether or not it matches, because a folder the user cannot
    /// open is a folder they cannot reach what they want through.
    /// </remarks>
    public JsonArray List(IReadOnlyList<string>? subdirectory = null)
    {
        var directory = Join(subdirectory ?? []);
        string[] items;

        try
        {
            items = [.. Directory.GetFileSystemEntries(directory).Order(StringComparer.Ordinal)];
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            return [];
        }

        var files = new JsonArray();
        var folders = new JsonArray();

        foreach (var item in items)
        {
            bool isFile;
            try
            {
                isFile = !Directory.Exists(item);
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException)
            {
                continue;
            }

            var valid = PythonGlob.IsMatch(item, Glob);

            if (isFile && !valid)
            {
                continue;
            }

            if (IgnoreGlob is { Length: > 0 } ignore && PythonGlob.IsMatch(item, ignore))
            {
                continue;
            }

            var entry = new JsonObject
            {
                ["name"] = Path.GetFileName(item),
                ["type"] = isFile ? "file" : "folder",
                ["valid"] = valid,
            };

            (isFile ? files : folders).Add(entry);
        }

        // Folders first, then files, which is the order the Python version answers in: a browser lists the
        // things a user can go into before the things they can take.
        var listing = new JsonArray();

        foreach (var entry in folders)
        {
            listing.Add(entry?.DeepClone());
        }

        foreach (var entry in files)
        {
            listing.Add(entry?.DeepClone());
        }

        return listing;
    }

    /// <summary>A path with the browsed directory's prefix removed.</summary>
    private string Strip(string path) =>
        path.StartsWith(RootDir, StringComparison.Ordinal) ? path[(RootDir.Length + 1)..] : path;

    /// <summary>A folder inside the browsed directory, from the segments the browser holds.</summary>
    /// <exception cref="InvalidOperationException">The segments climb out of the browsed directory.</exception>
    private string Join(IReadOnlyList<string> segments)
    {
        if (segments.Count == 0)
        {
            return RootDir;
        }

        var combined = string.Join(Path.DirectorySeparatorChar, segments);
        if (Path.DirectorySeparatorChar == '\\')
        {
            combined = combined.Replace('/', '\\');
        }

        var full = Path.GetFullPath(Path.Combine(RootDir, combined));
        var root = RootDir.EndsWith(Path.DirectorySeparatorChar) ? RootDir : RootDir + Path.DirectorySeparatorChar;

        return full.StartsWith(root, StringComparison.Ordinal) || string.Equals(full, RootDir, StringComparison.Ordinal)
            ? full
            : throw new InvalidOperationException("Attempted to navigate outside of the root directory.");
    }
}

/// <summary>
/// Python's <c>fnmatch</c>, which is the pattern language a component's globs are written in.
/// </summary>
/// <remarks>
/// <para>
/// Not the same as .NET's file-system globs: in <c>fnmatch</c> a <c>*</c> matches everything,
/// separators included, so <c>**/*</c> is a pattern that happens to match every path rather than a
/// special spelling for "recursively". The component's default glob depends on that difference, and
/// a matcher that treated <c>*</c> as "one segment" would hide every file one folder down.
/// </para>
/// <para>
/// On Windows the comparison is case-insensitive and a forward slash is the same character as a
/// backslash, which is what Python's <c>os.path.normcase</c> does before it matches.
/// </para>
/// </remarks>
internal static class PythonGlob
{
    /// <summary>Whether a path matches a pattern, as Python's <c>fnmatch</c> decides it.</summary>
    /// <param name="path">The path.</param>
    /// <param name="pattern">The pattern.</param>
    /// <returns>Whether it matches.</returns>
    internal static bool IsMatch(string path, string pattern)
    {
        var normalizedPath = Normalize(path);
        var normalizedPattern = Normalize(pattern);

        return Regex.IsMatch(
            normalizedPath,
            "^" + Translate(normalizedPattern) + "$",
            RegexOptions.Singleline | RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(1));
    }

    /// <summary>A path or pattern as the comparison sees it.</summary>
    private static string Normalize(string value) =>
        OperatingSystem.IsWindows() ? value.Replace('/', '\\').ToLowerInvariant() : value;

    /// <summary>A pattern as a regular expression.</summary>
    private static string Translate(string pattern)
    {
        var expression = new StringBuilder();

        for (var index = 0; index < pattern.Length; index++)
        {
            var character = pattern[index];

            switch (character)
            {
                case '*':
                    expression.Append(".*");
                    break;

                case '?':
                    expression.Append('.');
                    break;

                case '[':
                    var close = pattern.IndexOf(']', index + 1);
                    if (close < 0)
                    {
                        expression.Append("\\[");
                        break;
                    }

                    var set = pattern[(index + 1)..close];
                    expression.Append('[');
                    if (set.StartsWith('!'))
                    {
                        expression.Append('^').Append(Regex.Escape(set[1..]).Replace("\\-", "-", StringComparison.Ordinal));
                    }
                    else
                    {
                        expression.Append(Regex.Escape(set).Replace("\\-", "-", StringComparison.Ordinal));
                    }

                    expression.Append(']');
                    index = close;
                    break;

                default:
                    expression.Append(Regex.Escape(character.ToString()));
                    break;
            }
        }

        return expression.ToString();
    }
}
