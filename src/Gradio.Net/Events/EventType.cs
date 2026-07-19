using System.Text.Json;
using Gradio.Net.Components;

namespace Gradio.Net.Events;

/// <summary>///</summary>
public static class EventType
{
    /// <summary>///</summary>
    public const string Load = "load";

    /// <summary>///</summary>
    public const string Change = "change";

    /// <summary>///</summary>
    public const string Input = "input";

    /// <summary>///</summary>
    public const string Click = "click";

    /// <summary>///</summary>
    public const string DoubleClick = "double_click";

    /// <summary>///</summary>
    public const string Submit = "submit";

    /// <summary>///</summary>
    public const string Stop = "stop";

    /// <summary>///</summary>
    public const string Edit = "edit";

    /// <summary>///</summary>
    public const string Select = "select";

    /// <summary>///</summary>
    public const string Like = "like";

    /// <summary>///</summary>
    public const string Retry = "retry";

    /// <summary>///</summary>
    public const string Undo = "undo";

    /// <summary>///</summary>
    public const string KeyDown = "key_down";

    /// <summary>///</summary>
    public const string KeyUp = "key_up";

    /// <summary>///</summary>
    public const string Copy = "copy";

    /// <summary>///</summary>
    public const string Paste = "paste";

    /// <summary>///</summary>
    public const string Delete = "delete";

    /// <summary>///</summary>
    public const string Download = "download";

    /// <summary>///</summary>
    public const string Clear = "clear";

    /// <summary>///</summary>
    public const string Play = "play";

    /// <summary>///</summary>
    public const string Pause = "pause";

    /// <summary>///</summary>
    public const string End = "end";

    /// <summary>///</summary>
    public const string StartRecording = "start_recording";

    /// <summary>///</summary>
    public const string PauseRecording = "pause_recording";

    /// <summary>///</summary>
    public const string StopRecording = "stop_recording";

    /// <summary>///</summary>
    public const string Focus = "focus";

    /// <summary>///</summary>
    public const string Blur = "blur";

    /// <summary>///</summary>
    public const string Upload = "upload";

    /// <summary>///</summary>
    public const string Release = "release";

    /// <summary>///</summary>
    public const string Stream = "stream";

    /// <summary>///</summary>
    public const string ExampleSelect = "example_select";

    /// <summary>///</summary>
    public const string OptionSelect = "option_select";

    /// <summary>///</summary>
    public const string Apply = "apply";

    /// <summary>///</summary>
    public const string Tick = "tick";

    /// <summary>///</summary>
    public const string Expand = "expand";

    /// <summary>///</summary>
    public const string Collapse = "collapse";
}
