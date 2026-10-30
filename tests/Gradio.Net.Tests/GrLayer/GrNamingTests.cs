using System.Reflection;
using System.Text.RegularExpressions;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// The naming exemption, held to the one thing it is for.
/// </summary>
/// <remarks>
/// <para>
/// <c>.editorconfig</c> switches two rules off for this directory — CA1707, which objects to
/// underscores, and CS8981, which objects to an all-lower-case type name. Both are needed and both
/// are narrow, and this is what keeps them narrow: the exemption exists for the *parameters*, whose
/// names are the Python version's, and for the one type named after the Python version's module.
/// Everything else in the layer is spelled the way the rest of the framework is.
/// </para>
/// <para>
/// Without this, a directory with the naming rules off would slowly become the place to put anything
/// the rules objected to — a public helper called <c>helper_class</c> would compile.
/// </para>
/// </remarks>
public sealed class GrNamingTests
{
    /// <summary>The directory the exemption is scoped to, repository-relative.</summary>
    private const string Directory = "src/Gradio.Net/Gr/";

    private static readonly Regex _publicType = new(
        @"\bpublic\s+(?:static\s+|sealed\s+|abstract\s+|partial\s+|readonly\s+)*(?:class|record|struct|enum|interface)\s+(\w+)",
        RegexOptions.CultureInvariant);

    [Fact]
    public void The_entry_point_is_the_one_lower_case_name_and_it_is_spelled_like_upstream()
    {
        // The name is the point: a port writes `gr.Textbox(...)` because the Python wrote
        // `gr.Textbox(...)`. `Gr` would be a name to remember.
        Assert.Equal("gr", typeof(gr).Name);
        Assert.Equal("Gradio.Net", typeof(gr).Namespace);
    }

    [Fact]
    public void The_layer_exposes_one_entry_point_and_the_one_type_a_module_is_reached_through()
    {
        // The exemption covers a directory, so what may be declared in it is the one name the
        // exemption is for — plus the type a module is reached through. `gr.Themes` is the Python version's
        // `gr.themes`, and its type exists only so that `gr.Themes.Soft()` can be a member access:
        // a port never writes that name, and every member on it is held to the same rules as `gr`'s
        // (no underscores, the Python version's word with its first letter raised).
        var declared = Files()
            .SelectMany(file => _publicType.Matches(System.IO.File.ReadAllText(file)).Select(match => match.Groups[1].Value))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToList();

        Assert.Equal(["ThemesModule", "gr"], declared);

        // And it is reached only through `gr`: a module type a caller could construct would be a
        // second way to write the same thing.
        Assert.Contains(
            typeof(gr).GetProperties(BindingFlags.Public | BindingFlags.Static),
            property => property.PropertyType == typeof(ThemesModule));
    }

    [Fact]
    public void No_member_of_the_layer_carries_an_underscore()
    {
        var offenders = typeof(gr)
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName)
            .Select(method => method.Name)
            .Where(name => name.Contains('_', StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        Assert.True(
            offenders.Count == 0,
            $"These members carry an underscore: {string.Join(", ", offenders)}. A member's name is the Python version's "
            + "word with its first letter raised; the underscores belong to the parameters.");
    }

    [Fact]
    public void Every_parameter_is_the_python_name_in_camel_case()
    {
        // The parameters are the Python version's, spelled the C# way: `elem_id` arrives as
        // `elemId`. So a parameter never carries an underscore, and never starts with a capital —
        // the two things a name would have if it were copied from the Python version as it stands.
        var parameters = typeof(gr)
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(method => method.GetParameters())
            .Select(parameter => parameter.Name!)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var underscored = parameters.Where(name => name.Contains('_', StringComparison.Ordinal)).ToList();
        Assert.True(
            underscored.Count == 0,
            $"These parameters still carry the Python version's underscores: {string.Join(", ", underscored)}.");

        var capitalised = parameters
            .Where(name => name.Length > 0 && char.IsUpper(name[0]))
            .ToList();
        Assert.True(
            capitalised.Count == 0,
            $"These parameters are PascalCase rather than camelCase: {string.Join(", ", capitalised)}.");

        // The control: the list is not empty, so a filter that matched nothing would not pass for
        // the same reason a filter that matched everything would.
        Assert.NotEmpty(parameters);
    }

    private static IEnumerable<string> Files()
    {
        var root = Path.Combine(RepositoryRoot.FullPath, Directory.Replace('/', Path.DirectorySeparatorChar));

        Assert.True(System.IO.Directory.Exists(root), $"The layer's directory '{Directory}' is gone.");

        return System.IO.Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories);
    }
}
