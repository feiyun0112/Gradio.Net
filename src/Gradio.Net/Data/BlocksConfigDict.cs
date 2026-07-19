namespace Gradio.Net.Data;

public class BlocksConfigDict
{
    public string Version { get; set; } = string.Empty;

    public string? DeepLinkState { get; set; }

    public string Mode { get; set; } = string.Empty;

    public int AppId { get; set; }

    public bool DevMode { get; set; }

    public bool VibeMode { get; set; }

    public bool AnalyticsEnabled { get; set; }

    public List<Dictionary<string, object>> Components { get; set; } = new List<Dictionary<string, object>>();

    public string? Css { get; set; }

    public bool ConnectHeartbeat { get; set; }

    public object? Js { get; set; }

    public string? Head { get; set; }

    public object Title { get; set; } = string.Empty;

    public string? SpaceId { get; set; }

    public bool EnableQueue { get; set; }

    public bool ShowError { get; set; }

    public bool IsColab { get; set; }

    public int? MaxFileSize { get; set; }

    public List<string> Stylesheets { get; set; } = new List<string>();

    public string? Theme { get; set; }

    public string Protocol { get; set; } = string.Empty;

    public BodyCSS BodyCss { get; set; } = new BodyCSS();

    public bool FillHeight { get; set; }

    public bool FillWidth { get; set; }

    public string ThemeHash { get; set; } = string.Empty;

    public Layout? Layout { get; set; }

    public List<Dictionary<string, object>>? Dependencies { get; set; }

    public string? Root { get; set; }

    public string? Username { get; set; }

    public string ApiPrefix { get; set; } = string.Empty;

    public bool? Pwa { get; set; }

    public Dictionary<string, Page> Page { get; set; } = new Dictionary<string, Page>();

    public List<Tuple<string, string>> Pages { get; set; } = new List<Tuple<string, string>>();

    public string? CurrentPage { get; set; }

    public Dictionary<string, Dictionary<string, string>>? I18nTranslations { get; set; }

    public bool? McpServer { get; set; }

    public List<object> FooterLinks { get; set; } = new List<object>();
}
