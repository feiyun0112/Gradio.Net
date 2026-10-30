using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace Gradio.Net.Tests.Demo;

/// <summary>
/// The demos in <c>Demo/</c> are for people, so what is checked about them is structure rather than
/// behaviour: that each one is a program, that the build compiles it, and that it stays offline.
/// </summary>
/// <remarks>
/// <para>
/// The repository separates the two kinds of example: a demo is read and run by hand, the suite's own
/// pages are compared by machine. The behaviour of what a demo builds is therefore already asserted
/// by the suite — what is left, and what these tests are, is the part only a demo can get wrong: a
/// directory nobody can run, a demo that quietly reaches for a token, a page that is not in the
/// build and so rots without anybody noticing.
/// </para>
/// <para>
/// The scans are over the source rather than over the built programs, and each one carries a
/// positive control: a scan that found nothing would pass whatever the demos did, which is the way a
/// structural test stops testing anything.
/// </para>
/// </remarks>
public sealed class DemoProjectTests
{
    /// <summary>The demos' directory.</summary>
    private static readonly string _demos = Path.Combine(RepositoryRoot.FullPath, "Demo");

    /// <summary>The index a reader arrives from.</summary>
    private static readonly string _readme = Path.Combine(_demos, "README.md");

    /// <summary>A link from the index to a demo's directory.</summary>
    private static readonly Regex _demoLink = new(@"\]\((?<name>[A-Za-z0-9_-]+)/\)", RegexOptions.CultureInvariant);

    /// <summary>An address written in a string literal.</summary>
    private static readonly Regex _address = new(@"https?://(?<host>[^/\s"":]+)", RegexOptions.CultureInvariant);

    /// <summary>
    /// The names that mean "read a value the environment or the host's configuration supplies".
    /// </summary>
    /// <remarks>
    /// A host's configuration is where the environment ends up, so the two spellings are one reach:
    /// a demo that read a key either way would be a demo that does not run on a machine without one.
    /// The list is of identifiers rather than of call shapes because a demo may reach the
    /// environment through a property as easily as through a call.
    /// </remarks>
    private static readonly string[] _readingTheEnvironment =
    [
        "Configuration",
        "GetEnvironmentVariable",
        "GetEnvironmentVariables",
    ];

    /// <summary>
    /// The scenarios the demos exist for, one row per scenario the batch asked for.
    /// </summary>
    /// <remarks>
    /// A scenario is covered by a demo whose name says so; the row is the requirement rather than a
    /// list of files, so removing a demo that covers one of these is a failing test rather than a
    /// quiet loss of the example that explained it.
    /// </remarks>
    [Theory]
    [InlineData("Quickstart", "one call builds a page around a function")]
    [InlineData("Blocks", "a page written by hand, with events bound to its blocks")]
    [InlineData("Chat", "a conversation, answered by an endpoint")]
    [InlineData("Streaming", "a handler that answers in pieces, with progress")]
    [InlineData("Form", "several fields submitted together, and one field deciding another")]
    [InlineData("Files", "a file uploaded, converted and downloaded")]
    [InlineData("Media", "a picture, a video and a sound handed to a handler and back")]
    [InlineData("Multipage", "one application, several pages")]
    [InlineData("Themes", "a page's theme")]
    [InlineData("Charts", "a chart as data rather than as a figure")]
    public void The_scenario_is_covered_by_a_demo(string demo, string scenario)
    {
        Assert.True(
            Directory.Exists(Path.Combine(_demos, demo)),
            $"No demo covers {scenario}: '{demo}' is missing from Demo/.");
    }

    /// <summary>
    /// Every directory under <c>Demo/</c> is a program a person can run, and the index names it —
    /// and the index names nothing else.
    /// </summary>
    /// <remarks>
    /// "Can run" is two files: a project of the demo's own name, and the program in it. Both are
    /// asserted rather than assumed, because a demo that is a folder of notes is a demo that does
    /// not start. The index is what a reader arrives from, so it is compared as a set: a demo
    /// missing from it is a demo nobody finds, and a row whose directory was renamed is a reader
    /// following a link to nothing.
    /// </remarks>
    [Fact]
    public void Every_demo_is_a_program_the_index_names()
    {
        var demos = DemoDirectories();

        Assert.NotEmpty(demos);

        foreach (var demo in demos)
        {
            Assert.True(
                System.IO.File.Exists(Path.Combine(_demos, demo, demo + ".csproj")),
                $"Demo/{demo} has no {demo}.csproj, so 'dotnet run' in it starts nothing.");

            Assert.True(
                System.IO.File.Exists(Path.Combine(_demos, demo, "Program.cs")),
                $"Demo/{demo} has no Program.cs, so its project has nothing to run.");
        }

        Assert.Equal(demos, LinkedDemoNames());
    }

    /// <summary>
    /// No demo reads a token from the environment or calls an address it does not own.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A demo is the first thing a reader copies, so a demo that needs a key would be a demo that
    /// does not run; and one that calls a fixed address would stop running the day the address
    /// changed. Both are properties of the source rather than of a run, which is why they are read
    /// out of it: every string literal that holds an <c>http</c> address has to point at loopback,
    /// and nothing may read the environment — directly, or through the host's configuration, which
    /// is where the environment ends up.
    /// </para>
    /// <para>
    /// Comments are not scanned — the parser hands over the literals and not the prose — so a demo
    /// may quote an address in its explanation of what it is not doing.
    /// </para>
    /// </remarks>
    [Fact]
    public void No_demo_reads_a_token_or_calls_an_address_it_does_not_own()
    {
        var sources = SourceFiles();
        var addresses = new List<string>();
        var offenders = new List<string>();

        foreach (var file in sources)
        {
            var root = CSharpSyntaxTree
                .ParseText(
                    System.IO.File.ReadAllText(file.Absolute),
                    options: null,
                    path: file.Relative,
                    encoding: null,
                    cancellationToken: TestContext.Current.CancellationToken)
                .GetRoot(TestContext.Current.CancellationToken);
            var relative = file.Relative;

            foreach (var text in Literals(root))
            {
                foreach (Match address in _address.Matches(text))
                {
                    var host = address.Groups["host"].Value;
                    addresses.Add(host);

                    if (!string.Equals(host, "127.0.0.1", StringComparison.Ordinal)
                        && !string.Equals(host, "localhost", StringComparison.Ordinal))
                    {
                        offenders.Add($"{relative}: {address.Value}");
                    }
                }
            }

            // Configuration is where a host puts the environment, so reading either is the same
            // reach: `builder.Configuration["OPENAI_API_KEY"]` and
            // `Environment.GetEnvironmentVariable("OPENAI_API_KEY")` differ only in spelling.
            var reach = root.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Select(name => name.Identifier.ValueText)
                .FirstOrDefault(name => _readingTheEnvironment.Contains(name, StringComparer.Ordinal));

            if (reach is not null)
            {
                offenders.Add($"{relative}: reads {reach}");
            }
        }

        // The control: the demos do address servers, so the scan is looking at addresses. Without
        // this, a parser change that stopped finding literals would turn the rule into a no-op.
        Assert.NotEmpty(addresses);

        Assert.True(
            offenders.Count == 0,
            "A demo has to run with no account, no key and no address outside loopback. "
            + $"These reach for one: {string.Join("; ", offenders)}.");
    }

    /// <summary>Every demo is in the solution, so the build compiles it on every change.</summary>
    /// <remarks>
    /// A demo outside the solution is a demo nobody builds, and the way that fails is silent: it
    /// stops compiling months after the API it uses changed, and the first person to find out is a
    /// reader running it.
    /// </remarks>
    [Fact]
    public void Every_demo_is_in_the_solution()
    {
        var solution = System.IO.File.ReadAllText(Path.Combine(RepositoryRoot.FullPath, "Gradio.Net.sln"));

        foreach (var demo in DemoDirectories())
        {
            var project = $@"Demo\{demo}\{demo}.csproj";

            Assert.True(
                solution.Contains(project, StringComparison.OrdinalIgnoreCase),
                $"{project} is not in Gradio.Net.sln, so the build never compiles it.");
        }
    }

    /// <summary>Every demo runs the package in this tree rather than a published one.</summary>
    /// <remarks>
    /// The demos are part of how this repository explains itself, and a demo that referenced a
    /// published package would be explaining the last release rather than the code beside it.
    /// </remarks>
    [Fact]
    public void Every_demo_references_the_package_in_this_tree()
    {
        foreach (var demo in DemoDirectories())
        {
            var project = System.IO.File.ReadAllText(Path.Combine(_demos, demo, demo + ".csproj"));

            Assert.True(
                project.Contains(@"src\Gradio.Net\Gradio.Net.csproj", StringComparison.Ordinal),
                $"Demo/{demo} does not reference src/Gradio.Net/Gradio.Net.csproj, so it does not "
                + "demonstrate the code in this tree.");
        }
    }

    /// <summary>The demos, by directory name.</summary>
    /// <returns>One name per directory under <c>Demo/</c>.</returns>
    private static IReadOnlyList<string> DemoDirectories() =>
        [.. Directory.EnumerateDirectories(_demos)
            .Select(Path.GetFileName)
            .OfType<string>()
            .OrderBy(name => name, StringComparer.Ordinal)];

    /// <summary>The demos the index links to, in the order it links to them.</summary>
    /// <returns>One name per <c>](Name/)</c> link in <c>Demo/README.md</c>.</returns>
    private static IReadOnlyList<string> LinkedDemoNames() =>
        [.. _demoLink.Matches(System.IO.File.ReadAllText(_readme))
            .Select(match => match.Groups["name"].Value)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.Ordinal)];

    /// <summary>Every C# file under <c>Demo/</c>.</summary>
    /// <returns>The files, with the path a message should name them by.</returns>
    private static IReadOnlyList<(string Absolute, string Relative)> SourceFiles() =>
        [.. RepositoryTree
            .EnumerateFiles(["*.cs"], skippedSubtrees: [])
            .Where(path => path.StartsWith(_demos + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            .Select(path => (Absolute: path, Relative: Path.GetRelativePath(RepositoryRoot.FullPath, path)))
            .OrderBy(file => file.Relative, StringComparer.Ordinal)];

    /// <summary>The text of every string literal in a file, interpolated pieces included.</summary>
    /// <param name="root">The file's syntax tree root.</param>
    /// <returns>The literals' text, in the order they appear.</returns>
    private static IEnumerable<string> Literals(SyntaxNode root) =>
        root.DescendantNodes()
            .Select(node => node switch
            {
                LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression) =>
                    literal.Token.ValueText,
                InterpolatedStringTextSyntax text => text.TextToken.ValueText,
                _ => null,
            })
            .OfType<string>();
}
