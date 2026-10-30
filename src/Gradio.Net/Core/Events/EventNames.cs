namespace Gradio.Net;
/// <summary>
/// The Python spellings of the signals an event binding can listen for.
/// </summary>
/// <remarks>
/// <para>
/// The names are the wire spellings: they are what the config carries in <c>targets</c> and what
/// the vendored frontend dispatches on, so they cannot be the C# member names. Holding them in one
/// place is what stops a component from listening for <c>on_change</c> while its neighbour listens
/// for <c>change</c>, and what makes the set auditable against the Python version's list.
/// </para>
/// <para>
/// Only the signals the components in this build declare are here. The Python version has more — a
/// gallery drags, a dataset pages — and each arrives with the component that raises it.
/// </para>
/// </remarks>
public static class EventNames
{
    /// <summary>The value changed, whether the user changed it or a handler did.</summary>
    public const string Change = "change";

    /// <summary>The user changed the value.</summary>
    public const string Input = "input";

    /// <summary>The value was submitted.</summary>
    public const string Submit = "submit";

    /// <summary>The block took focus.</summary>
    public const string Focus = "focus";

    /// <summary>The block lost focus.</summary>
    public const string Blur = "blur";

    /// <summary>The user stopped a running event on the block.</summary>
    public const string Stop = "stop";

    /// <summary>The user let go of a value they were dragging.</summary>
    public const string Release = "release";

    /// <summary>The user pressed a key while the block had focus.</summary>
    public const string KeyUp = "key_up";

    /// <summary>The value was cleared.</summary>
    public const string Clear = "clear";

    /// <summary>The user selected something in the block.</summary>
    public const string Select = "select";

    /// <summary>The user clicked the block.</summary>
    public const string Click = "click";

    /// <summary>The user double-clicked the block.</summary>
    public const string DoubleClick = "double_click";

    /// <summary>The media started playing.</summary>
    public const string Play = "play";

    /// <summary>The media was paused.</summary>
    public const string Pause = "pause";

    /// <summary>The media reached its end.</summary>
    public const string End = "end";

    /// <summary>The user started recording.</summary>
    public const string StartRecording = "start_recording";

    /// <summary>The user paused a recording.</summary>
    public const string PauseRecording = "pause_recording";

    /// <summary>The user finished recording.</summary>
    public const string StopRecording = "stop_recording";

    /// <summary>The user applied the editor's changes.</summary>
    public const string Apply = "apply";

    /// <summary>A timer ticked.</summary>
    public const string Tick = "tick";

    /// <summary>The application finished loading.</summary>
    public const string Load = "load";

    /// <summary>The page that was showing the application went away.</summary>
    /// <remarks>
    /// The signal the application raises about itself rather than one a component raises: nothing
    /// on the page fires it, because nothing on the page is left to fire anything. The server is
    /// what knows the browser is gone — it is the far end of the connection the page held open —
    /// and it is what runs the bindings that listen for this (see <see cref="Blocks.Unload"/>).
    /// </remarks>
    public const string Unload = "unload";

    /// <summary>The user opened a preview.</summary>
    public const string PreviewOpen = "preview_open";

    /// <summary>The user closed a preview.</summary>
    public const string PreviewClose = "preview_close";

    /// <summary>The user uploaded a file into the block.</summary>
    public const string Upload = "upload";

    /// <summary>The user deleted one of the block's files.</summary>
    public const string Delete = "delete";

    /// <summary>The user asked to download what the block holds.</summary>
    public const string Download = "download";

    /// <summary>The block is producing values as it runs.</summary>
    public const string Stream = "stream";

    /// <summary>The user liked or disliked something in the block.</summary>
    public const string Like = "like";

    /// <summary>The user asked for the last exchange to be answered again.</summary>
    public const string Retry = "retry";

    /// <summary>The user asked to take the last exchange back.</summary>
    public const string Undo = "undo";

    /// <summary>The user picked one of the block's examples.</summary>
    public const string ExampleSelect = "example_select";

    /// <summary>The user picked one of a message's options.</summary>
    public const string OptionSelect = "option_select";

    /// <summary>The user edited a message.</summary>
    public const string Edit = "edit";

    /// <summary>The user copied something out of the block.</summary>
    public const string Copy = "copy";

    /// <summary>The user opened a collapsible layout.</summary>
    public const string Expand = "expand";

    /// <summary>The user closed a collapsible layout.</summary>
    public const string Collapse = "collapse";
}
