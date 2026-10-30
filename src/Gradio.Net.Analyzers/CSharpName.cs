namespace Gradio.Net.Analyzers;

/// <summary>
/// The one spelling rule both generators use: a Python parameter's name, written as the member
/// that holds it.
/// </summary>
/// <remarks>
/// The Python version spells a parameter <c>snake_case</c> and this framework spells a member
/// <c>PascalCase</c>, so the two generators that map one onto the other would otherwise carry a
/// copy each — and two copies of a spelling rule is how a parameter ends up mapping to a member in
/// one generator and to nothing in the other.
/// </remarks>
internal static class CSharpName
{
    /// <summary>Spells a Python parameter name the way this framework spells a member.</summary>
    /// <param name="name">The parameter's name, as the Python version spells it.</param>
    /// <returns>The member's name: each part capitalized, the underscores gone.</returns>
    /// <remarks>
    /// The Python version's names are almost all <c>snake_case</c>, and the ones that are not are
    /// already capitalized, so raising the first letter of each part covers both without asking
    /// which shape it was.
    /// </remarks>
    internal static string PascalCase(string name)
    {
        var builder = new System.Text.StringBuilder(name.Length);
        var upper = true;

        foreach (var character in name)
        {
            if (character == '_')
            {
                upper = true;
                continue;
            }

            builder.Append(upper ? char.ToUpperInvariant(character) : character);
            upper = false;
        }

        return builder.ToString();
    }

    /// <summary>Spells a Python parameter name the way this framework spells a parameter.</summary>
    /// <param name="name">The parameter's name, as the Python version spells it.</param>
    /// <returns>The parameter's name: each part capitalized but the first, the underscores gone.</returns>
    /// <remarks>
    /// The factory's parameters are the C# spelling of the Python version's, so <c>elem_id</c>
    /// arrives as <c>elemId</c>. A name that is already one word keeps its case, and a name that
    /// would be a keyword is written with the <c>@</c> that makes it an identifier.
    /// </remarks>
    internal static string CamelCase(string name)
    {
        var pascal = PascalCase(name);
        if (pascal.Length == 0)
        {
            return pascal;
        }

        var camel = char.ToLowerInvariant(pascal[0]) + pascal.Substring(1);

        return IsKeyword(camel) ? "@" + camel : camel;
    }

    /// <summary>Whether a name is one of C#'s reserved words.</summary>
    /// <param name="name">The candidate identifier.</param>
    /// <returns>Whether it has to be written with a leading <c>@</c>.</returns>
    private static bool IsKeyword(string name) => name switch
    {
        "abstract" or "as" or "base" or "bool" or "break" or "byte" or "case" or "catch" or "char"
            or "checked" or "class" or "const" or "continue" or "decimal" or "default" or "delegate"
            or "do" or "double" or "else" or "enum" or "event" or "explicit" or "extern" or "false"
            or "finally" or "fixed" or "float" or "for" or "foreach" or "goto" or "if" or "implicit"
            or "in" or "int" or "interface" or "internal" or "is" or "lock" or "long" or "namespace"
            or "new" or "null" or "object" or "operator" or "out" or "override" or "params"
            or "private" or "protected" or "public" or "readonly" or "ref" or "return" or "sbyte"
            or "sealed" or "short" or "sizeof" or "stackalloc" or "static" or "string" or "struct"
            or "switch" or "this" or "throw" or "true" or "try" or "typeof" or "uint" or "ulong"
            or "unchecked" or "unsafe" or "ushort" or "using" or "virtual" or "void" or "volatile"
            or "while" => true,
        _ => false,
    };
}
