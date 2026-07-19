
namespace Gradio.Net.Data;

public class DeveloperPath
{
    public string Value { get; set; } = string.Empty;

    public static implicit operator DeveloperPath(string value) => new DeveloperPath { Value = value };

    public static implicit operator string(DeveloperPath path) => path.Value;
}
