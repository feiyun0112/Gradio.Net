using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Gradio.Net.Components;
using Gradio.Net.Core;
using Gradio.Net.Core.Exceptions;
using Gradio.Net.Core.Flagging;
using Gradio.Net.Core.Layouts;
using Gradio.Net.Events;


namespace Gradio.Net;

public class TabbedInterface : Blocks
{
    public TabbedInterface(
        List<Blocks> interfaceList,
        List<string> tabNames = null,
        string title = null,
        bool? analyticsEnabled = null
    ) : base(
        title: title ?? "Gradio",
        analyticsEnabled: analyticsEnabled,
        mode: "tabbed_interface",
        fillHeight: true
    )
    {
        if (tabNames == null)
        {
            tabNames = Enumerable.Range(0, interfaceList.Count).Select(i => $"Tab {i + 1}").ToList();
        }

        if (title != null)
        {
            new Markdown($"<h1 style='text-align: center; margin-bottom: 1rem'>{title}</h1>");
        }
        new Tabs();
        {
            for (int i = 0; i < interfaceList.Count && i < tabNames.Count; i++)
            {
                var iface = interfaceList[i];
                var tabName = tabNames[i];
                new Tab(
                    label: tabName,
                    scale: iface.FillHeight ? 1 : (int?)null
                );
                {
                    iface.Render();
                }
            }
        }
    }
}
