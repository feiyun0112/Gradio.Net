using System.Collections.Concurrent;
using System.Threading.Channels;


namespace Gradio.Net;

internal static class Context
{
    internal static object PendingMessageLock = new object();
    internal static ConcurrentDictionary<string, List<string>> PendingEventIdsSession { get; private set; } = new ConcurrentDictionary<string, List<string>>();
    internal static ConcurrentDictionary<string, string> DownloadableFiles { get; private set; } = new ConcurrentDictionary<string, string>();
    internal static ConcurrentDictionary<string, EventResult> EventResults { get; private set; } = new ConcurrentDictionary<string, EventResult>();
    internal static Channel<Event> EventChannel { get; private set; } = Channel.CreateUnbounded<Event>();
    internal static Blocks RootBlock { get; private set; } = null;

    private static Blocks _currentBlocks = null;
    internal static void SetCurrentBlocks(Blocks blocks)
    {
        if (RootBlock == null && blocks != null)
        {
            RootBlock = blocks;
        }
        if (_currentBlocks == null)
        {
            if (blocks != null)
            {
                _currentBlocks = blocks;
            }
        }
        else
        {
            if (blocks != null)
            {
                _currentBlocks.Add(blocks);
                _currentBlocks = blocks;
            }
            else
            {
                Form formParent = null;
                Tabs tabsParent = null;
                List<Block> children = _currentBlocks.ToList();
                for (int i = 0; i < children.Count; i++)
                {
                    Block child = children[i];
                    if (child is Tab tab)
                    {
                        if (tabsParent == null)
                        {
                            tabsParent = [];
                            _currentBlocks.Insert(i, tabsParent);
                            tabsParent.ParentBlocks = _currentBlocks;
                            tabsParent.Render = false;

                        }

                        tabsParent.Add(tab);
                        _currentBlocks.Remove(child);

                    }
                    else if (child is FormComponent formComponent)
                    {
                        if (formComponent.GetPropertyValue<bool>(nameof(formComponent.Container)))
                        {
                            if (formParent == null)
                            {
                                formParent = [];
                                _currentBlocks.Insert(i, formParent);
                                formParent.ParentBlocks = _currentBlocks;
                                formParent.Render = false;

                            }

                            formParent.Add(formComponent);
                            _currentBlocks.Remove(child);
                        }
                    }
                }
                _currentBlocks = _currentBlocks.ParentBlocks;
            }
        }
    }

    internal static void AddToCurrentBlocks(Block block)
    {
        if (_currentBlocks != null)
        {
            _currentBlocks.Add(block);
        }
    }

    private static int _id = 0;
    internal static int NextId()
    {
        return _id++;
    }
}
