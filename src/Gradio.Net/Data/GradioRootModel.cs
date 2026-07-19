using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Gradio.Net.Data;

public class GradioRootModel<TRoot> : GradioDataModel
{
    public TRoot Root { get; set; }

    public override GradioDataModel CopyToDir(string dir)
    {
        // Create unique_copy function to handle FileData objects
        Func<object, object> uniqueCopy = obj =>
        {
            if (obj is Dictionary<string, object> dict)
            {
                // Create FileData from dict
                var fileData = new FileData
                {
                    Path = dict.TryGetValue("path", out var pathVal) ? pathVal.ToString() : null,
                    Url = dict.TryGetValue("url", out var urlVal) ? urlVal.ToString() : null,
                    Size = dict.TryGetValue("size", out var sizeVal) ? Convert.ToInt32(sizeVal) : null,
                    OrigName = dict.TryGetValue("orig_name", out var origNameVal) ? origNameVal.ToString() : null,
                    MimeType = dict.TryGetValue("mime_type", out var mimeTypeVal) ? mimeTypeVal.ToString() : null,
                    IsStream = dict.TryGetValue("is_stream", out var isStreamVal) && Convert.ToBoolean(isStreamVal),
                    Meta = dict.TryGetValue("meta", out var metaVal) && metaVal is Dictionary<string, object> metaDict
                        ? new FileDataMeta { Type = metaDict.TryGetValue("_type", out var typeVal) ? typeVal.ToString() : null }
                        : new FileDataMeta()
                };

                // Copy to unique directory
                string uniqueDir = System.IO.Path.Combine(dir, TraverseHelper.GenerateRandomHex(10));
                var copiedFileData = fileData.CopyToDir(uniqueDir);

                // Return as dict
                return new Dictionary<string, object>
                {
                    { "path", copiedFileData.Path },
                    { "url", copiedFileData.Url },
                    { "size", copiedFileData.Size },
                    { "orig_name", copiedFileData.OrigName },
                    { "mime_type", copiedFileData.MimeType },
                    { "is_stream", copiedFileData.IsStream },
                    { "meta", new Dictionary<string, object> { { "_type", "gradio.FileData" } } }
                };
            }
            return obj;
        };

        // Traverse the root value and apply uniqueCopy to FileData objects
        var traversedRoot = TraverseHelper.Traverse(Root, uniqueCopy, FileData.IsFileData);

        // Create new instance with traversed root
        var newModel = new GradioRootModel<TRoot>();
        newModel.Root = (TRoot)traversedRoot;

        return newModel;
    }

    public override GradioDataModel FromJson(object data)
    {
        var newModel = new GradioRootModel<TRoot>();
        newModel.Root = (TRoot)data;
        return newModel;
    }

    public TRoot ModelDump()
    {
        return Root;
    }
}

