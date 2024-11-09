using System.Data;

namespace Gradio.Net.ChatBot;

internal class FileMessage
{
    public FileData File { get; set; }
    public string AltText { get; set; } = null;
}

internal class Message
{
    public string Role { get; set; }
    public object Content { get; set; }
    public Metadata Metadata { get; set; }
}

internal class Metadata
{
    public string Title { get; set; }
}