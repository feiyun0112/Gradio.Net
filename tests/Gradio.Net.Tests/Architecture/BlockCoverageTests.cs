using System.Text.RegularExpressions;
using Xunit;

namespace Gradio.Net.Tests.Architecture;

/// <summary>
/// Every block the Python version declares is a block this build has, and no block this build has
/// is one the Python version does not.
/// </summary>
/// <remarks>
/// <para>
/// The parameter comparison (<c>GrParametersTests</c>) starts from a block this build declares and
/// asks whether it has the Python version's parameters. That leaves the other direction open: a
/// block the Python version declares and this build never wrote is a gap no comparison sees,
/// because there is nothing to compare it against. One was missing — <c>simpleimage</c> — and it
/// was found by counting, not by a test, which is what this file is for.
/// </para>
/// <para>
/// The declaration is read from the source rather than from the assembly because the block's name is
/// carried by an internal attribute: the annotation is what the props generator reads at build
/// time, so it is the annotation that has to agree with the manifest.
/// </para>
/// </remarks>
public sealed partial class BlockCoverageTests
{
    /// <summary>
    /// Blocks in the manifest that are base classes rather than blocks an application can write.
    /// </summary>
    /// <remarks>
    /// The Python version's extractor records every class with a constructor in
    /// <c>gradio/components</c>, which includes <c>FormComponent</c> — the base of every field. It
    /// is abstract there and abstract here, so there is nothing for an application to build and
    /// nothing to declare.
    /// </remarks>
    private static readonly string[] _bases = ["formcomponent"];

    [Fact]
    public void Every_block_upstream_declares_is_a_block_this_build_has()
    {
        var declared = DeclaredBlocks();
        var upstream = UpstreamBlocks();

        Assert.NotEmpty(declared);

        var missing = upstream.Except(declared).Except(_bases).Order(StringComparer.Ordinal).ToList();

        Assert.True(
            missing.Count == 0,
            $"The Python version declares {missing.Count} block(s) this build does not have: {string.Join(", ", missing)}. "
            + "A block nobody wrote is a page an application cannot build, and no parameter comparison "
            + "can see it.");
    }

    [Fact]
    public void This_build_declares_no_block_upstream_does_not()
    {
        var upstream = UpstreamBlocks();

        var invented = DeclaredBlocks().Except(upstream).Order(StringComparer.Ordinal).ToList();

        Assert.True(
            invented.Count == 0,
            $"This build declares {invented.Count} block(s) the Python version does not have: {string.Join(", ", invented)}. "
            + "A block invented here is a config the frontend has never been taught to draw.");
    }

    /// <summary>The block names the manifest records, as the config spells them.</summary>
    /// <returns>One name per block.</returns>
    private static HashSet<string> UpstreamBlocks() =>
        UpstreamDeclaration
            .Read("component-params.json")["blocks"]!
            .AsObject()
            .Select(pair => pair.Key)
            .ToHashSet(StringComparer.Ordinal);

    /// <summary>The block names the package's source declares.</summary>
    /// <returns>One name per <c>[GradioBlock("…")]</c> annotation.</returns>
    private static HashSet<string> DeclaredBlocks()
    {
        var declared = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in RepositoryTree.EnumerateFiles(["*.cs"], skippedSubtrees: ["tests", "Demo"]))
        {
            if (!file.Contains(Path.DirectorySeparatorChar + "src" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            {
                continue;
            }

            foreach (Match match in BlockAnnotation().Matches(System.IO.File.ReadAllText(file)))
            {
                declared.Add(match.Groups["block"].Value);
            }
        }

        return declared;
    }

    /// <summary>The annotation that names the Python block a class implements.</summary>
    [GeneratedRegex("""GradioBlock\("(?<block>[a-z0-9_]+)"\)""", RegexOptions.CultureInvariant)]
    private static partial Regex BlockAnnotation();
}
