using System.Text.Json;
using Gradio.Net.Components;

namespace Gradio.Net.Events;

public delegate Dependency TriggerBinding(
    Delegate? fn = null,
    object? inputs = null,
    object? outputs = null,
    string? apiName = null,
    bool queue = true);
