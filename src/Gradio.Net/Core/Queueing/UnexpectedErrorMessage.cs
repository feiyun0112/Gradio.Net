using System.Text.Json.Serialization;

namespace Gradio.Net.Core.Queueing;

public class UnexpectedErrorMessage : EventMessage
{
    public UnexpectedErrorMessage()
    {
        Msg = "unexpected_error";
    }

    public UnexpectedErrorMessage(string message, bool sessionNotFound = false)
    {
        Msg = "unexpected_error";
        Message = message;
        SessionNotFound = sessionNotFound;
    }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("session_not_found")]
    public bool SessionNotFound { get; set; }
}
