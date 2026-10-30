using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A button that signs the user in with Hugging Face, and signs them out once they are.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's button binds two things when it is created: a click that sends the browser
/// to <c>/login/huggingface</c> — or to <c>/logout</c>, once the label has changed — and a load
/// event that asks the server who is signed in and relabels the button with their name. The click
/// is bound here, because it is the button's own behaviour. The relabelling is not: a handler
/// cannot see the request that carries the session, so the label is written while the page is
/// described instead — <see cref="IOAuthLoginBlock"/> is what the host asks — and the user sees the
/// same thing either way (see the scope ledger).
/// </para>
/// <para>
/// The Python version draws the Hugging Face logo by default, from an image that ships inside the
/// Python package. This build has no copy of it, so the default is no icon; an application that
/// wants one gives <see cref="Button.Icon"/> a path or a URL.
/// </para>
/// </remarks>
[GradioBlock("loginbutton")]
public sealed partial class LoginButton : Button, IOAuthLoginBlock
{
    internal LoginButton()
    {
    }

    /// <summary>The label drawn once the user is signed in.</summary>
    /// <value>
    /// A format string with one placeholder, which the user's name is put into — the Python
    /// version's <c>"Logout ({})"</c>.
    /// </value>
    public string LogoutValue { get; set; } = "Logout ({})";

    /// <summary>Whether the block is drawn by its parent's component.</summary> <remarks>The Python
    /// version marks this button a template: the frontend draws it with the button component, and
    /// the config names that component rather than this class.</remarks>
    protected override bool IsTemplate => true;

    /// <inheritdoc />
    /// <remarks>
    /// The placeholder is the Python version's <c>{}</c> rather than .NET's <c>{0}</c>, because the
    /// value an application writes is the one its Python counterpart writes — <c>"Logout ({})"</c>
    /// — and a label is a string an application copies from a guide, not something to translate
    /// between two languages' formatting rules.
    /// </remarks>
    public string SignedInLabel(string userName) =>
        LogoutValue.Replace("{}", userName, StringComparison.Ordinal);

    /// <summary>The text drawn when the application names none.</summary>
    protected override string? DefaultValue => "Sign in with Hugging Face";

    /// <summary>The variant drawn when the application names none.</summary>
    protected override string DefaultVariant => "huggingface";

    /// <inheritdoc />
    protected internal override void OnAddedTo(Blocks root)
    {
        base.OnAddedTo(root);

        Activate();
    }

    /// <summary>
    /// Binds the click that sends the browser to the sign-in or sign-out route.
    /// </summary>
    /// <remarks>
    /// The binding runs entirely in the browser: it compares the button's own label with the label
    /// it started with, so one script serves both directions. The Python version builds the address
    /// from the page the button belongs to, which is why the script is assembled rather than
    /// written out.
    /// </remarks>
    public void Activate()
    {
        var label = PythonJson.Dump(JsonValue.Create(Value ?? string.Empty));

        // The template says `_target_url=/`, so the page goes in as it is spelled: the home page is
        // the empty path, and a routed page is its own suffix.
        var page = PagePath;

        // The literal is written with the file's line endings and served with one: the script is
        // compared byte for byte, and a checkout that stores the file with CRLF would otherwise
        // serve a script no browser the Python version ever sees.
        var script = RedirectScript
            .ReplaceLineEndings("\n")
            .Replace("BUTTON_DEFAULT_VALUE", label, StringComparison.Ordinal)
            .Replace("REDIRECT_URL", page, StringComparison.Ordinal);

        Click(
            fn: null,
            inputs: [this],
             js: script);
    }

    /// <summary>
    /// The script the Python version binds, with the label it compares against and the page it
    /// returns to left as tokens.
    /// </summary>
    /// <remarks>
    /// Character for character, including the leading and trailing newline: the script is served in
    /// the config, and a difference in whitespace is a difference in what the browser is handed.
    /// </remarks>
    private const string RedirectScript =
        "\n" + """
        (buttonValue) => {
            uri = buttonValue === BUTTON_DEFAULT_VALUE ? '/login/huggingface?_target_url=/REDIRECT_URL' : '/logout?_target_url=/REDIRECT_URL';
            window.parent?.postMessage({ type: "SET_SCROLLING", enabled: true }, "*");
            setTimeout(() => {
                window.location.assign(uri + window.location.search);
            }, 500);
        }
        """ + "\n";
}
