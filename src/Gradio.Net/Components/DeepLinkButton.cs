namespace Gradio.Net;
/// <summary>
/// A button that copies a link to the current session, so that the same state can be opened again.
/// </summary>
/// <remarks>
/// <para>
/// The link is the page's own address with a <c>deep_link</c> parameter naming the session's state,
/// and the state is written by the server when the browser asks for it: the button binds a click
/// that fetches <c>/gradio_api/deep_link</c> and puts the answer in the clipboard, and a second click
/// that runs a second later and puts the button's own label back, so that "Link Copied!" is not the
/// label from then on.
/// </para>
/// <para>
/// The Python version names the button's element after how many of them the process has made,
/// because the second click has to address the first one's label. This build derives the same
/// number from the application instead of from the process, so that two applications in one process
/// keep separate numbering — which is the state the Python version's counter keeps process-wide.
/// </para>
/// <para>
/// The Python version draws an icon from an image that ships inside the Python package. This build
/// has no copy of it, so the default is no icon; an application that wants one gives
/// <see cref="Button.Icon"/> a path or a URL.
/// </para>
/// </remarks>
[GradioBlock("deeplinkbutton")]
public sealed partial class DeepLinkButton : Button
{
    internal DeepLinkButton()
    {
    }

    /// <summary>Whether the two clicks have been bound.</summary>
    private bool _activated;

    /// <summary>The label drawn once the link has been copied.</summary>
    public string CopiedValue { get; set; } = "Link Copied!";

    /// <summary>Whether the block is drawn by its parent's component.</summary> <remarks>The Python
    /// version marks this button a template: the frontend draws it with the button component, and
    /// the config names that component rather than this class.</remarks>
    protected override bool IsTemplate => true;

    /// <summary>The text drawn when the application names none.</summary>
    protected override string? DefaultValue => "Share via Link";

    /// <summary>The element id the config carries, which the Python version assigns rather than
    /// takes.</summary>
    /// <value>
    /// <c>gradio-share-link-button-</c> followed by the button's position among the application's
    /// share buttons.
    /// </value>
    /// <remarks>
    /// An <c>elem_id</c> the application sets is ignored, which is the Python version's behaviour:
    /// the constructor overwrites it. The id is computed when the config is built because it counts
    /// the application's blocks, and those exist only once the block has been added.
    /// </remarks>
    [UpstreamParameter("elem_id")]
    private string GeneratedElemId => $"gradio-share-link-button-{Ordinal}";

    /// <summary>How many share buttons the application wrote before this one.</summary>
    private int Ordinal
    {
        get
        {
            if (Root is not { } root)
            {
                return 0;
            }

            var ordinal = 0;
            foreach (var block in root.Registered)
            {
                if (ReferenceEquals(block, this))
                {
                    break;
                }

                if (block is DeepLinkButton)
                {
                    ordinal++;
                }
            }

            return ordinal;
        }
    }

    /// <inheritdoc />
    protected internal override void OnAddedTo(Blocks root)
    {
        base.OnAddedTo(root);

        Activate();
    }

    /// <summary>Binds the two clicks: one copies the link, the other restores the label.</summary>
    /// <remarks>
    /// Binds them once. An application activates the button itself — before the columns exist,
    /// which is where the Python version binds it and therefore where the served bindings are
    /// numbered — and adding the button to the application activates it again; a second pair of
    /// bindings would serve the same click twice.
    /// </remarks>
    public void Activate()
    {
        if (_activated)
        {
            return;
        }

        _activated = true;

        Click(
            fn: (Delegate?)null,
            outputs: [this],
             // The literal is written with the file's line endings and served with one: the
                // script is compared byte for byte.
                js: ShareLinkScript
                    .ReplaceLineEndings("\n")
                    .Replace("%COPIED%", CopiedValue, StringComparison.Ordinal), apiVisibility: ApiVisibility.Private);

        // The label goes back after a second. It is a handler rather than a timer in the script
        // because the value the button shows is the server's: the Python version waits and then writes the
        // label it started with, and does not go through the queue to do it.
        Click(
            fn: (Func<Task<object?>>)Restore,
            outputs: [this],
             apiName: "lambda", queue: false, apiVisibility: ApiVisibility.Undocumented);
    }

    /// <summary>Waits a second, then answers with the label the button started with.</summary>
    private async Task<object?> Restore()
    {
        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

        return Value;
    }

    /// <summary>
    /// The script the Python version binds, with the copied label left as a token.
    /// </summary>
    /// <remarks>
    /// Character for character, including the leading and trailing newline and the empty line the
    /// optional sign-parameter line leaves behind: the script is served in the config, and a
    /// difference in whitespace is a difference in what the browser is handed.
    /// </remarks>
    private const string ShareLinkScript =
        "\n" + """
        () => {
            const sessionHash = window.__gradio_session_hash__;
            fetch(`gradio_api/deep_link?session_hash=${sessionHash}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }
                    return response.text();
                })
                .then(data => {
                    const currentUrl = new URL(window.location.href);
                    const cleanData = data.replace(/^"|"$/g, '');
                    if (cleanData) {
                        currentUrl.searchParams.set('deep_link', cleanData);
                    }

                    navigator.clipboard.writeText(currentUrl.toString());
                })
                .catch(error => {
                    console.error('Error fetching deep link:', error);
                    return "Error";
                });

            return "%COPIED%";
        }
        """ + "\n";
}
