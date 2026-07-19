using System.Collections;
using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("submit")]
public class Dialogue : Component
{
    public string DialogueType { get; set; }
    public List<string>? Speakers { get; set; }
    public Func<string, string, string>? Formatter { get; set; }
    public Func<string, string, Dictionary<string, string>>? Unformatter { get; set; }
    public List<string> Tags { get; set; }
    public string Separator { get; set; }
    public Dictionary<string, string>? ColorMap { get; set; }
    public string? Placeholder { get; set; }
    public bool Autofocus { get; set; }
    public bool Autoscroll { get; set; }
    public int? MaxLines { get; set; }
    public List<string>? Buttons { get; set; }
    public object? SubmitBtn { get; set; }
    public string UiMode { get; set; }

    public Dialogue(
        object? value = null,
        string type = "text",
        List<string>? speakers = null,
        Func<string, string, string>? formatter = null,
        Func<string, string, Dictionary<string, string>>? unformatter = null,
        List<string>? tags = null,
        string separator = "\n",
        Dictionary<string, string>? colorMap = null,
        string? label = "Dialogue",
        string? info = "Type colon (:) in the dialogue line to see the available tags",
        string? placeholder = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object? visible = null,
        string? elemId = null,
        bool autofocus = false,
        bool autoscroll = true,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        int? maxLines = null,
        List<string>? buttons = null,
        object? submitBtn = null,
        string uiMode = "both")
    {
        if (separator == " ")
        {
            throw new Error("Separator cannot be an empty string.");
        }

        if (type is not "list" and not "text")
        {
            throw new Error("`type` must be one of: 'list', 'text'.");
        }

        if (uiMode is not "dialogue" and not "text" and not "both")
        {
            throw new Error("`ui_mode` must be one of: 'dialogue', 'text', 'both'.");
        }

        Value = value;
        Label = label;
        Info = info;
        ShowLabel = showLabel ?? ShowLabel;
        Container = container;
        Scale = scale;
        MinWidth = minWidth;
        Interactive = interactive;
        if (visible is bool vb) Visible = vb;
        ElemId = elemId;
        ElemClasses = elemClasses switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => ElemClasses
        };
        Key = key;

        UiMode = uiMode;
        DialogueType = type;
        Placeholder = placeholder;
        Autofocus = autofocus;
        Autoscroll = autoscroll;
        MaxLines = maxLines;
        Speakers = speakers;
        Tags = tags ?? new List<string>();
        Formatter = formatter;
        Unformatter = unformatter;
        Separator = separator;
        ColorMap = colorMap;
        Buttons = buttons;
        SubmitBtn = submitBtn ?? false;

        if (interactive == false)
        {
            Info = null;
        }
    }

    public override string GetBlockName() => "dialogue";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["type"] = DialogueType;
        config["speakers"] = Speakers;
        config["tags"] = Tags;
        config["separator"] = Separator;
        config["color_map"] = ColorMap;
        config["placeholder"] = Placeholder;
        config["autofocus"] = Autofocus;
        config["autoscroll"] = Autoscroll;
        config["max_lines"] = MaxLines;
        config["buttons"] = Buttons;
        config["submit_btn"] = SubmitBtn;
        config["ui_mode"] = UiMode;
        return config;
    }

    public override object Preprocess(object payload)
    {
        var model = ToModel(payload);
        if (DialogueType == "list")
        {
            return model.ModelDump();
        }

        return FormatInternal(model);
    }

    public string FormatInternal(DialogueModelData payload)
    {
        if (payload.Root is string s)
        {
            return string.IsNullOrEmpty(s) ? string.Empty : s;
        }

        var lines = ToDialogueLines(payload.Root);
        if (lines.Count == 0)
        {
            return string.Empty;
        }

        if (lines.Count == 1 && string.IsNullOrEmpty(lines[0].Text))
        {
            return string.Empty;
        }

        var formatter = Formatter ?? DefaultFormatter;
        return string.Join(Separator, lines.Select(l => formatter(l.Speaker, l.Text)));
    }

    public static string DefaultFormatter(string speaker, string text)
    {
        return $"[{speaker}] {text}";
    }

    public static Dictionary<string, string> DefaultUnformatter(string line, string defaultSpeaker)
    {
        var trimmed = (line ?? string.Empty).Trim();
        if (trimmed.Length == 0)
        {
            return new Dictionary<string, string> { ["speaker"] = string.Empty, ["text"] = string.Empty };
        }

        if (trimmed.StartsWith("[", StringComparison.Ordinal) && trimmed.Contains("]", StringComparison.Ordinal))
        {
            var end = trimmed.IndexOf(']');
            var speaker = trimmed.Substring(1, end - 1);
            var text = trimmed.Substring(end + 1).Trim();
            return new Dictionary<string, string> { ["speaker"] = speaker, ["text"] = text };
        }

        return new Dictionary<string, string> { ["speaker"] = defaultSpeaker, ["text"] = trimmed };
    }

    public object Format(object value)
    {
        return FormatInternal(ToModel(value));
    }

    public object Unformat(Dictionary<string, object> payload)
    {
        var value = payload != null && payload.TryGetValue("text", out var textObj) ? textObj?.ToString() ?? string.Empty : string.Empty;
        if (string.IsNullOrWhiteSpace(value))
        {
            return new List<Dictionary<string, string>>();
        }

        var lines = value.Split(Separator);
        var dialogueLines = new List<Dictionary<string, string>>();

        var unformatter = Unformatter ?? DefaultUnformatter;
        var defaultSpeaker = Speakers != null && Speakers.Count > 0 ? Speakers[0] : "Unknown";

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0) continue;

            var parsed = unformatter(line, defaultSpeaker);
            var speaker = parsed.TryGetValue("speaker", out var s) ? s : string.Empty;
            var text = parsed.TryGetValue("text", out var t) ? t : string.Empty;
            if (!string.IsNullOrEmpty(speaker) || !string.IsNullOrEmpty(text))
            {
                dialogueLines.Add(new Dictionary<string, string>
                {
                    ["speaker"] = speaker,
                    ["text"] = text
                });
            }
        }

        return dialogueLines;
    }

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is string s)
        {
            return new DialogueModelData { Root = s };
        }

        return new DialogueModelData { Root = ToDialogueLines(value) };
    }

    public override object AsExample(object example)
    {
        return Preprocess(new DialogueModelData { Root = example });
    }

    public override object ExamplePayload()
    {
        return new List<Dictionary<string, string>>
        {
            new() { ["speaker"] = "Speaker 1", ["text"] = "Hello, how are you?" },
            new() { ["speaker"] = "Speaker 2", ["text"] = "I'm fine, thank you!" }
        };
    }

    public override object ExampleValue()
    {
        return new List<Dictionary<string, string>>
        {
            new() { ["speaker"] = "Speaker 1", ["text"] = "Hello, how are you?" },
            new() { ["speaker"] = "Speaker 2", ["text"] = "I'm fine, thank you!" }
        };
    }

    private static DialogueModelData ToModel(object payload)
    {
        if (payload is DialogueModelData dm)
        {
            return dm;
        }

        if (payload is string s)
        {
            return new DialogueModelData { Root = s };
        }

        return new DialogueModelData { Root = payload };
    }

    private static List<DialogueLineData> ToDialogueLines(object? payload)
    {
        var result = new List<DialogueLineData>();
        if (payload == null)
        {
            return result;
        }

        if (payload is IEnumerable<DialogueLineData> typed)
        {
            result.AddRange(typed);
            return result;
        }

        if (payload is IEnumerable enumerable && payload is not string)
        {
            foreach (var item in enumerable)
            {
                if (item is DialogueLineData d)
                {
                    result.Add(d);
                    continue;
                }

                if (item is Dictionary<string, string> ss)
                {
                    result.Add(new DialogueLineData
                    {
                        Speaker = ss.TryGetValue("speaker", out var sp) ? sp ?? string.Empty : string.Empty,
                        Text = ss.TryGetValue("text", out var tx) ? tx ?? string.Empty : string.Empty
                    });
                    continue;
                }

                if (item is Dictionary<string, object> so)
                {
                    result.Add(new DialogueLineData
                    {
                        Speaker = so.TryGetValue("speaker", out var sp) ? sp?.ToString() ?? string.Empty : string.Empty,
                        Text = so.TryGetValue("text", out var tx) ? tx?.ToString() ?? string.Empty : string.Empty
                    });
                }
            }
        }

        return result;
    }
}
