
namespace Gradio.Net.Components
{
    public class DialogueModelData
    {
        public object Root { get; set; } = string.Empty;

        public object ModelDump()
        {
            if (Root is IEnumerable<DialogueLineData> lines)
            {
                return lines.Select(l => new Dictionary<string, string>
                {
                    ["speaker"] = l.Speaker,
                    ["text"] = l.Text
                }).ToList();
            }

            return Root;
        }
    }
}
