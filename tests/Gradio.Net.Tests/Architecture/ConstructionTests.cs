using System.Reflection;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Architecture;

/// <summary>
/// A component is built by its factory and by nothing else.
/// </summary>
/// <remarks>
/// <para>
/// <c>gr.Textbox(...)</c> is the one way to put a textbox on a page: it builds the block, places it
/// on the page being written, and hands it back. A public constructor would be a second way — one
/// that builds a block without placing it, which is the shape the framework is written to avoid —
/// so every component's constructor is internal and this test is what keeps it that way.
/// </para>
/// <para>
/// The base classes are the exception, and they are not components: <see cref="Html"/> is what a
/// custom component derives from, so its constructor is <c>protected internal</c> — reachable from
/// a derived class, not from a caller.
/// </para>
/// </remarks>
public sealed class ConstructionTests
{
    [Fact]
    public void Every_component_is_built_by_its_factory_rather_than_by_new()
    {
        var offenders = typeof(gr).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && !type.IsNested)
            .Where(type => typeof(Block).IsAssignableFrom(type) || typeof(Blocks).IsAssignableFrom(type))
            .Where(type => type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length > 0)
            .Select(type => type.Name)
            .Order(StringComparer.Ordinal)
            .ToList();

        Assert.True(
            offenders.Count == 0,
            $"These types can be built with `new`, which is the second way to make a component: "
            + $"{string.Join(", ", offenders)}. A component is built by its factory.");
    }

}
