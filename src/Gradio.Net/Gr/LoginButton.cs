using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static LoginButton LoginButton(
        string value = "Sign in with Hugging Face",
        string logoutValue = "Logout ({})",
        object every = null,
        object inputs = null,
        string variant = "huggingface",
        string size = "lg",
        object icon = null,
        string link = null,
        bool visible = true,
        bool interactive = true,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        int? scale = null,
        int? minWidth = null)
    {
        return new LoginButton(
            value: value,
            logoutValue: logoutValue,
            every: every,
            inputs: inputs,
            variant: variant,
            size: size,
            icon: icon,
            link: link,
            visible: visible,
            interactive: interactive,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            scale: scale,
            minWidth: minWidth);
    }
}
