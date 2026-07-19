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


public class TrackedIterable
{
    public IEnumerator<object>? Iterable { get; set; }
    public double? Index { get; set; }
    public double? Length { get; set; }
    public string? Desc { get; set; }
    public string? Unit { get; set; }
    public object? Tqdm { get; set; }
    public double? Progress { get; set; }

    public TrackedIterable(
        IEnumerator<object>? iterable,
        double? index,
        double? length,
        string? desc,
        string? unit,
        object? tqdm,
        double? progress = null
    )
    {
        Iterable = iterable;
        Index = index;
        Length = length;
        Desc = desc;
        Unit = unit;
        Tqdm = tqdm;
        Progress = progress;
    }

    public bool HasNext => Iterable != null;

    public object Next()
    {
        if (Iterable == null)
        {
            throw new InvalidOperationException("No iterable to advance");
        }

        if (!Iterable.MoveNext())
        {
            throw new InvalidOperationException("End of iteration");
        }

        return Iterable.Current;
    }
}
