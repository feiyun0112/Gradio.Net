using System.Security.Cryptography;
using System.Text;

namespace Gradio.Net;
/// <summary>
/// Who may open an application: a set of accounts, or a rule that decides for itself.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>launch(auth=...)</c>, which takes a username and password, a list of
/// them, or a function that answers whether a pair is valid. The three are one type here because
/// they answer one question — <see cref="VerifyAsync"/> — and because an application is protected
/// or not, not protected three different ways.
/// </para>
/// <para>
/// A password is compared in constant time (<see cref="CryptographicOperations.FixedTimeEquals"/>),
/// which is what the Python version's <c>compare_passwords_securely</c> does: a comparison that
/// returns early on the first differing byte tells an attacker how much of a guess was right.
/// </para>
/// <para>
/// What this does not do is keep the session: it decides who may sign in, and
/// <see cref="Blocks.Auth"/> says which rule an application admits people by. Issuing and reading
/// the cookie that carries a signed-in session is the host's business.
/// </para>
/// </remarks>
public sealed class GradioAuth
{
    private readonly IReadOnlyDictionary<string, string>? _accounts;
    private readonly Func<string, string, CancellationToken, ValueTask<bool>>? _verify;

    private GradioAuth(IReadOnlyDictionary<string, string> accounts) => _accounts = accounts;

    private GradioAuth(Func<string, string, CancellationToken, ValueTask<bool>> verify) => _verify = verify;

    /// <summary>An application with one account.</summary>
    /// <param name="userName">The name the account signs in with.</param>
    /// <param name="password">The password the account signs in with.</param>
    /// <returns>The rule that admits that pair and no other.</returns>
    /// <exception cref="ArgumentException">Either part is empty.</exception>
    public static GradioAuth OneAccount(string userName, string password) =>
        Accounts((userName, password));

    /// <summary>An application with a fixed set of accounts.</summary>
    /// <param name="accounts">The name and password of each account.</param>
    /// <returns>The rule that admits those pairs and no others.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="accounts"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">There are no accounts, or either part of one is empty.</exception>
    /// <remarks>
    /// <para>
    /// Empty parts are refused rather than accepted. The Python version warns and starts anyway,
    /// because it has a warning channel and this framework does not; an account with an empty
    /// password is a door left open, and starting with one would look like protection. That is a
    /// deliberate deviation and is written down in the scope ledger.
    /// </para>
    /// <para>
    /// A name that appears twice keeps the last password it was given, which is what assigning to
    /// the dictionary the Python version builds from the same list does.
    /// </para>
    /// </remarks>
    public static GradioAuth Accounts(params (string UserName, string Password)[] accounts)
    {
        ArgumentNullException.ThrowIfNull(accounts);

        if (accounts.Length == 0)
        {
            throw new ArgumentException("An application protected by accounts needs at least one.", nameof(accounts));
        }

        var byName = new Dictionary<string, string>(accounts.Length, StringComparer.Ordinal);

        foreach (var (userName, password) in accounts)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("An account needs a username.", nameof(accounts));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(
                    $"The account '{userName}' needs a password; an empty one admits anyone who guesses the name.",
                    nameof(accounts));
            }

            byName[userName] = password;
        }

        return new GradioAuth(byName);
    }

    /// <summary>An application that decides for itself who may sign in.</summary>
    /// <param name="verify">The rule, answered from a username and a password.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="verify"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The synchronous form of the rule, for one that reads a dictionary or compares a hash. A rule
    /// that has to ask somewhere else — a database, an identity service — takes the asynchronous
    /// overload instead, so that signing in does not hold a thread while it waits.
    /// </remarks>
    public static GradioAuth Verify(Func<string, string, bool> verify)
    {
        ArgumentNullException.ThrowIfNull(verify);

        return new GradioAuth((userName, password, _) => ValueTask.FromResult(verify(userName, password)));
    }

    /// <summary>An application that decides for itself who may sign in, possibly by asking elsewhere.</summary>
    /// <param name="verify">The rule, answered from a username and a password.</param>
    /// <returns>The rule.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="verify"/> is <see langword="null"/>.</exception>
    public static GradioAuth Verify(Func<string, string, CancellationToken, ValueTask<bool>> verify)
    {
        ArgumentNullException.ThrowIfNull(verify);

        return new GradioAuth(verify);
    }

    /// <summary>Whether a pair of credentials may open the application.</summary>
    /// <param name="userName">The username as it was submitted.</param>
    /// <param name="password">The password as it was submitted.</param>
    /// <param name="cancellationToken">Cancels the answer.</param>
    /// <returns><see langword="true"/> when the pair is one the rule admits.</returns>
    /// <remarks>
    /// A rule of accounts answers for a name it does not have the same way it answers for a wrong
    /// password, and compares the password in constant time either way: the caller learns only
    /// whether the pair is valid.
    /// </remarks>
    public ValueTask<bool> VerifyAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        if (_verify is not null)
        {
            return _verify(userName, password, cancellationToken);
        }

        return ValueTask.FromResult(
            _accounts!.TryGetValue(userName, out var expected) && SameBytes(password, expected));
    }

    /// <summary>Whether two strings are the same, compared without returning early on a difference.</summary>
    /// <param name="left">One string.</param>
    /// <param name="right">The other.</param>
    /// <returns><see langword="true"/> when they are equal.</returns>
    /// <remarks>
    /// The comparison runs over both strings whole, so how long it takes says nothing about how
    /// many leading characters matched. Lengths still differ visibly — as they do the Python
    /// version, where the same comparison is <c>hmac.compare_digest</c>.
    /// </remarks>
    private static bool SameBytes(string left, string right) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(left), Encoding.UTF8.GetBytes(right));
}
