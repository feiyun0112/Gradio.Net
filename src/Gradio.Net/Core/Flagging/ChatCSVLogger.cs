namespace Gradio.Net.Core.Flagging;

using System.Text;
using Gradio.Net.Events;

public class ChatCSVLogger
{
    protected string FlaggingDir { get; set; } = string.Empty;

    public ChatCSVLogger()
    {
    }

    public void Setup(string flaggingDir)
    {
        FlaggingDir = flaggingDir;
        Directory.CreateDirectory(flaggingDir);
    }

    public void Flag(LikeData likeData, List<object> messages)
    {
        var flaggingDir = FlaggingDir;
        var logFilePath = Path.Combine(flaggingDir, "log.csv");
        var isNew = !File.Exists(logFilePath);

        string feedback;
        if (likeData.Liked is bool likedValue)
        {
            if (likedValue)
            {
                feedback = "Like";
            }
            else
            {
                feedback = "Dislike";
            }
        }
        else
        {
            feedback = likeData.Liked?.ToString() ?? string.Empty;
        }

        var csvData = new List<string>
        {
            System.Text.Json.JsonSerializer.Serialize(messages),
            likeData.Index?.ToString() ?? string.Empty,
            feedback,
            System.DateTime.Now.ToString()
        };

        using (var writer = new StreamWriter(logFilePath, true, Encoding.UTF8))
        {
            if (isNew)
            {
                writer.WriteLine("conversation,index,value,flag,timestamp");
            }
            var sanitizedList = Gradio.Net.Utils.Utils.SanitizeListForCsv(csvData.Cast<object>().ToList());
            var csvLine = string.Join(",", sanitizedList.Select(v => v.ToString()));
            writer.WriteLine(csvLine);
        }
    }
}
