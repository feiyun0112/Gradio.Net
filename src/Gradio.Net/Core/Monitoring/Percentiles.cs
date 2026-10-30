namespace Gradio.Net;
/// <summary>
/// The percentile both the run summary and the profiling summary report, computed the way the
/// Python version gets it from numpy.
/// </summary>
/// <remarks>
/// numpy's default is the linear method: rank the values, then read between the two neighbours of
/// the fractional position. Reproducing it here rather than inventing a rounding rule is what makes
/// the numbers comparable with the Python version's — the panel shows them, and an operator who
/// moves from one to the other should not see the same run summarised differently.
/// </remarks>
internal static class Percentiles
{
    /// <summary>One percentile of a sorted set of values.</summary>
    /// <param name="sorted">The values, ascending.</param>
    /// <param name="percent">Which percentile, 0 to 100.</param>
    /// <returns>The value, or <see langword="null"/> for an empty set.</returns>
    internal static double? At(IReadOnlyList<double> sorted, double percent)
    {
        if (sorted.Count == 0)
        {
            return null;
        }

        var rank = (sorted.Count - 1) * percent / 100.0;
        var low = (int)Math.Floor(rank);
        var high = (int)Math.Ceiling(rank);

        return sorted[low] + ((sorted[high] - sorted[low]) * (rank - low));
    }
}
