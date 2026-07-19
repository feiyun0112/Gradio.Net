using Gradio.Net.Data;
using Gradio.Net.Events;

namespace Gradio.Net.Components;

[Event("upload")]
[Event("clear")]
[Event("change")]
[Event("select")]
public class GradioFile : FormComponent
{
    public List<string> FileTypes { get; set; }
    public int FileCount { get; set; }
    public string Type { get; set; }

    public GradioFile(
        object value = null,
        List<string> fileTypes = null,
        int fileCount = 1,
        string type = "filepath",
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null)
    {
        Value = value;
        Label = label;
        ShowLabel = showLabel ?? true;
        Container = container;
        Scale = scale;
        MinWidth = minWidth;
        Interactive = interactive;
        FileTypes = fileTypes;
        FileCount = fileCount;
        Type = type;
    }

    public override string GetBlockName() => "file";

    public override Dictionary<string, object> GetConfig(Type cls = null)
    {
        var config = base.GetConfig(cls);
        config["file_types"] = FileTypes;
        config["file_count"] = FileCount;
        config["type"] = Type;
        return config;
    }

    public override object Preprocess(object payload)
    {
        if (payload == null) return null;

        if (payload is FileData fileData)
            return Type == "filepath" ? fileData.Path : fileData;

        if (payload is List<FileData> fileList)
        {
            if (Type == "filepath")
            {
                var paths = new List<string>();
                foreach (var file in fileList)
                    paths.Add(file.Path);
                return FileCount == 1 ? paths[0] : paths;
            }
            return FileCount == 1 ? fileList[0] : fileList;
        }

        return payload;
    }

    public override object Postprocess(object value)
    {
        if (value == null) return null;

        if (value is string filePath)
            return new FileData { Path = filePath, Url = $"/file={filePath}" };

        if (value is List<string> filePaths)
        {
            var fileDataList = new List<FileData>();
            foreach (var path in filePaths)
                fileDataList.Add(new FileData { Path = path, Url = $"/file={path}" });
            return fileDataList;
        }

        return value;
    }
}
