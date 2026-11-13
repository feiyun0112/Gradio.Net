using System.Reflection;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// Why there is no <c>gr.Draggable()</c>, and why that is correct rather than a missing entry point.
/// </summary>
/// <remarks>
/// The Python wheel ships the class in <c>gradio.layouts.draggable</c> and still does not put a
/// <c>Draggable</c> name in <c>gradio</c>'s top-level exports. A compatibility layer that added the
/// factory would be offering a name an application could not have used in Python. The type stays
/// public so the frontend's component description can be produced from it, but it has no public
/// constructor, because a layout is normally built by a factory that also enters its render scope.
/// </remarks>
public sealed class DraggableSurfaceTests
{
    [Fact]
    public void The_python_version_does_not_export_Draggable()
    {
        Assert.False(UpstreamDeclaration.Exports().ContainsKey("Draggable"));
        Assert.DoesNotContain("Draggable", GrSurface.UpstreamNames());
    }

    [Fact]
    public void The_layer_offers_no_Draggable_factory()
    {
        Assert.DoesNotContain("Draggable", GrSurface.LayerMembers());
    }

    [Fact]
    public void The_type_is_described_but_cannot_be_constructed_by_an_application()
    {
        Assert.True(typeof(Draggable).IsPublic);
        Assert.Empty(typeof(Draggable).GetConstructors(BindingFlags.Public | BindingFlags.Instance));
    }

    [Fact]
    public void The_layout_is_still_one_the_python_wheel_declares()
    {
        var declared = UpstreamDeclaration.Parameters()
            .Any(block => string.Equals(block.Block, "Draggable", StringComparison.Ordinal));

        Assert.True(declared, "the wheel's component manifest no longer declares Draggable");
    }
}
