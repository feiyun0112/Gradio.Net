using Gradio.Net.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Gradio.Net;

public class File : Component, IHaveClearEvent, IHaveChangeEvent, IHaveSelectEvent, IHaveUploadEvent
{
    internal File() { }
    internal FileCount? FileCount { get; set; }
    internal IEnumerable<string> FileTypes { get; set; }

    internal FileType? Type { get; set; }
    internal decimal? Height { get; set; }

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
        { nameof(FileCount),  Enums.FileCount.Single},
         { nameof(Type), FileType.Filepath },

         { nameof(Container),true },
        {nameof(MinWidth),160 },
           { nameof(Visible), true },
        { nameof(Render), true },
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;

    public void CheckStreamable()
    {
        //todo
    }

    internal override object PreProcess(object data)
    {
        if (data == null)
        {
            return null;
        }

        string? str = data.ToString();

        if (str.StartsWith("["))
        {
            FileData[] fileDatas = JsonUtils.Deserialize<FileData[]>(str);
            return fileDatas.Select(p => p.Path).ToArray();
        }

        FileData fileData = JsonUtils.Deserialize<FileData>(str);
        return fileData.Path;
    }

    internal override object PostProcess(string rootUrl, object data)
    {
        if (data == null)
        {
            return null;
        }
        if (data is string[] arrFiles)
        {
            List<FileData> fileDataList = new List<FileData>();
            for (int i = 0; i < arrFiles.Length; i++)
            {
                fileDataList.Add(ConvertToFileData(rootUrl, arrFiles[i]));
            }

            return fileDataList;
        }
        return ConvertToFileData(rootUrl, data.ToString());
    }

    private static FileData ConvertToFileData(string rootUrl, string? str)
    {
        Context.DownloadableFiles.TryAdd(str, str);
        if (ClientUtils.IsUrl(str))
        {
            return new FileData { Path = null, Url = str };
        }

        FileInfo fileInfo = new FileInfo(str);
        return new FileData { Path = str, Url = $"{rootUrl}{GradioApp.API_PREFIX}/file={str}", Size = fileInfo.Length, OrigName = fileInfo.Name };
    }

    public static string Payload(object obj)
    {
        if (obj == null)
        {
            return null;
        }


        if (obj is string str)
        {
            return str;
        }

        throw new ArgumentException($"Payload Type expect string actual {obj.GetType()}");
    }
}
