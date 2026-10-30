namespace Gradio.Net;
/// <summary>
/// How much of a file an edit changed, counted the way the Python version counts it.
/// </summary>
/// <remarks>
/// <para>
/// The editor answers an edit with two numbers — lines added and lines removed — which the panel
/// shows beside the message that made the change. The Python version gets them by running Python's
/// <c>difflib.unified_diff</c> over the two versions of the file and counting the lines that begin
/// with <c>+</c> or <c>-</c>; with no context lines, that count is exactly the number of lines the
/// edit's opcodes say were inserted and deleted.
/// </para>
/// <para>
/// So the numbers are <c>difflib</c>'s, which means the matching is <c>SequenceMatcher</c>'s: the
/// longest matching block is found first, the two sides of it are searched the same way, and blocks
/// that end up adjacent are merged. That is not a plain longest-common-subsequence diff — it is a
/// recursive one that prefers the earliest, longest run — and on a file whose lines repeat it can
/// count differently. Porting it rather than approximating it is what makes the two numbers the same
/// number, so this is a port rather than a diff: the algorithm, its tie-breaking, and its
/// "popular line" heuristic for long inputs are all kept.
/// </para>
/// <para>
/// What is dropped is the junk function, because the Python version passes none:
/// <c>unified_diff</c> is called without <c>isjunk</c>, so no element is ever junk and the two
/// extension passes that exist to grow a block over junk can never run.
/// </para>
/// </remarks>
internal static class VibeDiff
{
    /// <summary>What one line is, as an edit sees it.</summary>
    private enum Kind
    {
        /// <summary>A line both versions have, in the same order.</summary>
        Equal,

        /// <summary>Lines the first version has where the second has others.</summary>
        Replace,

        /// <summary>Lines only the first version has.</summary>
        Delete,

        /// <summary>Lines only the second version has.</summary>
        Insert,
    }

    /// <summary>One run of lines, and what happened to it.</summary>
    /// <param name="Kind">What happened.</param>
    /// <param name="First">Where the run starts in the first version.</param>
    /// <param name="FirstEnd">Where it ends there.</param>
    /// <param name="Second">Where the run starts in the second version.</param>
    /// <param name="SecondEnd">Where it ends there.</param>
    /// <remarks>
    /// <c>difflib</c>'s opcode, which is a five-tuple there and a type here: the four positions are
    /// meaningless without the kind that says which of them are half-open ranges of the same thing.
    /// </remarks>
    private readonly record struct Opcode(
        Kind Kind,
        int First,
        int FirstEnd,
        int Second,
        int SecondEnd);

    /// <summary>Counts the lines an edit added and removed.</summary>
    /// <param name="original">The file before the edit.</param>
    /// <param name="updated">The file after it.</param>
    /// <returns>How many lines were added, and how many were removed.</returns>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    internal static (int Added, int Removed) Counts(string original, string updated)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(updated);

        var before = SplitLines(original);
        var after = SplitLines(updated);
        var added = 0;
        var removed = 0;

        foreach (var opcode in Opcodes(before, after))
        {
            switch (opcode.Kind)
            {
                case Kind.Replace:
                    removed += opcode.FirstEnd - opcode.First;
                    added += opcode.SecondEnd - opcode.Second;
                    break;

                case Kind.Delete:
                    removed += opcode.FirstEnd - opcode.First;
                    break;

                case Kind.Insert:
                    added += opcode.SecondEnd - opcode.Second;
                    break;

                default:
                    break;
            }
        }

        return (added, removed);
    }

    /// <summary>Splits text into lines the way Python's <c>str.splitlines</c> does.</summary>
    /// <param name="text">The text to split.</param>
    /// <returns>The lines, each keeping the break that ended it.</returns>
    /// <remarks>
    /// <para>
    /// The breaks are kept because a diff compares lines as they were written: <c>"x\n"</c> and
    /// <c>"x"</c> are two different lines to <c>difflib</c>, and dropping the endings would report a
    /// file whose last newline was added as unchanged.
    /// </para>
    /// <para>
    /// The set of breaks is Python's, which is wider than the one .NET's readers know: besides
    /// <c>\n</c> and <c>\r\n</c> it counts <c>\r</c>, the vertical tab, the form feed, the three
    /// separators above them, the next-line character and the two Unicode line separators.
    /// </para>
    /// </remarks>
    private static List<string> SplitLines(string text)
    {
        var lines = new List<string>();
        var start = 0;
        var index = 0;

        while (index < text.Length)
        {
            var width = BreakAt(text, index);

            if (width == 0)
            {
                index++;
                continue;
            }

            lines.Add(text[start..(index + width)]);
            index += width;
            start = index;
        }

        if (start < text.Length)
        {
            lines.Add(text[start..]);
        }

        return lines;
    }

    /// <summary>How long the line break starting at an index is.</summary>
    /// <param name="text">The text being split.</param>
    /// <param name="index">Where to look.</param>
    /// <returns>The break's length, or zero when the character there does not break a line.</returns>
    private static int BreakAt(string text, int index) => text[index] switch
    {
        '\r' => index + 1 < text.Length && text[index + 1] == '\n' ? 2 : 1,
        '\n' or '\v' or '\f' or '\x1c' or '\x1d' or '\x1e' or '\x85' or '\u2028' or '\u2029' => 1,
        _ => 0,
    };

    /// <summary>The opcodes of a diff, as <c>SequenceMatcher.get_opcodes</c> gives them.</summary>
    /// <param name="before">The lines of the first version.</param>
    /// <param name="after">The lines of the second version.</param>
    /// <returns>One entry per run of lines, in order.</returns>
    private static List<Opcode> Opcodes(List<string> before, List<string> after)
    {
        var matcher = new Matcher(before, after);
        var opcodes = new List<Opcode>();
        var first = 0;
        var second = 0;

        foreach (var (firstStart, secondStart, size) in matcher.MatchingBlocks())
        {
            var kind = (firstStart > first, secondStart > second) switch
            {
                (true, true) => Kind.Replace,
                (true, false) => Kind.Delete,
                (false, true) => Kind.Insert,
                _ => Kind.Equal,
            };

            if (kind != Kind.Equal)
            {
                opcodes.Add(new Opcode(kind, first, firstStart, second, secondStart));
            }

            first = firstStart + size;
            second = secondStart + size;

            if (size > 0)
            {
                opcodes.Add(new Opcode(Kind.Equal, firstStart, first, secondStart, second));
            }
        }

        return opcodes;
    }

    /// <summary>
    /// Python's <c>SequenceMatcher</c>, as far as a line count needs it.
    /// </summary>
    /// <remarks>
    /// The state is the second version's lines indexed by their text, which is what makes finding a
    /// match a lookup rather than a scan. Two of the original's decisions are visible in the results
    /// and are therefore kept: a tie is broken towards the earliest place in the first version and
    /// then the earliest in the second, and a line that occurs in more than one per cent of a long
    /// second version is treated as too common to anchor a match on.
    /// </remarks>
    private sealed class Matcher
    {
        /// <summary>How long the second version must be before its popular lines are ignored.</summary>
        private const int AutojunkThreshold = 200;

        private readonly List<string> _before;
        private readonly List<string> _after;
        private readonly Dictionary<string, List<int>> _afterIndex;

        internal Matcher(List<string> before, List<string> after)
        {
            _before = before;
            _after = after;
            _afterIndex = Index(after);
        }

        /// <summary>The blocks both versions have in common, in order, ending with a zero-length one.</summary>
        /// <returns>The blocks, as a start in each version and a length.</returns>
        /// <remarks>
        /// The zero-length block at the end is <c>difflib</c>'s sentinel: it is what makes the opcode
        /// walk finish the run that follows the last match, and it is why the walk needs no special
        /// case for a diff whose last line changed.
        /// </remarks>
        internal List<(int First, int Second, int Size)> MatchingBlocks()
        {
            var pending = new List<(int First, int FirstEnd, int Second, int SecondEnd)>
            {
                (0, _before.Count, 0, _after.Count),
            };

            var found = new List<(int First, int Second, int Size)>();

            // The original pops the last region it queued, so the two halves of a split are searched
            // in the opposite order to the one they were queued in. It does not change which blocks
            // are found — they are sorted below — but it is what the original does.
            while (pending.Count > 0)
            {
                var (first, firstEnd, second, secondEnd) = pending[^1];
                pending.RemoveAt(pending.Count - 1);

                var (start, secondStart, size) = LongestMatch(first, firstEnd, second, secondEnd);

                if (size == 0)
                {
                    continue;
                }

                found.Add((start, secondStart, size));

                if (first < start && second < secondStart)
                {
                    pending.Add((first, start, second, secondStart));
                }

                if (start + size < firstEnd && secondStart + size < secondEnd)
                {
                    pending.Add((start + size, firstEnd, secondStart + size, secondEnd));
                }
            }

            found.Sort();

            var merged = new List<(int First, int Second, int Size)>();
            var (keptFirst, keptSecond, keptSize) = (0, 0, 0);

            foreach (var (first, second, size) in found)
            {
                if (keptFirst + keptSize == first && keptSecond + keptSize == second)
                {
                    keptSize += size;
                }
                else
                {
                    if (keptSize > 0)
                    {
                        merged.Add((keptFirst, keptSecond, keptSize));
                    }

                    (keptFirst, keptSecond, keptSize) = (first, second, size);
                }
            }

            if (keptSize > 0)
            {
                merged.Add((keptFirst, keptSecond, keptSize));
            }

            merged.Add((_before.Count, _after.Count, 0));

            return merged;
        }

        /// <summary>The second version's lines, indexed by their text.</summary>
        /// <param name="lines">The lines.</param>
        /// <returns>The index, without the lines that are too common to be worth matching on.</returns>
        private static Dictionary<string, List<int>> Index(List<string> lines)
        {
            var index = new Dictionary<string, List<int>>(StringComparer.Ordinal);

            for (var position = 0; position < lines.Count; position++)
            {
                if (!index.TryGetValue(lines[position], out var positions))
                {
                    positions = [];
                    index[lines[position]] = positions;
                }

                positions.Add(position);
            }

            if (lines.Count >= AutojunkThreshold)
            {
                var tooCommon = (lines.Count / 100) + 1;

                foreach (var (text, positions) in index.ToList())
                {
                    if (positions.Count > tooCommon)
                    {
                        index.Remove(text);
                    }
                }
            }

            return index;
        }

        /// <summary>The longest run of lines the two ranges share.</summary>
        /// <param name="first">Where the first range starts.</param>
        /// <param name="firstEnd">Where it ends.</param>
        /// <param name="second">Where the second range starts.</param>
        /// <param name="secondEnd">Where it ends.</param>
        /// <returns>Where the run starts in each range, and how long it is.</returns>
        private (int First, int Second, int Size) LongestMatch(
            int first,
            int firstEnd,
            int second,
            int secondEnd)
        {
            var (bestFirst, bestSecond, bestSize) = (first, second, 0);
            var previous = new Dictionary<int, int>();

            for (var position = first; position < firstEnd; position++)
            {
                var current = new Dictionary<int, int>();

                if (!_afterIndex.TryGetValue(_before[position], out var candidates))
                {
                    previous = current;
                    continue;
                }

                foreach (var candidate in candidates)
                {
                    if (candidate < second)
                    {
                        continue;
                    }

                    if (candidate >= secondEnd)
                    {
                        break;
                    }

                    var size = (previous.TryGetValue(candidate - 1, out var carried) ? carried : 0) + 1;
                    current[candidate] = size;

                    if (size > bestSize)
                    {
                        (bestFirst, bestSecond, bestSize) = (position - size + 1, candidate - size + 1, size);
                    }
                }

                previous = current;
            }

            // A run found in the middle of a longer one is grown backwards and forwards to its real
            // ends, which is what makes the block the longest one rather than the longest one seen
            // from the position that happened to be scanning.
            while (bestFirst > first
                && bestSecond > second
                && _before[bestFirst - 1] == _after[bestSecond - 1])
            {
                (bestFirst, bestSecond, bestSize) = (bestFirst - 1, bestSecond - 1, bestSize + 1);
            }

            while (bestFirst + bestSize < firstEnd
                && bestSecond + bestSize < secondEnd
                && _before[bestFirst + bestSize] == _after[bestSecond + bestSize])
            {
                bestSize++;
            }

            return (bestFirst, bestSecond, bestSize);
        }
    }
}
