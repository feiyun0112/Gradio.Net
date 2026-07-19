using Gradio.Net.Events;
using Gradio.Net.Components;
using Gradio.Net.Core.Layouts;

namespace Gradio.Net.Core;

public class Renderable : IRenderable
{
    private readonly BlocksConfig _blocksContext;

    public int Id { get; private set; }
    public Type ContainerClass { get; private set; }
    public Block Container { get; private set; }
    public int ContainerId { get; private set; }
    public string Page { get; private set; }
    public Delegate Fn { get; private set; }
    public List<Component> Inputs { get; private set; }
    public List<EventListenerMethod> Triggers { get; private set; }
    public Dictionary<object, int> KeyToIdMap { get; private set; }
    public int RenderIteration { get; private set; }

    public Renderable(
        Delegate fn,
        List<Component> inputs,
        List<EventListenerMethod> triggers,
        object concurrencyLimit,
        string concurrencyId,
        string triggerMode,
        bool queue,
        string showProgress
    )
    {
        if (Context.RootBlock == null)
            throw new InvalidOperationException("Reactive render must be inside a Blocks context.");

        Id = Context.RootBlock.Renderables.Count;
        Context.RootBlock.Renderables.Add(this);

        var currentBlock = Context.Block;
        if (currentBlock is Row)
        {
            ContainerClass = typeof(Row);
            Container = new Row(visible: true, render: true) { ShowProgress = true };
        }
        else
        {
            ContainerClass = typeof(Column);
            Container = new Column(visible: true, render: true) { ShowProgress = true };
        }

        ContainerId = Container._id;
        Page = Context.RootBlock.CurrentPage;

        Fn = fn;
        Inputs = inputs;
        Triggers = triggers;
        KeyToIdMap = new Dictionary<object, int>();
        RenderIteration = 0;

        _blocksContext = Context.RootBlock.DefaultConfig;
        _blocksContext.SetEventTrigger(
            targets: triggers,
            fn: (Delegate)Apply,
            inputs: inputs,
            outputs: Container,
            apiVisibility: "undocumented",
            concurrencyLimit: concurrencyLimit,
            concurrencyId: concurrencyId,
            renderable: this,
            triggerMode: triggerMode,
            postprocess: false,
            queue: queue,
            showProgress: showProgress
        );
    }

    public object Apply(params object[] args)
    {
        var blocksConfig = LocalContext.BlocksConfig;
        if (blocksConfig == null)
            throw new InvalidOperationException("Reactive render must be inside a LocalContext.");

        var fnsFromLastRender = new List<BlockFunction>();
        foreach (var fnEntry in blocksConfig.Fns.Values)
        {
            if (fnEntry.RenderedIn == this)
                fnsFromLastRender.Add(fnEntry);
        }

        Block containerCopy = ContainerClass == typeof(Row)
            ? new Row(visible: true, render: false) { ShowProgress = true }
            : new Column(visible: true, render: false) { ShowProgress = true };

        containerCopy._id = ContainerId;
        containerCopy.Page = Page;
        var previousContainerRenderSequence = Container.RenderSequence;
        Container = containerCopy;

        var previousRenderable = LocalContext.Renderable;
        var previousKeyToIdMap = LocalContext.KeyToIdMap;
        var previousRenderBlock = LocalContext.RenderBlock;
        var previousContextBlock = Context.Block;

        LocalContext.Renderable = this;
        LocalContext.KeyToIdMap = KeyToIdMap;
        LocalContext.RenderBlock = containerCopy as BlockContext;
        Context.Block = containerCopy as BlockContext;

        try
        {
            try
            {
                Fn.DynamicInvoke(args);
                if (containerCopy is BlockContext copyContext)
                    copyContext.FillExpectedParents();
                blocksConfig.Blocks[ContainerId] = containerCopy;
                containerCopy.RenderSequence = previousContainerRenderSequence;
                blocksConfig.AttachLoadEvents(this);
            }
            finally { }
        }
        finally
        {
            LocalContext.Renderable = previousRenderable;
            LocalContext.KeyToIdMap = previousKeyToIdMap;
            LocalContext.RenderBlock = previousRenderBlock;
            Context.Block = previousContextBlock;
        }

        foreach (var fn in fnsFromLastRender)
        {
            if (blocksConfig.Fns.ContainsKey(fn.Id) &&
                blocksConfig.Fns[fn.Id].RenderIteration != RenderIteration)
            {
                blocksConfig.Fns.Remove(fn.Id);
            }
        }

        RenderIteration++;
        return null;
    }
}
