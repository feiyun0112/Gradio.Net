namespace Gradio.Net.Core.Flagging;

using System.Text;
using Gradio.Net.Utils;

public class SimpleCSVLogger : FlaggingCallback
{
    protected IEnumerable<Components.Component> Components { get; set; } = Enumerable.Empty<Components.Component>();
    protected string FlaggingDir { get; set; } = string.Empty;

    public override void Setup(IEnumerable<Components.Component> components, string flaggingDir)
    {
        Components = components;
        FlaggingDir = flaggingDir;
        Directory.CreateDirectory(flaggingDir);
    }

    public override int Flag(List<object> flagData, string? flagOption = null, string? username = null)
    {
        var flaggingDir = FlaggingDir;
        var logFilePath = Path.Combine(flaggingDir, "log.csv");

        var csvData = new List<string>();

        foreach (var (component, sample) in Components.Zip(flagData, (c, d) => (c, d)))
        {
            var label = component.Label ?? string.Empty;
            var saveDir = Path.Combine(
                flaggingDir,
                StripInvalidFilenameCharacters(label)
            );
            Directory.CreateDirectory(saveDir);

            var flaggedData = component.Flag(sample, saveDir);
            // Convert Dictionary<string, object> to string for CSV
            var flaggedDataStr = System.Text.Json.JsonSerializer.Serialize(flaggedData);
            csvData.Add(flaggedDataStr);
        }

        // Ensure the file exists and write the data
        using (var writer = new StreamWriter(logFilePath, true, Encoding.UTF8))
        {
            // Convert List<string> to List<object>
            var objectList = csvData.Cast<object>().ToList();
            var sanitizedList = Gradio.Net.Utils.Utils.SanitizeListForCsv(objectList);
            var csvLine = string.Join(",", sanitizedList.Select(v => v.ToString()));
            writer.WriteLine(csvLine);
        }

        string StripInvalidFilenameCharacters(string filename)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(filename.Where(c => !invalidChars.Contains(c)).ToArray());
            return sanitized;
        }

        // Count the number of lines in the file
        using (var reader = new StreamReader(logFilePath, Encoding.UTF8))
        {
            var lineCount = 0;
            while (reader.ReadLine() != null)
            {
                lineCount++;
            }
            return lineCount - 1; // Subtract 1 for header if it exists
        }
    }
}
