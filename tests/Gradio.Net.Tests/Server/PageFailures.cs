using System.Collections.Concurrent;
using Microsoft.Playwright;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// What kind of thing went wrong while a browser was being driven.
/// </summary>
/// <remarks>
/// A kind rather than a phrase: a test that reads the entries can say "the console complained" or
/// "a resource was refused" without depending on how the entry is worded, so rewording one cannot
/// turn an assertion green.
/// </remarks>
internal enum PageFailureKind
{
    /// <summary>A resource the page asked for was answered with 400 or more.</summary>
    Resource,

    /// <summary>The frontend reported an error while still running.</summary>
    Console,

    /// <summary>A script threw and stopped.</summary>
    Script,
}

/// <summary>
/// One thing that went wrong on a page: what kind, and what it said.
/// </summary>
/// <param name="Kind">Which of the three.</param>
/// <param name="Detail">What the browser said about it.</param>
internal sealed record PageFailure(PageFailureKind Kind, string Detail);

/// <summary>
/// Everything that went wrong while a browser was being driven: what it failed to load, what it
/// complained about, and what it threw.
/// </summary>
/// <remarks>
/// <para>
/// The page is served whole — the template, the vendored frontend, the assets it asks for — and the
/// only way to know it arrived whole is to watch what the browser did with it. Three things count as
/// a failure, and they are three because each catches what the others cannot: a resource answered
/// with 400 or more is something the server refused, a console error is the frontend saying
/// something went wrong while still running, and a page error is a script that threw and stopped.
/// </para>
/// <para>
/// A console <em>error</em> rather than every console message: the frontend logs ordinary progress
/// too, and a test that failed on any of it would be asserting that the bundle is silent, which is
/// not the claim. What is asserted is that nothing on the page reports a failure.
/// </para>
/// </remarks>
internal sealed class PageFailures
{
    private readonly ConcurrentQueue<PageFailure> _entries = new();

    private PageFailures(IPage page)
    {
        page.PageError += (_, error) => _entries.Enqueue(new PageFailure(PageFailureKind.Script, error));

        page.Console += (_, message) =>
        {
            if (string.Equals(message.Type, "error", StringComparison.Ordinal))
            {
                _entries.Enqueue(new PageFailure(PageFailureKind.Console, message.Text));
            }
        };

        page.Response += (_, response) =>
        {
            // The page must load everything it asks for: a missing asset is exactly the failure
            // that a document comparison cannot see.
            if (response.Status >= 400)
            {
                _entries.Enqueue(new PageFailure(PageFailureKind.Resource, $"{response.Status} {response.Url}"));
            }
        };
    }

    /// <summary>What went wrong so far, in the order it happened.</summary>
    internal IReadOnlyCollection<PageFailure> Entries => _entries.ToArray();

    /// <summary>Starts watching a page.</summary>
    /// <param name="page">The page to watch.</param>
    /// <returns>The collector, which fills as the page is driven.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="page"/> is <see langword="null"/>.</exception>
    internal static PageFailures Watch(IPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return new PageFailures(page);
    }
}
