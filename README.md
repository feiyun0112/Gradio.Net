# Gradio.NET: Build Machine Learning Web Apps — in .NET [![main](https://github.com/feiyun0112/Gradio.Net/actions/workflows/main.yml/badge.svg)](https://github.com/feiyun0112/Gradio.Net/actions/workflows/main.yml) [![NuGet](https://img.shields.io/nuget/v/Gradio.Net.svg)](https://nuget.org/packages/Gradio.Net)

**English** | **[简体中文](README.zh-CN.md)**

Gradio for .NET — a port of [Gradio](https://github.com/gradio-app/gradio) 6.30.0, the Python package
that turns a function into a web app. Same frontend, same protocol, same config: the page a browser
gets is drawn by the Python version's own compiled frontend, unchanged.

With Gradio.Net you build a UI for a model, an API or any C# method in a few lines: users drag in
their own images, paste text, record their voice, and talk to your code through the browser. No
JavaScript, no CSS.

A few lines of .NET are all it takes. Let's start.

### Quick start

1. Create an ASP.NET Core Web API project.
2. Install the NuGet package **Gradio.Net**.
3. Put one of the examples below in `Program.cs` and run it.

#### With `gr.Interface` (the fastest way)

```csharp
using Gradio.Net;

string Greet(string name, double intensity) =>
    "Hello, " + name + new string('!', (int)intensity);

var demo = gr.Interface(
    Greet,
    inputs: ["text", "slider"],
    outputs: ["text"]);

demo.Launch();   // http://127.0.0.1:7860
```

`gr.Interface` wraps any C# method into a page — no layout code. It is the Python version's
quickstart, ported: the Python source is the same five lines with `fn=greet` instead of `Greet`, and
the port is checked by a test that runs it (`GrPortingTests`).

#### With `gr.Blocks` (full control of the layout)

```csharp
using Gradio.Net;

static string Greet(string name) => $"Welcome to Gradio.Net, {name}!";

var demo = gr.Blocks(title: "Hello");

using (demo)
{
    gr.Markdown("Start typing below and then click **Run** to see the output.");

    Textbox name, greeting;

    using (gr.Row())
    {
        name = gr.Textbox(label: "Name", placeholder: "What is your name?");
        greeting = gr.Textbox(label: "Greeting", interactive: false);
    }

    gr.Button("Run").Click(Greet, [name], [greeting]);
}

demo.Launch();
```

Every `gr.…(...)` line inside the `using` is a line of the page — that is the Python version's
`with gr.Blocks() as demo:`, and `gr.Row()` is the layout and its scope at once.

#### Inside an existing ASP.NET Core project

`AddGradio()` registers the kernel; write the page, hand it to `UseGradio(page)`, and the host serves
it — routes, assets, queue and all:

```csharp
using Gradio.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGradio();

var app = builder.Build();

app.UseGradio(CreatePage());

app.Run();

static Blocks CreatePage()
{
    var demo = gr.Blocks(title: "My Gradio app");

    using (demo)
    {
        var name = gr.Textbox(label: "Name");
        var greeting = gr.Textbox(label: "Greeting", interactive: false);

        gr.Button("Run").Click(Greet, [name], [greeting]);
    }

    return demo;
}

static string Greet(string name) => $"Welcome to Gradio.Net, {name}!";
```

`gr.Interface` works the same way, because it returns an application too:

```csharp
using Gradio.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGradio();

var app = builder.Build();

string Greet(string name, double intensity) =>
    "Hello, " + name + new string('!', (int)intensity);

app.UseGradio(gr.Interface(Greet, inputs: ["text", "slider"], outputs: ["text"]));

app.Run();
```

### One API, two spellings

There is one package and one application. `gr` is a set of factories, not a wrapper: what it hands
back is the core type.

**`gr` — the spelling a Python file already has.** The names are the Python version's, down to the parameter
names and their defaults, so a ported file differs from the Python by three kinds of edit and no
more: `=` becomes `:`, `with` becomes `using`, and a decorator becomes a call. That is the layer the
examples above use, and the one a port starts with.

**The types underneath — the spelling C# would give them.** `gr.Textbox(...)` returns a `Textbox`,
`gr.Blocks()` returns a `Blocks`, and those are ordinary types: events are methods on the component
(`button.Click(fn, inputs, outputs)`), the component lists are `Block[]`, and a handler is a
delegate. Hold them, pass them, subclass a component — nothing is wrapped.

What is deliberately *not* public is writing into a page by hand. `Blocks.BeginRender`, `Add`,
`Reserve`, `Attach` and `Enter` are internal: a page is written by calling a factory, because
`gr.Textbox(...)` is a line on the page rather than a value and `using (gr.Blocks())` is the render
its closing brace ends. That rule is what keeps `demo.Add(gr.Textbox(...))` out of a port, and it
applies to new code as well — there is one way to put a block on a page.

### Demos

| Demo | What it shows | Change this |
| --- | --- | --- |
| [Quickstart](Demo/Quickstart/) | `gr.Interface`: a function, two inputs, one output, and no layout code. | `Greet` — the handler; the page is built from its signature. |
| [Blocks](Demo/Blocks/) | A page written by hand: `Row` and `Column`, two textboxes, a button, two events, and a `Tabs`/`Tab` bar. | `Shout`, and the component lists that say what each event reads and writes. |
| [Chat](Demo/Chat/) | `gr.ChatInterface` with a handler that streams an answer from an endpoint. | `Answer` — the request it sends. `LocalModel.cs` is what answers it. |
| [Streaming](Demo/Streaming/) | A generator handler: each `yield` replaces the output while `Progress` moves the bar. | `Type` — what each piece is, and how fast the pieces arrive. |
| [Form](Demo/Form/) | Eight fields submitted together by one button, and one field deciding what another offers (`gr.Update`). | `Describe` — what the form answers with. |
| [Files](Demo/Files/) | A CSV uploaded, its rows shown as a table, the same rows downloaded as tab-separated text. | `Convert` — the one function that reads, reshapes and writes. |
| [Media](Demo/Media/) | An image, a video and a sound handed to a handler and handed back — no conversion — plus the four signals a video player raises. | `Same` — the handler every file row binds; `Count` — what the player's signals do. |
| [Multipage](Demo/Multipage/) | One application, several pages: `demo.Route("About")`, the navbar that offers them, and an endpoint on each page. | the route's name, or the markdown inside it. |
| [Themes](Demo/Themes/) | `demo.Theme = gr.Themes.Soft(...)` — one line for a preset, eight choices on top. | the preset, and the two choices it is given. |
| [Charts](Demo/Charts/) | A chart drawn from the columns a handler returns, and a plotly drawing passed through. | `Visits` — the columns; `Bars` — the drawing. |

Ten programs in all — quickstart, a page written by hand, chat, streaming, files, form, media,
multipage, themes, and charts. `cd` into one and `dotnet run`; each is offline, and each references
this tree rather than a published package. [`Demo/README.md`](Demo/README.md) says what each one is
and which line to change in it.

### Coming from Python

The table below is the whole of the translation: three naming rules, then the places C# forces a
different shape. A ported file needs nothing else.

| Python | C# | Why |
| --- | --- | --- |
| `gr.Textbox(value="x")` | `gr.Textbox(value: "x")` | named arguments are `:` |
| `with gr.Blocks() as demo:` | `using (var demo = gr.Blocks())` | no context managers |
| `**kwargs` | `IDictionary<string, object?> props` | no keyword splat |
| `@gr.render` / `@gr.mcp.tool` | `gr.Render(...)` / `gr.Mcp.Tool(...)` | no decorators — neither member exists yet; see the table below |
| `def fn(..., progress=gr.Progress())` | the handler takes a `Progress` parameter | injection by annotation |
| `gr.on([btn.click, tb.change])` | `gr.On([btn.Events.Click, tb.Events.Change])` | an event used as a value |
| `tb.value` | `tb.Value` | the factory returns the core type |

Member names keep their word and change only their case (`btn.click` → `btn.Click`,
`demo.launch()` → `demo.Launch()`), and a trigger point moves under `Events` only because the binding
method already took the name: bind with `btn.Click(...)`, merge with `gr.On([btn.Events.Click, …])`.
