using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Gradio.Net.Components;
using Gradio.Net.Core.Flagging;
using Gradio.Net.Data;
using Gradio.Net.Events;
using Gradio.Net.Utils;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net.Core;

public class SimpleNamespace
{
    public Dictionary<string, object> Props { get; }
    public bool IsComponentUpdate { get; }

    public SimpleNamespace(Dictionary<string, object> props)
    {
        Props = props;
        IsComponentUpdate = true;
    }

    public object this[string key]
    {
        get => Props[key];
        set => Props[key] = value;
    }
}
