using System.Collections;
using Gradio.Net.Core;
using Gradio.Net.Events;

namespace Gradio.Net.Components;

public class DeepLinkButton : Button
{
    public bool IsTemplate { get; } = true;

    public static int NCreated { get; private set; } = 0;

    public string CopiedValue { get; set; }

    public DeepLinkButton(
        string value = "Share via Link",
        string copiedValue = "Link Copied!",
        object? inputs = null,
        string variant = "secondary",
        string size = "lg",
        object? icon = null,
        string? link = null,
        bool visible = true,
        bool interactive = true,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        int? scale = null,
        int? minWidth = null,
        object? every = null
    ) : base(
        value: value,
        variant: variant,
        size: size,
        icon: icon?.ToString() ?? global::Gradio.Net.Utils.Utils.GetIconPath("link.svg"),
        link: link,
        visible: visible,
        interactive: interactive,
        elemId: $"gradio-share-link-button-{NCreated}",
        elemClasses: elemClasses as List<string>,
        key: key,
        preservedByKey: preservedByKey as string,
        scale: scale,
        minWidth: minWidth)
    {
        CopiedValue = copiedValue;

        if (elemClasses is string cls)
        {
            ElemClasses = new List<string> { cls };
        }

        if (preservedByKey is string pbk)
        {
            PreservedByKey = new List<string> { pbk };
        }
        else if (preservedByKey is List<string> pbkList)
        {
            PreservedByKey = pbkList;
        }

        NCreated += 1;

        if (Context.GetBlocksContext() != null && render)
        {
            Activate();
        }
    }

    public void Activate()
    {
        var js = GetShareLink(Value ?? "Share via Link", CopiedValue ?? "Link Copied!");

        var blocksConfig = Context.GetBlocksContext();
        if (blocksConfig == null)
        {
            return;
        }

        blocksConfig.SetEventTrigger(
            targets: new List<EventListenerMethod> { new EventListenerMethod(this, "click") },
            fn: null,
            inputs: Array.Empty<object>(),
            outputs: new List<object> { this },
            js: js,
            preprocess: false,
            postprocess: false,
            apiVisibility: "private"
        );

        blocksConfig.SetEventTrigger(
            targets: new List<EventListenerMethod> { new EventListenerMethod(this, "click") },
            fn: (Func<string>)(() =>
            {
                Thread.Sleep(1000);
                return Value ?? "Share via Link";
            }),
            inputs: Array.Empty<object>(),
            outputs: new List<object> { this },
            queue: false,
            apiVisibility: "undocumented"
        );
    }

    public string GetShareLink(string value = "Share via Link", string copiedValue = "Link Copied!")
    {
        var deleteSignLine = global::Gradio.Net.Utils.Utils.GetSpace() != null
            ? "currentUrl.searchParams.delete('__sign');"
            : string.Empty;

        var js = @"
() => {
    const sessionHash = window.__gradio_session_hash__;
    fetch(`gradio_api/deep_link?session_hash=${sessionHash}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.text();
        })
        .then(data => {
            const currentUrl = new URL(window.location.href);
            const cleanData = data.replace(/^""|""$/g, '');
            if (cleanData) {
                currentUrl.searchParams.set('deep_link', cleanData);
            }
            DELETE_SIGN_LINE
            navigator.clipboard.writeText(currentUrl.toString());
        })
        .catch(error => {
            console.error('Error fetching deep link:', error);
            return ""Error"";
        });

    return ""BUTTON_COPIED_VALUE"";
}
";

        return js
            .Replace("BUTTON_DEFAULT_VALUE", value, StringComparison.Ordinal)
            .Replace("BUTTON_COPIED_VALUE", copiedValue, StringComparison.Ordinal)
            .Replace("DELETE_SIGN_LINE", deleteSignLine, StringComparison.Ordinal)
            .Replace("ID", ElemId ?? string.Empty, StringComparison.Ordinal);
    }
}
