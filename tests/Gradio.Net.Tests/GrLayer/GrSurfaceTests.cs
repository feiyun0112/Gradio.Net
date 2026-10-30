using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// The set comparison this layer exists to satisfy, and the ledger that makes it readable.
/// </summary>
/// <remarks>
/// <para>
/// Two questions, and they fail for different reasons. A name the layer offers that the Python
/// version does not is a name this layer invented — the one thing a compatibility layer must never
/// do, because a port that compiled here would not have compiled there. A name the Python version
/// offers that the layer does not is a gap, and gaps are expected today: what must not happen is a
/// gap nobody wrote down.
/// </para>
/// <para>
/// The ledger is exact in both directions on purpose. A name that leaves it without the layer
/// gaining it, or one that joins it without the Python version gaining it, is a mistake in the
/// table rather than a change in the code, and the failure says which.
/// </para>
/// </remarks>
public sealed class GrSurfaceTests
{
    [Fact]
    public void The_layer_offers_no_name_upstream_does_not_have()
    {
        var invented = GrSurface.LayerMembers()
            .Except(GrSurface.UpstreamNames())
            .Except(GrSurface.OwnAdditions.Keys)
            .Order(StringComparer.Ordinal)
            .ToList();

        Assert.True(
            invented.Count == 0,
            $"This layer offers {invented.Count} name(s) the Python version does not export: {string.Join(", ", invented)}. "
            + "A name invented here is a port that compiles and would not have run in Python.");

        // The additions are a table rather than a filter: a name that stops existing has to leave
        // it, and a name that joins it has to be argued for in the file.
        var stale = GrSurface.OwnAdditions.Keys
            .Where(name => !GrSurface.LayerMembers().Contains(name))
            .Order(StringComparer.Ordinal)
            .ToList();

        Assert.True(
            stale.Count == 0,
            $"These names are recorded as additions and the layer does not have them: {string.Join(", ", stale)}.");
    }

    [Fact]
    public void Every_addition_has_a_reason()
    {
        var unreasoned = GrSurface.OwnAdditions
            .Where(pair => pair.Value.Length < 40)
            .Select(pair => pair.Key)
            .ToList();

        Assert.True(unreasoned.Count == 0, $"These additions have no reason: {string.Join(", ", unreasoned)}.");
    }

    [Fact]
    public void The_gaps_are_the_ones_the_ledger_owes()
    {
        var owed = Owed().ToHashSet(StringComparer.Ordinal);
        var missing = GrSurface.UpstreamSurface()
            .Except(GrSurface.LayerMembers())
            .Except(GrSurfaceOwed.Never.Keys)
            .ToHashSet(StringComparer.Ordinal);

        var unlisted = missing.Except(owed).Order(StringComparer.Ordinal).ToList();
        var stale = owed.Except(missing).Order(StringComparer.Ordinal).ToList();

        Assert.True(
            unlisted.Count == 0,
            $"The Python version exports {unlisted.Count} name(s) that this layer does not have and the ledger does not "
            + $"owe: {string.Join(", ", unlisted)}. Add them to the ticket that will build them, or to "
            + $"{nameof(GrSurfaceOwed.Never)} with the reason this layer will not.");

        Assert.True(
            stale.Count == 0,
            $"The ledger owes {stale.Count} name(s) this layer already has, or the Python version no longer exports: "
            + $"{string.Join(", ", stale)}. Delete the row: the list is what is left, not what was.");
    }

    [Fact]
    public void Every_name_the_ledger_owes_is_one_upstream_exports()
    {
        // A typo in the ledger would otherwise sit there for ever: it is not a gap (the Python version does not
        // export it) and it is not a member, so nothing else in this file would notice.
        var upstream = GrSurface.UpstreamSurface();

        var unknown = Owed().Where(name => !upstream.Contains(name)).Order(StringComparer.Ordinal).ToList();

        Assert.True(
            unknown.Count == 0,
            $"The ledger owes {unknown.Count} name(s) the Python version does not export: {string.Join(", ", unknown)}.");
    }

    [Fact]
    public void No_name_is_both_owed_and_never()
    {
        // The two tables answer opposite questions — "not yet" and "not ever" — so a name in both is
        // a name nobody has decided about.
        var both = Owed().Intersect(GrSurfaceOwed.Never.Keys, StringComparer.Ordinal).ToList();

        Assert.True(both.Count == 0, $"These names are both owed and never: {string.Join(", ", both)}.");
    }

    [Fact]
    public void The_vertical_slice_is_in_the_layer_and_the_comparison_saw_the_whole_surface()
    {
        var members = GrSurface.LayerMembers();

        // Without this, every assertion above would pass on an empty layer: nothing invented,
        // nothing owed, nothing missing.
        Assert.Contains(nameof(gr.Textbox), members);
        Assert.Contains(nameof(gr.Button), members);
        Assert.Contains(nameof(gr.Blocks), members);

        Assert.True(
            GrSurface.UpstreamExports().Count >= 100,
            $"The comparison found {GrSurface.UpstreamExports().Count} the Python version export(s), which is far fewer than "
            + "the release declares: the declaration was not read.");
    }

    private static IEnumerable<string> Owed() =>
        GrSurfaceOwed.Components.Concat(GrSurfaceOwed.Layouts).Concat(GrSurfaceOwed.TopLevel);
}
