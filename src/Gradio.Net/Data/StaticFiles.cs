using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Gradio.Net.Data;


public static class StaticFiles
{
    public static List<string> AllPaths { get; set; } = new List<string>();

    public static void Initialize(List<string> paths)
    {
        AllPaths = paths.Select(p => System.IO.Path.GetFullPath(p)).ToList();
    }

    public static void Clear()
    {
        AllPaths.Clear();
    }
}
