
namespace Gradio.Net.Core;

public static class Context
{
    public static Blocks? RootBlock { get; set; } = null;
    public static BlockContext? Block { get; set; } = null;
    private static int _id = 0;
    private static int _nextId = -1;
    public static string? Token { get; set; } = null;

    public static int Id => _id++;
    public static int NextId => Interlocked.Increment(ref _nextId);

    public static BlockContext? GetRenderContext()
    {
        if (LocalContext.Renderable != null)
        {
            return LocalContext.RenderBlock;
        }
        else
        {
            return Block;
        }
    }

    public static void SetRenderContext(BlockContext? block)
    {
        if (LocalContext.Renderable != null)
        {
            LocalContext.RenderBlock = block;
        }
        else
        {
            Block = block;
        }
    }

    public static BlocksConfig? GetBlocksContext()
    {
        if (LocalContext.Renderable != null)
        {
            if (LocalContext.BlocksConfig != null)
            {
                return LocalContext.BlocksConfig;
            }
        }

        if (RootBlock != null)
        {
            return RootBlock.DefaultConfig;
        }

        return null;
    }

    public static void SetBlocksContext(BlockContext? context)
    {
        // This method is kept for backward compatibility
        Block = context;
    }
}
