namespace Gradio.Net;

public static partial class gr
{
    public static void SetStaticPaths(params string[] paths)
        => Utils.Utils.SetStaticPaths(paths);
}
