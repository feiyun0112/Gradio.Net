; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
GRADIO0001 | Gradio.Architecture | Error | Static mutable state is prohibited
GRADIO0002 | Gradio.Architecture | Error | `#pragma warning disable` is prohibited
GRADIO0003 | Gradio.Architecture | Error | `#nullable disable` is prohibited
GRADIO0004 | Gradio.Architecture | Error | A block does not declare a Python parameter
GRADIO0005 | Gradio.Architecture | Error | A block is not one the Python parameter manifest knows
GRADIO0006 | Gradio.Architecture | Error | The Python parameter manifest is not readable
GRADIO0007 | Gradio.Architecture | Error | A block's parameter cannot be written into a factory
GRADIO0008 | Gradio.Architecture | Error | A declaration the factories are written from is not readable
GRADIO0009 | Gradio.Architecture | Error | The Python version names an event this build has no constant for
GRADIO0010 | Gradio.Architecture | Error | The event manifest is not readable
