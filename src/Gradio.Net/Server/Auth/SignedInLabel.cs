using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// Writes the label a signed-in user sees on the button that signs them in.
/// </summary>
/// <remarks>
/// <para>
/// The Python version relabels the button from a load event: the page mounts, the handler asks the
/// session who is signed in, and the answer updates the button. A handler here cannot ask — it does
/// not see the request that carries the session — so the label is written while the page is
/// described instead. The user sees the same thing either way: their name, and an offer to sign
/// out.
/// </para>
/// <para>
/// It is a deliberate difference and is written down in the scope ledger, because it is visible in
/// the config: an authenticated browser is served the sign-out label rather than the sign-in one,
/// where the Python version serves the default label and changes it a moment later.
/// </para>
/// </remarks>
internal static class SignedInLabel
{
    /// <summary>Relabels the buttons that sign users in, when someone is signed in.</summary>
    /// <param name="config">The config being served.</param>
    /// <param name="blocks">The application, whose blocks are what the config describes.</param>
    /// <param name="profile">Who is signed in, or <see langword="null"/> for nobody.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    internal static void Apply(JsonObject config, Blocks blocks, OAuthProfile? profile)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(blocks);

        if (profile is null || config["components"] is not JsonArray components)
        {
            return;
        }

        foreach (var block in blocks.Registered)
        {
            if (block is not IOAuthLoginBlock login)
            {
                continue;
            }

            var label = login.SignedInLabel(profile.UserName);

            foreach (var entry in components)
            {
                if (entry?["id"] is JsonValue id
                    && id.TryGetValue<int>(out var value)
                    && value == block.Id
                    && entry["props"] is JsonObject props)
                {
                    props["value"] = label;
                }
            }
        }
    }
}
