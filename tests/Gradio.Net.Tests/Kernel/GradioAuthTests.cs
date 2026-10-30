using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The rule that decides who may open an application: what it admits, and what it refuses to be
/// built from.
/// </summary>
/// <remarks>
/// The refusals are the interesting half. The Python version raises at launch for an account with
/// an empty name or password, and this framework refuses to build the rule at all — an account with
/// an empty password is a door left open rather than a configuration, and a check that only ran
/// when someone signed in would let the application start and look protected.
/// </remarks>
public sealed class GradioAuthTests
{
    [Fact]
    public async Task An_account_admits_its_own_pair_and_nothing_else()
    {
        var auth = GradioAuth.OneAccount("alice", "s3cret");

        Assert.True(await auth.VerifyAsync("alice", "s3cret", TestContext.Current.CancellationToken));
        Assert.False(await auth.VerifyAsync("alice", "s3cre", TestContext.Current.CancellationToken));
        Assert.False(await auth.VerifyAsync("Alice", "s3cret", TestContext.Current.CancellationToken));
        Assert.False(await auth.VerifyAsync("bob", "s3cret", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task A_set_of_accounts_admits_each_of_them()
    {
        var auth = GradioAuth.Accounts(("alice", "one"), ("bob", "two"));

        Assert.True(await auth.VerifyAsync("alice", "one", TestContext.Current.CancellationToken));
        Assert.True(await auth.VerifyAsync("bob", "two", TestContext.Current.CancellationToken));

        // The passwords are not interchangeable: a name finds its own password and no other.
        Assert.False(await auth.VerifyAsync("alice", "two", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task A_rule_that_has_to_ask_somewhere_else_is_awaited()
    {
        var asked = 0;

        var auth = GradioAuth.Verify(async (name, password, cancellationToken) =>
        {
            await Task.Yield();
            asked++;

            return name == password;
        });

        Assert.True(await auth.VerifyAsync("same", "same", TestContext.Current.CancellationToken));
        Assert.False(await auth.VerifyAsync("same", "other", TestContext.Current.CancellationToken));
        Assert.Equal(2, asked);
    }

    [Fact]
    public async Task A_name_that_appears_twice_keeps_the_last_password_it_was_given()
    {
        var auth = GradioAuth.Accounts(("alice", "one"), ("alice", "two"));

        // The Python version builds a dictionary from the same list, and assigning to a key twice keeps the
        // last value.
        Assert.True(await auth.VerifyAsync("alice", "two", TestContext.Current.CancellationToken));
        Assert.False(await auth.VerifyAsync("alice", "one", TestContext.Current.CancellationToken));
    }

    [Fact]
    public void An_account_with_an_empty_part_is_refused()
    {
        // The Python version warns and starts anyway; a warning channel this framework does not have. Starting
        // with an empty password would look like protection, so the rule is refused instead.
        Assert.Throws<ArgumentException>(() => GradioAuth.Accounts(("", "password")));
        Assert.Throws<ArgumentException>(() => GradioAuth.Accounts(("alice", "")));
        Assert.Throws<ArgumentException>(() => GradioAuth.Accounts());
    }
}
