namespace Gradio.Net;
public static partial class gr
{
    /// <summary>Builds the Python version's <c>row</c>: a <see cref="Row"/>, with the Python
    /// version's parameters in the Python version's order.</summary> <returns>The layout, which is
    /// the core type rather than a wrapper around it.</returns>
    /// <remarks>
    /// <para>
    /// A layout is a block and a scope at once, because the Python version's <c>with gr.Row():</c>
    /// is both. <c>using (gr.Row())</c> is the whole of it: the row is placed where the line is,
    /// the lines under it belong to it, and the closing brace closes it — which is also where the
    /// row groups what was written into it, as the Python version's <c>__exit__</c> does. Disposing
    /// the row is what closes the scope, which is why <see cref="BlockContext"/> is
    /// <see cref="IDisposable"/> and why its <c>Dispose</c> means "leave the scope" rather than
    /// "destroy the block".
    /// </para>
    /// <para>
    /// The row is entered when it is built rather than when the <c>using</c> starts, because C# has
    /// nowhere else to put the opening brace: <c>using</c> says only what happens at the end. So a
    /// row that is built and never used is a row that stays open — the same cost <c>gr.Blocks()</c>
    /// has, and the same reason to always write the block.
    /// </para>
    /// <para>
    /// The parameter list is the Python version's, spelled the way the Python version spells it.
    /// Written by hand rather than generated: a layout's factory and its scope are one feature, so
    /// the two are written together. What keeps the hand-written list honest is the same comparison
    /// the generated ones face — the names, their order and their defaults against the recorded the
    /// Python version signature — so a parameter the Python version adds fails the build rather
    /// than going missing.
    /// </para>
    /// </remarks>
    public static Row Row(
        string variant = "default",
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        int? scale = null,
        bool render = true,
        object? height = null,
        object? maxHeight = null,
        object? minHeight = null,
        bool equalHeight = false,
        bool showProgress = false,
        object? key = null,
        object? preservedByKey = null)
    {
        var block = new Row
        {
            Variant = variant,
            Render = render,
            EqualHeight = equalHeight,
            ShowProgress = showProgress,
        };

        WriteCommon(block, visible, elemId, elemClasses, key, preservedByKey);

        if (scale is { } scaleGiven)
        {
            block.Scale = scaleGiven;
        }

        if (height is { } heightGiven)
        {
            block.Height = heightGiven;
        }

        if (maxHeight is { } maxHeightGiven)
        {
            block.MaxHeight = maxHeightGiven;
        }

        if (minHeight is { } minHeightGiven)
        {
            block.MinHeight = minHeightGiven;
        }

        return Ambient.Enter(block);
    }

    /// <summary>Builds the Python version's <c>column</c>: a <see cref="Column"/>, with the Python
    /// version's parameters in the Python version's order.</summary> <returns>The layout, which is
    /// the core type rather than a wrapper around it.</returns>
    /// <remarks>
    /// Entered the way a row is: <c>using (gr.Column())</c>. <c>scale</c> and
    /// <c>minWidth</c> are what a row divides its width by, which is why they are the first two
    /// parameters here and the last two on a row.
    /// </remarks>
    public static Column Column(
        int scale = 1,
        int minWidth = 320,
        string variant = "default",
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        bool showProgress = false,
        object? key = null,
        object? preservedByKey = null)
    {
        var block = new Column
        {
            Scale = scale,
            MinWidth = minWidth,
            Variant = variant,
            Render = render,
            ShowProgress = showProgress,
        };

        WriteCommon(block, visible, elemId, elemClasses, key, preservedByKey);

        return Ambient.Enter(block);
    }

    /// <summary>Builds the Python version's <c>group</c>: a <see cref="Group"/>, with the Python
    /// version's parameters in the Python version's order.</summary> <returns>The layout, which is
    /// the core type rather than a wrapper around it.</returns>
    /// <remarks>
    /// Entered the way a row is. A group draws its children as one block, without the padding and
    /// margin a column would put between them.
    /// </remarks>
    public static Group Group(
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null)
    {
        var block = new Group { Render = render };

        WriteCommon(block, visible, elemId, elemClasses, key, preservedByKey);

        return Ambient.Enter(block);
    }

    /// <summary>Builds the Python version's <c>accordion</c>: an <see cref="Accordion"/>, with the
    /// Python version's parameters in the Python version's order.</summary> <returns>The layout,
    /// which is the core type rather than a wrapper around it.</returns>
    /// <remarks>
    /// Entered the way a row is. <c>open</c> decides what the first visitor sees; the two signals
    /// the user's clicks raise are trigger points on the block (<see cref="Accordion.Expand"/>).
    /// </remarks>
    public static Accordion Accordion(
        string? label = null,
        bool open = true,
        object? height = null,
        object? maxHeight = null,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null)
    {
        var block = new Accordion { Label = label, Open = open, Render = render };

        WriteCommon(block, visible, elemId, elemClasses, key, preservedByKey);

        if (height is { } heightGiven)
        {
            block.Height = heightGiven;
        }

        if (maxHeight is { } maxHeightGiven)
        {
            block.MaxHeight = maxHeightGiven;
        }

        return Ambient.Enter(block);
    }

    /// <summary>Builds the Python version's <c>sidebar</c>: a <see cref="Sidebar"/>, with the
    /// Python version's parameters in the Python version's order.</summary> <returns>The layout,
    /// which is the core type rather than a wrapper around it.</returns>
    /// <remarks>
    /// Entered the way a row is. The panel is drawn down one side of the page, and its label names
    /// it for the application rather than for the user — the Python version does not draw it.
    /// </remarks>
    public static Sidebar Sidebar(
        string? label = null,
        bool open = true,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? width = null,
        string position = "left",
        object? key = null,
        object? preservedByKey = null)
    {
        var block = new Sidebar
        {
            Label = label,
            Open = open,
            Render = render,
            Position = position,
        };

        WriteCommon(block, visible, elemId, elemClasses, key, preservedByKey);

        if (width is { } widthGiven)
        {
            block.Width = widthGiven;
        }

        return Ambient.Enter(block);
    }

    /// <summary>Builds the Python version's <c>tabs</c>: a <see cref="Tabs"/>, with the Python
    /// version's parameters in the Python version's order.</summary> <returns>The layout, which is
    /// the core type rather than a wrapper around it.</returns>
    /// <remarks>
    /// Entered the way a row is, and a <see cref="Tab"/> written inside it is one of its tabs:
    /// <c>using (gr.Tabs()) { using (gr.Tab("One")) { … } }</c>.
    /// </remarks>
    public static Tabs Tabs(
        object? selected = null,
        string overflowBehavior = "menu",
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null)
    {
        var block = new Tabs { OverflowBehavior = overflowBehavior, Render = render };

        WriteCommon(block, visible, elemId, elemClasses, key, preservedByKey);

        if (selected is { } selectedGiven)
        {
            block.Selected = selectedGiven;
        }

        return Ambient.Enter(block);
    }

    /// <summary>Builds the Python version's <c>tab</c>: a <see cref="Tab"/>, with the Python
    /// version's parameters in the Python version's order.</summary> <returns>The layout, which is
    /// the core type rather than a wrapper around it.</returns>
    /// <remarks>
    /// Entered the way a row is. Written straight into an application rather than into a
    /// <see cref="Tabs"/>, a tab is grouped into a tabs of its own — which is the Python version's
    /// behaviour and the reason the scope shape is worth writing even when there is one tab.
    /// </remarks>
    public static Tab Tab(
        string? label = null,
        object? visible = null,
        bool interactive = true,
        object? id = null,
        string alignment = "left",
        string? elemId = null,
        object? elemClasses = null,
        int? scale = null,
        object? height = null,
        object? maxHeight = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        bool renderChildren = false)
    {
        var block = new Tab
        {
            Label = label,
            Interactive = interactive,
            Alignment = alignment,
            Render = render,
            RenderChildren = renderChildren,
        };

        WriteCommon(block, visible, elemId, elemClasses, key, preservedByKey);

        if (id is { } idGiven)
        {
            block.TabId = idGiven;
        }

        if (scale is { } scaleGiven)
        {
            block.Scale = scaleGiven;
        }

        if (height is { } heightGiven)
        {
            block.Height = heightGiven;
        }

        if (maxHeight is { } maxHeightGiven)
        {
            block.MaxHeight = maxHeightGiven;
        }

        return Ambient.Enter(block);
    }

    /// <summary>Builds the same block as <see cref="Tab"/>: the Python version exports both
    /// spellings.</summary> <returns>The layout, which is the core type rather than a wrapper
    /// around it.</returns>
    /// <remarks>
    /// The arguments are passed by name rather than by position, because half of them are
    /// <see cref="object"/>: two of those swapped would compile and build a tab whose label is its
    /// id. The names are the signature's, so a rename here is a compile error rather than a
    /// behaviour that quietly changes.
    /// </remarks>
    public static Tab TabItem(
        string? label = null,
        object? visible = null,
        bool interactive = true,
        object? id = null,
        string alignment = "left",
        string? elemId = null,
        object? elemClasses = null,
        int? scale = null,
        object? height = null,
        object? maxHeight = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        bool renderChildren = false) =>
        Tab(
            label: label,
            visible: visible,
            interactive: interactive,
            id: id,
            alignment: alignment,
            elemId: elemId,
            elemClasses: elemClasses,
            scale: scale,
            height: height,
            maxHeight: maxHeight,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            renderChildren: renderChildren);

    /// <summary>Builds the Python version's <c>walkthrough</c>: a <see cref="Walkthrough"/>, with
    /// the Python version's parameters in the Python version's order.</summary> <returns>The
    /// layout, which is the core type rather than a wrapper around it.</returns>
    /// <remarks>
    /// Entered the way a row is, and a <see cref="Step"/> written inside it is one of its steps.
    /// </remarks>
    public static Walkthrough Walkthrough(
        int? selected = null,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null)
    {
        var block = new Walkthrough { Selected = selected, Render = render };

        WriteCommon(block, visible, elemId, elemClasses, key, preservedByKey);

        return Ambient.Enter(block);
    }

    /// <summary>Builds the Python version's <c>step</c>: a <see cref="WalkthroughStep"/>, with the
    /// Python version's parameters in the Python version's order.</summary> <returns>The layout,
    /// which is the core type rather than a wrapper around it.</returns>
    /// <remarks>
    /// The Python version calls the block <c>Step</c> and so does this factory; the type is named
    /// after the layout it belongs to, because <c>Step</c> is a reserved word in some .NET
    /// languages and a type named that way is a type those languages cannot spell.
    /// </remarks>
    public static WalkthroughStep Step(
        string? label = null,
        object? visible = null,
        bool interactive = true,
        object? id = null,
        string? elemId = null,
        object? elemClasses = null,
        int? scale = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null)
    {
        var block = new WalkthroughStep
        {
            Label = label,
            Interactive = interactive,
            Render = render,
        };

        WriteCommon(block, visible, elemId, elemClasses, key, preservedByKey);

        if (id is { } idGiven)
        {
            block.StepId = idGiven;
        }

        if (scale is { } scaleGiven)
        {
            block.Scale = scaleGiven;
        }

        return Ambient.Enter(block);
    }

    /// <summary>Writes the parameters every block declares, the way the generated factories
    /// do.</summary> <param name="block">The layout being built.</param> <param name="visible">The
    /// Python version's <c>visible</c>: a boolean, or the string <c>"hidden"</c>.</param>
    /// <param name="elemId">The HTML id, when the application gives one.</param>
    /// <param name="elemClasses">One class name, or several.</param> <param name="key">The key a
    /// re-render identifies the block by.</param> <param name="preservedByKey">The parameters a
    /// re-render keeps.</param>
    /// <remarks>
    /// Shared because the seven parameters are declared once on <see cref="Block"/> and
    /// <see cref="BlockContext"/> rather than once per layout, and a second copy per factory would
    /// be a second place for one of them to drift. A parameter left out is not written, which is
    /// the Python version's default: the block's own initialiser is the value the Python version
    /// declares.
    /// </remarks>
    private static void WriteCommon(
        BlockContext block,
        object? visible,
        string? elemId,
        object? elemClasses,
        object? key,
        object? preservedByKey)
    {
        if (visible is { } visibleGiven)
        {
            block.Visible = Values.VisibilityOf(visibleGiven);
        }

        if (elemId is { } elemIdGiven)
        {
            block.ElemId = elemIdGiven;
        }

        if (elemClasses is { } elemClassesGiven)
        {
            block.ElemClasses = Values.Strings(elemClassesGiven);
        }

        if (key is { } keyGiven)
        {
            block.Key = keyGiven;
        }

        if (preservedByKey is { } preservedGiven)
        {
            block.PreservedByKey = Values.Strings(preservedGiven);
        }
    }
}
