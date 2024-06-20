using Gradio.Net.Enums;

namespace Gradio.Net;

public static partial class gr
{
    public static Dropdown Dropdown(
        IEnumerable<string> choices = null,
        object value = null,
        DropdownType? type = null,
        bool? multiselect = null,
        bool? allowCustomValue = null,
        int? maxChoices = null,
        bool? filterable = null,
        string label = null,
        string info = null,
        decimal? every = null,
        bool? showLabel = null,
        bool? container = null,
        int? scale = null,
        int? minWidth = null,
        bool? interactive = null,
        bool? visible = null,
        string elemId = null,
        IEnumerable<string> elemClasses = null,
        bool? render = null
    )
    {
        Dropdown block = new();
        block.Choices = choices;
        block.Type = type;
        block.Multiselect = multiselect;
        block.MaxChoices = maxChoices;
        block.AllowCustomValue = allowCustomValue;
        block.Filterable = filterable;

        block.Label = label;
        block.Info = info;
        block.Every = every;
        block.ShowLabel = showLabel;
        block.Container = container;
        block.Scale = scale;
        block.MinWidth = minWidth;
        block.Interactive = interactive;
        block.Visible = visible;
        block.ElemId = elemId;
        block.ElemClasses = elemClasses;
        block.Render = render;
        block.Value = value;

        Context.AddToCurrentBlocks(block);
        return block;
    }
}
