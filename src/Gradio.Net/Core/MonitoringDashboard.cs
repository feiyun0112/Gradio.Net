using System.Collections.Concurrent;
using System.Diagnostics;

namespace Gradio.Net.Core;

public class MonitoringDashboard
{
    public static readonly ConcurrentDictionary<string, MonitoringData> Data = new();

    public static void Initialize() { }

    public static void LogRequest(string function, string status, double processTime, string sessionHash)
    {
        var data = new MonitoringData
        {
            Time = DateTimeOffset.Now.ToUnixTimeSeconds(),
            Status = status,
            Function = function,
            ProcessTime = processTime,
            SessionHash = sessionHash
        };

        Data.TryAdd(Guid.NewGuid().ToString(), data);

        if (Data.Count > 10000)
        {
            var oldestKey = Data.OrderBy(kv => kv.Value.Time).First().Key;
            Data.TryRemove(oldestKey, out _);
        }
    }

    public static MonitoringStats GetStats(DateTime? start = null, DateTime? end = null, string? selectedFunction = null)
    {
        start ??= DateTime.Now.AddDays(-1);
        end ??= DateTime.Now;

        var startUnix = new DateTimeOffset(start.Value).ToUnixTimeSeconds();
        var endUnix = new DateTimeOffset(end.Value).ToUnixTimeSeconds();

        var filteredData = Data.Values
            .Where(d => d.Time >= startUnix && d.Time <= endUnix)
            .Where(d => selectedFunction == null || selectedFunction == "All" || d.Function == selectedFunction);

        var uniqueUsers = filteredData.Select(d => d.SessionHash).Distinct().Count();
        var totalRequests = filteredData.Count();
        var avgProcessTime = totalRequests > 0 ? filteredData.Average(d => d.ProcessTime) : 0;

        return new MonitoringStats
        {
            UniqueUsers = uniqueUsers,
            TotalRequests = totalRequests,
            AvgProcessTime = Math.Round(avgProcessTime, 2),
            RequestData = filteredData.ToList()
        };
    }
}
