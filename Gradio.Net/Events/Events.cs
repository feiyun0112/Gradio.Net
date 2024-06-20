using Gradio.Net.Enums;

namespace Gradio.Net;


public interface IHaveReleaseEvent
{
}
public interface IHaveEndEvent
{
}
public interface IHavePauseEvent
{
}
public interface IHavePauseRecordingEvent
{
}

public interface IHavePlayEvent
{
}
public interface IHaveStopEvent
{
}
public interface IHaveStopRecordingEvent
{
}
public interface IHaveStartRecordingEvent
{
}
public interface IHaveClearEvent
{
}
public interface IHaveUploadEvent
{
}
public interface IHaveStreamEvent
{
}
public interface IHaveChangeEvent
{
}
public interface IHaveBlurEvent
{
}
public interface IHaveFocusEvent
{
}
public interface IHaveSubmitEvent
{
}
public interface IHaveSelectEvent
{
}
public interface IHaveInputEvent
{
}
public interface IHaveClickEvent
{
}
internal static class Events
{
    internal static EventListener Click { get; } = new EventListener("click", doc: "Triggered when the {{ component }} is clicked.");
    internal static EventListener Change { get; } = new EventListener("change", doc: "Triggered when the value of the {{ component }} changes either because of user input (e.g. a user types in a textbox) OR because of a function update (e.g. an image receives a value from the output of an event trigger). See `.input()` for a listener that is only triggered by user input.");
    internal static EventListener Input { get; } = new EventListener("input", doc: "This listener is triggered when the user changes the value of the {{ component }}.");
    internal static EventListener Submit { get; } = new EventListener("submit", doc: "This listener is triggered when the user presses the Enter key while the {{ component }} is focused.");
    internal static EventListener Edit { get; } = new EventListener("edit", doc: "This listener is triggered when the user edits the {{ component }} (e.g. image) using the built-in editor.");
    internal static EventListener Clear { get; } = new EventListener("clear", doc: "This listener is triggered when the user clears the {{ component }} using the X button for the component.");
    internal static EventListener Play { get; } = new EventListener("play", doc: "This listener is triggered when the user plays the media in the {{ component }}.");
    internal static EventListener Pause { get; } = new EventListener("pause", doc: "This listener is triggered when the media in the {{ component }} stops for any reason.");
    internal static EventListener Stop { get; } = new EventListener("stop", doc: "This listener is triggered when the user reaches the end of the media playing in the {{ component }}.");
    internal static EventListener End { get; } = new EventListener("end", doc: "This listener is triggered when the user reaches the end of the media playing in the {{ component }}.");

    internal static EventListener StartRecording { get; } = new EventListener("start_recording", doc: "This listener is triggered when the user starts recording with the {{ component }}.");
    internal static EventListener PauseRecording { get; } = new EventListener("pause_recording", doc: "This listener is triggered when the user pauses recording with the {{ component }}.");
    internal static EventListener StopRecording { get; } = new EventListener("stop_recording", doc: "This listener is triggered when the user stops recording with the {{ component }}.");
    internal static EventListener Focus { get; } = new EventListener("focus", doc: "This listener is triggered when the {{ component }} is focused.");
    internal static EventListener Blur { get; } = new EventListener("blur", doc: "This listener is triggered when the {{ component }} is unfocused/blurred.");
    internal static EventListener Upload { get; } = new EventListener("upload", doc: "This listener is triggered when the user uploads a file into the {{ component }}.");
    internal static EventListener Release { get; } = new EventListener("release", doc: "This listener is triggered when the user releases the mouse on this {{ component }}.");
    internal static EventListener Select { get; } = new EventListener("select", callback: async (block) => await block.SetSelectable(true), doc: "Event listener for when the user selects or deselects the {{ component }}. Uses event data gradio.SelectData to carry `value` referring to the label of the {{ component }}, and `selected` to refer to state of the {{ component }}. See EventData documentation on how to use this event data");
    internal static EventListener Stream { get; } = new EventListener("stream", showProgress: ShowProgress.Hidden, configData: () => new Dictionary<string, object> { { "streamable", false } }, callback: async (block) => block.Streaming = true, doc: "This listener is triggered when the user streams the {{ component }}.");
    internal static EventListener Like { get; } = new EventListener("like", showProgress: ShowProgress.Hidden, configData: () => new Dictionary<string, object> { { "likeable", false } }, callback: async (block) => block.Likeable = true, doc: "This listener is triggered when the user likes/dislikes from within the {{ component }}. This event has EventData of type gradio.LikeData that carries information, accessible through LikeData.index and LikeData.value. See EventData documentation on how to use this event data.");
    internal static EventListener Load { get; } = new EventListener("load", doc: "This listener is triggered when the {{ component }} initially loads in the browser.");
    internal static EventListener KeyUp { get; } = new EventListener("key_up", doc: "This listener is triggered when the user presses a key while the {{ component }} is focused.");
    internal static EventListener Apply { get; } = new EventListener("apply", doc: "This listener is triggered when the user applies changes to the {{ component }} through an integrated UI action.");
}
