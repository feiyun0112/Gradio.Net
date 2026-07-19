using System.Collections;
using System.Text.RegularExpressions;
using Gradio.Net.Core.Exceptions;
using Gradio.Net.Data;

namespace Gradio.Net.Components;

[Events.Event("change")]
public class FileExplorer : Component
{
    public string Glob { get; set; }
    public string FileCount { get; set; }
    public DeveloperPath RootDir { get; set; }
    public string? IgnoreGlob { get; set; }
    public object? Height { get; set; }
    public object? MaxHeight { get; set; }
    public object? MinHeight { get; set; }

    public FileExplorer(
        string glob = "**/*",
        object? value = null,
        string fileCount = "multiple",
        object? rootDir = null,
        string? ignoreGlob = null,
        string? label = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        object? height = null,
        object? maxHeight = null,
        object? minHeight = null,
        bool? interactive = null,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null)
    {
        var root = rootDir?.ToString() ?? ".";
        var absRoot = Path.GetFullPath(root);

        if (!Directory.Exists(absRoot))
        {
            throw new Error($"The specified root_dir does not exist: {root}");
        }

        var validFileCount = new[] { "single", "multiple" };
        if (!validFileCount.Contains(fileCount))
        {
            throw new Error($"Invalid value for parameter `file_count`: {fileCount}. Please choose from one of: [{string.Join(", ", validFileCount)}]");
        }

        Glob = string.IsNullOrWhiteSpace(glob) ? "**/*" : glob;
        RootDir = absRoot;
        IgnoreGlob = ignoreGlob;
        FileCount = fileCount;
        Height = height;
        MaxHeight = maxHeight ?? 500;
        MinHeight = minHeight;

        Value = value;
        Label = label;
        ShowLabel = showLabel ?? ShowLabel;
        Container = container;
        Scale = scale;
        MinWidth = minWidth;
        Interactive = interactive;
        if (visible is bool vb) Visible = vb;
        ElemId = elemId;
        ElemClasses = elemClasses switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => ElemClasses
        };
        Key = key;
        PreservedByKey = preservedByKey switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => PreservedByKey
        };
    }

    public override string GetBlockName() => "fileexplorer";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["glob"] = Glob;
        config["file_count"] = FileCount;
        config["root_dir"] = (string)RootDir;
        config["ignore_glob"] = IgnoreGlob;
        config["height"] = Height;
        config["max_height"] = MaxHeight;
        config["min_height"] = MinHeight;
        return config;
    }

    public override object ExamplePayload()
    {
        return new List<List<string>> { new() { "gradio", "app.py" } };
    }

    public override object ExampleValue()
    {
        return Path.Combine("gradio", "app.py");
    }

    public override object? Preprocess(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        var model = payload as FileExplorerData ?? FromPayload(payload);
        if (model == null)
        {
            return null;
        }

        if (FileCount == "single")
        {
            if (model.Root.Count > 1)
            {
                throw new Error($"Expected only one file, but {model.Root.Count} were selected.");
            }

            if (model.Root.Count == 0)
            {
                return null;
            }

            return Path.GetFullPath(Path.Combine((string)RootDir, Path.Combine(model.Root[0].ToArray())));
        }

        var files = new List<string>();
        foreach (var file in model.Root)
        {
            var combined = Path.Combine((string)RootDir, Path.Combine(file.ToArray()));
            files.Add(Path.GetFullPath(combined));
        }

        return files;
    }

    public string StripRoot(string path)
    {
        var root = (string)RootDir;
        if (path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            var trimmed = path.Substring(root.Length);
            return trimmed.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        return path;
    }

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        var files = new List<string>();
        if (value is string s)
        {
            files.Add(s);
        }
        else if (value is IEnumerable enumerable && value is not string)
        {
            foreach (var item in enumerable)
            {
                var p = item?.ToString();
                if (!string.IsNullOrWhiteSpace(p))
                {
                    files.Add(p);
                }
            }
        }
        else
        {
            var p = value.ToString();
            if (!string.IsNullOrWhiteSpace(p))
            {
                files.Add(p);
            }
        }

        var root = new List<List<string>>();
        foreach (var file in files)
        {
            var stripped = StripRoot(file);
            var parts = stripped
                .Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            root.Add(parts);
        }

        return new FileExplorerData { Root = root };
    }

    public List<Dictionary<string, object>> Ls(List<string>? subdirectory = null)
    {
        subdirectory ??= new List<string>();

        var fullSubdirPath = SafeJoin(subdirectory);
        var result = new List<Dictionary<string, object>>();

        string[] subdirItems;
        try
        {
            subdirItems = Directory.GetFileSystemEntries(fullSubdirPath)
                .Select(Path.GetFileName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToArray()!;
        }
        catch (Exception ex) when (ex is DirectoryNotFoundException || ex is UnauthorizedAccessException)
        {
            return result;
        }

        var folders = new List<Dictionary<string, object>>();
        var files = new List<Dictionary<string, object>>();

        foreach (var item in subdirItems)
        {
            var fullPath = Path.Combine(fullSubdirPath, item);
            bool isFile;
            try
            {
                isFile = !Directory.Exists(fullPath);
            }
            catch
            {
                continue;
            }

            var validByGlob = MatchesGlob(fullPath, Glob);
            if (isFile && !validByGlob)
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(IgnoreGlob) && MatchesGlob(fullPath, IgnoreGlob!))
            {
                continue;
            }

            var entry = new Dictionary<string, object>
            {
                ["name"] = item,
                ["type"] = isFile ? "file" : "folder",
                ["valid"] = validByGlob
            };

            if (isFile)
            {
                files.Add(entry);
            }
            else
            {
                folders.Add(entry);
            }
        }

        folders.AddRange(files);
        return folders;
    }

    public string SafeJoin(List<string> folders)
    {
        if (folders == null || folders.Count == 0)
        {
            return RootDir;
        }

        UserProvidedPath combined = Path.Combine(folders.ToArray());
        var combinedPath = (string)combined;

        if (OperatingSystem.IsWindows())
        {
            combinedPath = combinedPath.Replace('\\', '/');
        }

        return global::Gradio.Net.Utils.Utils.SafeJoin(RootDir, combinedPath);
    }

    private static FileExplorerData? FromPayload(object payload)
    {
        if (payload is Dictionary<string, object> dict && dict.TryGetValue("root", out var rootObj))
        {
            return new FileExplorerData { Root = ParseRoot(rootObj) };
        }

        if (payload is IDictionary<string, object> idict && idict.TryGetValue("root", out var irootObj))
        {
            return new FileExplorerData { Root = ParseRoot(irootObj) };
        }

        return null;
    }

    private static List<List<string>> ParseRoot(object? rootObj)
    {
        var root = new List<List<string>>();
        if (rootObj is not IEnumerable outer)
        {
            return root;
        }

        foreach (var item in outer)
        {
            if (item is IEnumerable inner && item is not string)
            {
                var parts = new List<string>();
                foreach (var p in inner)
                {
                    if (p != null)
                    {
                        parts.Add(p.ToString() ?? string.Empty);
                    }
                }
                root.Add(parts);
            }
        }

        return root;
    }

    private static bool MatchesGlob(string fullPath, string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        var normalizedPath = fullPath.Replace('\\', '/');
        var normalizedPattern = pattern.Replace('\\', '/');

        var regex = "^" + Regex.Escape(normalizedPattern)
            .Replace("\\*\\*", ".*")
            .Replace("\\*", "[^/]*")
            .Replace("\\?", ".") + "$";

        return Regex.IsMatch(normalizedPath, regex, RegexOptions.IgnoreCase);
    }
}
