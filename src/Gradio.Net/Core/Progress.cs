using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Gradio.Net.Components;
using Gradio.Net.Core.Flagging;
using Gradio.Net.Data;
using Gradio.Net.Events;
using Gradio.Net.Utils;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net.Core;

public class Progress : IEnumerable<object>, IEnumerator<object>
{
    public bool TrackTqdm { get; set; }
    public List<TrackedIterable> Iterables { get; set; }

    public Progress(bool trackTqdm = false)
    {
        if (trackTqdm)
        {
            PatchTqdm();
        }

        TrackTqdm = trackTqdm;
        Iterables = new List<TrackedIterable>();
    }

    public int Length
    {
        get
        {
            if (Iterables.Count == 0)
            {
                return 0;
            }
            return (int)(Iterables.Last().Length ?? 0);
        }
    }

    public IEnumerator<object> GetEnumerator()
    {
        return this;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        // Dispose resources if needed
    }

    public object Current { get; private set; }

    public bool MoveNext()
    {
        var callback = ProgressCallback();
        if (callback != null)
        {
            if (Iterables.Count == 0)
            {
                return false;
            }

            var currentIterable = Iterables.Last();

            // Find the last iterable with a next method
            while (!currentIterable.HasNext && Iterables.Count > 0)
            {
                currentIterable = Iterables.Last();
                Iterables.RemoveAt(Iterables.Count - 1);
            }

            if (Iterables.Count == 0)
            {
                return false;
            }

            callback(Iterables);

            if (currentIterable.Index == null)
            {
                throw new IndexOutOfRangeException("Index not set.");
            }

            currentIterable.Index++;

            try
            {
                Current = currentIterable.Next();
                return true;
            }
            catch (InvalidOperationException)
            {
                // End of iteration
                Iterables.RemoveAt(Iterables.Count - 1);
                return false;
            }
        }
        else
        {
            Current = this;
            return true;
        }
    }

    public void Reset()
    {
        Iterables.Clear();
    }

    public void Update(object progress, string? desc = null, double? total = null, string unit = "steps", object? tqdm = null)
    {
        var callback = ProgressCallback();
        if (callback != null)
        {
            double? progressValue = null;
            double? index = null;
            double? length = total;

            if (progress is double doubleProgress)
            {
                progressValue = doubleProgress;
            }
            else if (progress is Tuple<object, object> tupleProgress)
            {
                if (tupleProgress.Item1 is int intIndex)
                {
                    index = intIndex;
                }
                else if (tupleProgress.Item1 is double doubleIndex)
                {
                    index = doubleIndex;
                }

                if (tupleProgress.Item2 is int intLength)
                {
                    length = intLength;
                }
                else if (tupleProgress.Item2 is double doubleLength)
                {
                    length = doubleLength;
                }
            }

            var trackedIterable = new TrackedIterable(
                iterable: null,
                index: index,
                length: length,
                desc: desc,
                unit: unit,
                tqdm: tqdm,
                progress: progressValue
            );

            callback(Iterables.Concat(new[] { trackedIterable }).ToList());
        }
    }

    public void Update(double progress, string? desc = null, double? total = null, string unit = "steps", object? tqdm = null)
    {
        Update((object)progress, desc, total, unit, tqdm);
    }

    public void Update(Tuple<object, object> progress, string? desc = null, string unit = "steps", object? tqdm = null)
    {
        Update((object)progress, desc, null, unit, tqdm);
    }

    public void __call__(object progress, string? desc = null, double? total = null, string unit = "steps", object? tqdm = null)
    {
        Update(progress, desc, total, unit, tqdm);
    }

    public void __call__(double progress, string? desc = null, double? total = null, string unit = "steps", object? tqdm = null)
    {
        Update(progress, desc, total, unit, tqdm);
    }

    public void __call__(Tuple<object, object> progress, string? desc = null, string unit = "steps", object? tqdm = null)
    {
        Update(progress, desc, null, unit, tqdm);
    }

    public Progress Tqdm(IEnumerable<object>? iterable, string? desc = null, double? total = null, string unit = "steps", object? tqdm = null)
    {
        var callback = ProgressCallback();
        if (callback != null)
        {
            if (iterable == null)
            {
                var newIterable = new TrackedIterable(
                    iterable: null,
                    index: 0,
                    length: total,
                    desc: desc,
                    unit: unit,
                    tqdm: tqdm
                );
                Iterables.Add(newIterable);
                callback(Iterables);
                return this;
            }

            var length = iterable is ICollection<object> collection ? collection.Count : total;
            Iterables.Add(new TrackedIterable(
                iterable: iterable.GetEnumerator(),
                index: 0,
                length: length,
                desc: desc,
                unit: unit,
                tqdm: tqdm
            ));

            callback(Iterables);
            return this;
        }

        if (iterable == null)
        {
            return this;
        }

        return this;
    }

    public void Update(int n = 1)
    {
        var callback = ProgressCallback();
        if (callback != null && Iterables.Count > 0)
        {
            var currentIterable = Iterables.Last();
            if (currentIterable.Index == null)
            {
                throw new IndexOutOfRangeException("Index not set.");
            }

            currentIterable.Index += n;
            callback(Iterables);
        }
    }

    public void Close(object tqdm)
    {
        var callback = ProgressCallback();
        if (callback != null)
        {
            for (int i = 0; i < Iterables.Count; i++)
            {
                if (Iterables[i].Tqdm == tqdm)
                {
                    Iterables.RemoveAt(i);
                    break;
                }
            }
            callback(Iterables);
        }
    }

    private Action<List<TrackedIterable>>? ProgressCallback()
    {
        var blocks = LocalContext.Blocks;
        var eventId = LocalContext.EventId;

        if (blocks == null || eventId == null)
        {
            return null;
        }

        return (iterables) =>
        {
            var app = LocalContext.CurrentApp;
            app?.QueueManager?.SetProgress(eventId, iterables);
        };
    }

    public static void PatchTqdm()
    {
        // Note: tqdm is a Python library not directly available in .NET
        // This function is a no-op in the C# implementation
        // In Python, this would monkey-patch tqdm to work with Gradio's progress tracking
    }
}
