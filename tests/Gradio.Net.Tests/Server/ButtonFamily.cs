using Gradio.Net;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A page holding the button family, so that a browser can be asked whether the buttons do what
/// they are for.
/// </summary>
/// <remarks>
/// <para>
/// The manifest page proves what the buttons <em>say</em>; this one is for what a person can
/// <em>do</em> with them. It holds a field to empty, one file to upload and a list of them, a file to
/// download — one the page was built with and one a handler makes — plus the share button: the
/// behaviours that are a click rather than a label.
/// </para>
/// <para>
/// The sign-in button is not here: clicking it leaves the page for the sign-in route, which is
/// authentication's own batch (ticket 29), and a test that followed it would assert a page this
/// batch does not serve. The duplicate button is not here either, for the Python version's reason:
/// outside a Space it binds nothing, so there is nothing to click.
/// </para>
/// </remarks>
internal static class ButtonFamily
{
    /// <summary>The text typed into the textbox before it is cleared.</summary>
    internal const string Typed = "typed here";

    /// <summary>The number typed into the number field before it is cleared.</summary>
    internal const string Counted = "7";

    /// <summary>The name of the file the test uploads.</summary>
    internal const string Uploaded = "notes.txt";

    /// <summary>The label the share button shows while the link is being copied.</summary>
    internal const string Copied = "Copied";

    /// <summary>The label the clear button shows.</summary>
    internal const string Cleared = "Clear the fields";

    /// <summary>The name of the file the page is built with, which is also the download's name.</summary>
    internal const string Downloaded = "download.txt";

    /// <summary>What that file holds, which is what the download must carry.</summary>
    internal const string DownloadedText = "the bytes a person downloads";

    /// <summary>The name of the file the prepare button makes.</summary>
    internal const string Handed = "handed.txt";

    /// <summary>What the prepare button writes into it.</summary>
    internal const string HandedText = "the bytes a handler made";

    /// <summary>What the prepare button's second output says, once the file is made.</summary>
    internal const string Ready = "ready";

    /// <summary>Builds the page into an application.</summary>
    /// <param name="blocks">The application to render into.</param>
    internal static void Build(Blocks blocks)
    {
        var file = Write(Downloaded, DownloadedText);

        using (blocks.BeginRender())
        {
            // Both fields are editable because the application says so: the Python version draws a field
            // that no handler reads as read-only, and the clear button writes to these two rather
            // than reading them.
            var name = blocks.Add(new Textbox { Label = "Name", Interactive = true });
            var count = blocks.Add(new Number { Label = "Count", Interactive = true });
            var got = blocks.Add(new Textbox { Label = "Got" });
            var listed = blocks.Add(new Textbox { Label = "Many" });

            blocks.Add(new ClearButton
            {
                Components = [name, count],
                Value = Cleared,
            });

            var upload = blocks.Add(new UploadButton { Label = "Upload one" });

            var many = blocks.Add(new UploadButton
            {
                Label = "Upload many",
                FileCount = "multiple",
                FileTypes = [".txt"],
            });

            var download = blocks.Add(new DownloadButton { Label = "Get", Value = file });
            var status = blocks.Add(new Textbox { Label = "Status" });

            blocks.Add(new DeepLinkButton { Value = "Share", CopiedValue = Copied });

            // A file the handler makes, so that what a person downloads is what the handler
            // answered rather than what the page was built with. The status field is how a test
            // knows the run has finished: a download button's value is the browser's, so nothing on
            // the page says when it changed.
            var prepare = blocks.Add(new Button { Value = "Prepare" });

            prepare.Click(() => (Write(Handed, HandedText), Ready), [download, status]);

            // The upload is answered by a field: what a handler is handed is the file's path, and a
            // field holding it is how a test reads what the button reported.
            upload.Upload(
                path => Path.GetFileName(path),
                [upload],
                [got]);

            many.Upload(
                (Func<string[], object?>)(paths =>
                    $"{paths.Length}: {string.Join(", ", paths.Select(Path.GetFileName))}"),
                [many],
                [listed]);
        }
    }

    /// <summary>Writes a file in a folder of its own, outside the application's cache.</summary>
    /// <param name="name">The file's name, which is what a download is called.</param>
    /// <param name="text">What to write into it.</param>
    /// <returns>The file's path, which is what a download button's value is.</returns>
    private static string Write(string name, string text)
    {
        var folder = Path.Combine(Path.GetTempPath(), $"gradio-buttons-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, name);
        System.IO.File.WriteAllText(path, text);

        return path;
    }
}
