using System.Collections.Concurrent;
using System.Diagnostics;
using System.Web;
using Gradio.Net.Core;
using Gradio.Net.Utils;


namespace Gradio.Net;

public class ProcessTime
{
    public float ProcessTimeValue { get; set; }
    public int Count { get; set; }
    public float AvgTime { get; set; }

    public ProcessTime()
    {
        ProcessTimeValue = 0;
        Count = 0;
        AvgTime = 0;
    }

    public void Add(float time)
    {
        ProcessTimeValue += time;
        Count += 1;
        AvgTime = ProcessTimeValue / Count;
    }
}
