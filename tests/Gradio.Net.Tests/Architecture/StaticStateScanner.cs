using System.Reflection;
using System.Runtime.CompilerServices;

namespace Gradio.Net.Tests.Architecture;

/// <summary>
/// Finds static mutable fields in a set of compiled types.
/// </summary>
/// <remarks>
/// <para>
/// This is the assembly-level half of the "no static mutable state" rule whose source-level half is
/// the build's GRADIO0001 analyzer. Both exist on purpose: the analyzer rejects the declaration, the
/// architecture test proves what was actually emitted, so the rule survives a change to the
/// analyzer's severity and covers types the analyzer never sees.
/// </para>
/// <para>
/// Only fields are inspected. A static property with a setter and a static event both get a static
/// mutable backing field, so the field rule already reports those shapes — by the name of the field
/// they mutate. A <c>static readonly</c> reference to a mutable object is invisible to reflection and
/// to the analyzer alike; that gap is recorded in the analyzer's own remarks.
/// </para>
/// <para>
/// Compiler-generated types are skipped. Roslyn caches a static lambda in a lazily assigned static
/// field on a generated holder type, which is compiler infrastructure rather than framework state —
/// and the analyzer never sees it either, because generated-code analysis is switched off.
/// </para>
/// </remarks>
internal static class StaticStateScanner
{
    private const BindingFlags StaticFields =
        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    internal static IReadOnlyList<string> FindStaticMutableFields(IEnumerable<Type> types)
    {
        var offenders = new List<string>();

        foreach (var type in types)
        {
            if (type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false))
            {
                continue;
            }

            foreach (var field in type.GetFields(StaticFields))
            {
                if (!field.IsLiteral && !field.IsInitOnly)
                {
                    offenders.Add($"{type.FullName}.{field.Name}");
                }
            }
        }

        return offenders;
    }
}
