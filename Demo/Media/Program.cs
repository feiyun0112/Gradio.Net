using Gradio.Net;

// Media in and out, and what the browser reports while you use it.
//
// The framework moves files, it does not convert them. A handler that returns the path it was given
// is a handler that returns the same bytes, and that is the whole of what the first three rows show —
// the replacement for "install ffmpeg and let the library transcode" is a handler that decodes,
// changes and re-encodes with a library the application chose, before it returns.
//
//     image.change(same, image, image)          # the path in is the path out
//
// The last row is the other half of a media component: the signals it raises. A video reports when it
// is uploaded, when its value changes, and when it is played or paused, and each of those is a
// binding like any other — so a page can react to what the reader does with the player rather than
// only to the file they chose.

static string Same(string path) => path;

static double Count(double current) => current + 1;

var demo = gr.Blocks(title: "Media");

using (demo)
{
    gr.Markdown(
        """
        # Media

        Three kinds of file, each one handed to a handler and handed back. Nothing is converted:
        what comes out is the file that went in, served from the application's own cache.
        """);

    gr.Markdown("## Image");

    var picture = gr.Image(label: "Picture", type: "filepath");
    var pictureOut = gr.Image(label: "Handed back", type: "filepath", interactive: false);

    picture.Change(Same, [picture], [pictureOut], apiName: "image");

    gr.Markdown("## Video");

    var video = gr.Video(label: "Video", format: "mp4");
    var videoOut = gr.Video(label: "Handed back", interactive: false);

    video.Change(Same, [video], [videoOut], apiName: "video");

    gr.Markdown("## Audio");

    // The two ways a browser can give the page a sound: a file it uploads, or the microphone.
    string[] sources = ["upload", "microphone"];

    var sound = gr.Audio(label: "Sound", type: "filepath", sources: sources);
    var soundOut = gr.Audio(label: "Handed back", type: "filepath", interactive: false);

    sound.Change(Same, [sound], [soundOut], apiName: "audio");

    gr.Markdown(
        """
        ## What the player reports

        Four counters, one per signal the video raises. Upload a file, scrub it, press play and pause:
        each one is a run, and each run is the handler below.
        """);

    var uploaded = gr.Number(value: 0, label: "upload", interactive: false);
    var changed = gr.Number(value: 0, label: "change", interactive: false);
    var played = gr.Number(value: 0, label: "play", interactive: false);
    var paused = gr.Number(value: 0, label: "pause", interactive: false);

    video.Upload((double count) => Count(count), [uploaded], [uploaded], apiName: "uploaded");
    video.Change((double count) => Count(count), [changed], [changed], apiName: "changed");
    video.Play((double count) => Count(count), [played], [played], apiName: "played");
    video.Pause((double count) => Count(count), [paused], [paused], apiName: "paused");
}

// Blocks until Ctrl-C. The page is at http://127.0.0.1:7860.
demo.Launch();
