using Microsoft.AspNetCore.Http;

namespace Gradio.Net.Core;

public static class LocalContext
{
    private static AsyncLocal<Blocks?> _blocks = new();
    private static AsyncLocal<BlocksConfig?> _blocksConfig = new();
    private static AsyncLocal<IRenderable?> _renderable = new();
    private static AsyncLocal<BlockContext?> _renderBlock = new();
    private static AsyncLocal<bool> _inEventListener = new();
    private static AsyncLocal<string?> _eventId = new();
    private static AsyncLocal<HttpRequest?> _request = new();
    private static AsyncLocal<Progress?> _progress = new();
    private static AsyncLocal<Dictionary<object, int>?> _keyToIdMap = new();
    private static AsyncLocal<App?> _currentApp = new();

    public static Blocks? Blocks
    {
        get => _blocks.Value;
        set => _blocks.Value = value;
    }

    public static BlocksConfig? BlocksConfig
    {
        get => _blocksConfig.Value;
        set => _blocksConfig.Value = value;
    }

    public static IRenderable? Renderable
    {
        get => _renderable.Value;
        set => _renderable.Value = value;
    }

    // Alias for backward compatibility
    public static IRenderable? CurrentRenderable
    {
        get => Renderable;
        set => Renderable = value;
    }

    public static BlockContext? RenderBlock
    {
        get => _renderBlock.Value;
        set => _renderBlock.Value = value;
    }

    public static bool InEventListener
    {
        get => _inEventListener.Value;
        set => _inEventListener.Value = value;
    }

    public static string? EventId
    {
        get => _eventId.Value;
        set => _eventId.Value = value;
    }

    public static HttpRequest? Request
    {
        get => _request.Value;
        set => _request.Value = value;
    }

    public static Progress? Progress
    {
        get => _progress.Value;
        set => _progress.Value = value;
    }

    public static Dictionary<object, int>? KeyToIdMap
    {
        get => _keyToIdMap.Value;
        set => _keyToIdMap.Value = value;
    }

    public static App? CurrentApp
    {
        get => _currentApp.Value;
        set => _currentApp.Value = value;
    }
}
