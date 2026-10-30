# Demos

**English** | **[简体中文](README.zh-CN.md)**

Ten programs, one idea each. `cd` into one and run it:

```bash
cd Demo/Quickstart
dotnet run          # the page is at http://127.0.0.1:7860
```

`dotnet run --project Demo/Quickstart` does the same from the root. Ctrl-C stops it.

Each demo is a program of its own rather than a file in a shared one, and each references
`src/Gradio.Net/Gradio.Net.csproj` rather than a published package — so what a demo runs is the code
in this tree, and a demo is one directory to copy and change.

| Demo | What it shows | Change this |
| --- | --- | --- |
| [Quickstart](Quickstart/) | `gr.Interface`: a function, two inputs, one output, and no layout code. | `Greet` — the handler; the page is built from its signature. |
| [Blocks](Blocks/) | A page written by hand: `Row` and `Column`, two textboxes, a button, two events, and a `Tabs`/`Tab` bar. | `Shout`, and the component lists that say what each event reads and writes. |
| [Chat](Chat/) | `gr.ChatInterface` with a handler that streams an answer from an endpoint. | `Answer` — the request it sends. `LocalModel.cs` is what answers it. |
| [Streaming](Streaming/) | A generator handler: each `yield` replaces the output while `Progress` moves the bar. | `Type` — what each piece is, and how fast the pieces arrive. |
| [Form](Form/) | Eight fields submitted together by one button, and one field deciding what another offers (`gr.Update`). | `Describe` — what the form answers with. |
| [Files](Files/) | A CSV uploaded, its rows shown as a table, the same rows downloaded as tab-separated text. | `Convert` — the one function that reads, reshapes and writes. |
| [Media](Media/) | An image, a video and a sound handed to a handler and handed back — no conversion — plus the four signals a video player raises. | `Same` — the handler every file row binds; `Count` — what the player's signals do. |
| [Multipage](Multipage/) | One application, several pages: `demo.Route("About")`, the navbar that offers them, and an endpoint on each page. | the route's name, or the markdown inside it. |
| [Themes](Themes/) | `demo.Theme = gr.Themes.Soft(...)` — one line for a preset, eight choices on top. | the preset, and the two choices it is given. |
| [Charts](Charts/) | A chart drawn from the columns a handler returns, and a plotly drawing passed through. | `Visits` — the columns; `Bars` — the drawing. |

## These are for reading

The examples are written to be read and changed, so they are not also the thing that fails when the
protocol drifts. What is checked about them is that each one is a program, that the build
compiles it, and that it reaches no address outside loopback and reads no configuration or
environment variable; see `DemoProjectTests`.

## Offline, by construction

No demo reads a token, and none calls an address outside loopback. The one that talks to a server
runs its own: `Chat/LocalModel.cs` answers like an OpenAI-compatible endpoint, on a port the
operating system picks, so it cannot collide with a server you already have.

A real model is the same handler with a different address — which is the point of that demo: what
changes when you point it at your own is the address, and nothing else.

## Two at once

Every demo launches on 7860 unless it is told otherwise, so run one at a time, or give the second one
another port:

```csharp
demo.Launch(serverPort: 7861);
```

## The .NET version

The demos target `net10.0`, the SDK this repository builds with. The package itself targets `net8.0`,
`net9.0` and `net10.0`, so running a demo on an older SDK is the `TargetFramework` line in its
`.csproj` changed to that version.
