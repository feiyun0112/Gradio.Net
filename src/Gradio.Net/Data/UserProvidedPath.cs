
namespace Gradio.Net.Data;

public class UserProvidedPath
{
    public string Value { get; set; } = string.Empty;

    public static implicit operator UserProvidedPath(string value) => new UserProvidedPath { Value = value };

    public static implicit operator string(UserProvidedPath path) => path.Value;
}
