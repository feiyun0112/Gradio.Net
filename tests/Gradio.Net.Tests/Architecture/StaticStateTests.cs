using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Architecture;

/// <summary>
/// The framework keeps no static mutable state, asserted against the assembly rather than against
/// the source: the render context is an AsyncLocal on a service, the session store is a service,
/// and nothing else is process-wide.
/// </summary>
/// <remarks>
/// There is one field this scan cannot judge and that the rule still has to account for: the
/// compatibility layer's ambient render stack (<c>Gr/Ambient.cs</c>), a <c>static readonly
/// AsyncLocal</c>. It is a per-flow value on a process-wide field — the shape the layer needs to
/// make <c>gr.Textbox(...)</c> a line on the page. A <c>static readonly</c> reference is invisible
/// here and to GRADIO0001, which is exactly why the decision is written down rather than left to a
/// scanner's silence.
/// </remarks>
public sealed class StaticStateTests
{
    private static readonly Assembly _package = typeof(GradioVersion).Assembly;

    [Fact]
    public void The_package_declares_no_static_mutable_fields()
    {
        var types = _package.GetTypes();
        var offenders = StaticStateScanner.FindStaticMutableFields(types);

        // The rule only means something if the scan is looking at the framework's own types.
        Assert.Contains(typeof(RenderContextAccessor), types);
        Assert.Contains(typeof(MemorySessionStore), types);

        Assert.True(
            offenders.Count == 0,
            "Static mutable state stops two hosts from coexisting in one process and breaks concurrent " +
            "rendering; take it from dependency injection instead. Offenders: " +
            string.Join(", ", offenders));
    }

    [Fact]
    public void Static_const_fields_are_not_mistaken_for_state()
    {
        var type = EmitType(field =>
        {
            var constant = field.DefineField(
                "Version",
                typeof(string),
                FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal);
            constant.SetConstant("6.30.0");
        });

        Assert.Empty(StaticStateScanner.FindStaticMutableFields([type]));
    }

    [Fact]
    public void Static_readonly_fields_are_not_mistaken_for_state()
    {
        var type = EmitType(field => field.DefineField(
            "Cached",
            typeof(int),
            FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.InitOnly));

        Assert.Empty(StaticStateScanner.FindStaticMutableFields([type]));
    }

    [Fact]
    public void Compiler_generated_holders_are_not_mistaken_for_state()
    {
        // Roslyn caches static lambdas in lazily assigned static fields on a generated holder type.
        var type = EmitType(
            field => field.DefineField("Cached", typeof(int), FieldAttributes.Public | FieldAttributes.Static),
            [new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes)!, [])]);

        Assert.Empty(StaticStateScanner.FindStaticMutableFields([type]));
    }

    [Fact]
    public void The_scanner_detects_a_mutable_static_field()
    {
        var type = EmitType(field => field.DefineField(
            "Counter",
            typeof(int),
            FieldAttributes.Public | FieldAttributes.Static));

        Assert.Equal([$"{type.FullName}.Counter"], StaticStateScanner.FindStaticMutableFields([type]));
    }

    private static Type EmitType(Action<TypeBuilder> define, IReadOnlyList<CustomAttributeBuilder>? attributes = null)
    {
        var assembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName($"StaticStateProbe{Guid.NewGuid():N}"),
            AssemblyBuilderAccess.Run);
        var module = assembly.DefineDynamicModule("StaticStateProbe");
        var type = module.DefineType("StaticStateProbe.Probe", TypeAttributes.Public | TypeAttributes.Class);

        define(type);

        if (attributes is not null)
        {
            foreach (var attribute in attributes)
            {
                type.SetCustomAttribute(attribute);
            }
        }

        return type.CreateType()!;
    }
}
