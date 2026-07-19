# Gradio.NET: Build Machine Learning Web Apps — in .NET [![main](https://github.com/feiyun0112/Gradio.Net/actions/workflows/main.yml/badge.svg)](https://github.com/feiyun0112/Gradio.Net/actions/workflows/main.yml) [![NuGet](https://img.shields.io/nuget/v/Gradio.Net.svg)](https://nuget.org/packages/Gradio.Net)

**English** | **[简体中文](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/README_zh-cn.md)** | **[日本語](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/README_ja.md)**

For more information, see the [Guides](https://github.com/feiyun0112/Gradio.Net/tree/main/docs/guides).

Gradio for .NET – a port of [Gradio](https://github.com/gradio-app/gradio), an open-source Python package that allows you to quickly **build** a demo or web application for your machine learning model, API, or any arbitrary Python function. *No JavaScript, CSS, or web hosting experience needed!*

![demo](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/demo.gif)

It just takes a few lines of .NET code to create a beautiful demo like the one above, so let's get started 💫

### Building Your First Demo

- 1. Create an ASP.NET Core Web API project.

- 2. Install NuGet package **Gradio.Net**.

- 3. Enter the sample code in `Program.cs`:

#### Using `gr.Interface` (quickest way)

```csharp
using Gradio.Net;

string Greet(string name, double intensity)
{
    return "Hello, " + name + "!" + new string('!', (int)intensity - 1);
}

var demo = gr.Interface(
    fn: Greet,
    inputs: new object[] { "text", gr.Slider(value: 2, minimum: 1, maximum: 10, step: 1) },
    outputs: new[] { "text" }
);

await demo.Launch();
```

`gr.Interface` wraps any C# function into a UI in seconds — no layout code required.

#### Using `gr.Blocks` (full layout control)

```csharp
using Gradio.Net;

await (await CreateBlocks()).Launch();

async Task<Blocks> CreateBlocks()
{
    using (var blocks = gr.Blocks())
    {
        gr.Markdown("Start typing below and then click **Run** to see the output.");
        Textbox input, output;
        using (gr.Row())
        {
            input = gr.Textbox(placeholder: "What is your name?");
            output = gr.Textbox();
        }
        var btn = gr.Button("Run");
        btn.Click(
            fn: new Func<string, (string, string)>(name => ($"Welcome to Gradio.Net, {name}!", "")),
            inputs: new[] { input },
            outputs: new[] { output, input });

        return blocks;
    }
}
```

That's All 🎉🎉🎉

#### Integrating into an existing ASP.NET Core project

Use the `AddGradio` and `UseGradio` extension methods:

```csharp
using Gradio.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGradio();

var app = builder.Build();

app.UseGradio(await CreateBlocks());

app.Run();

async Task<Blocks> CreateBlocks()
{
    using (var blocks = gr.Blocks())
    {
        gr.Markdown("# My Gradio App");
        Textbox input, output;
        using (gr.Row())
        {
            input = gr.Textbox(placeholder: "What is your name?");
            output = gr.Textbox();
        }
        var btn = gr.Button("Run");
        btn.Click(
            fn: new Func<string, (string, string)>(name => ($"Welcome to Gradio.Net, {name}!", "")),
            inputs: new[] { input },
            outputs: new[] { output, input });

        return blocks;
    }
}
```

You can also use `gr.Interface` with `app.UseGradio`:

```csharp
using Gradio.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGradio();

var app = builder.Build();

string Greet(string name, double intensity)
{
    return "Hello, " + name + "!" + new string('!', (int)intensity - 1);
}

var demo = gr.Interface(
    fn: Greet,
    inputs: new object[] { "text", gr.Slider(value: 2, minimum: 1, maximum: 10, step: 1) },
    outputs: new[] { "text" }
);

app.UseGradio(await demo.BuildBlocks());

app.Run();
```

### Demos

| Source Code | Demo Image |
| ----------- | ---------- |
| [Layout](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/layout_demo.md) | ![image](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/layout_demo.gif) |
| [Form](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/form_demo.md) | ![image](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/form_demo.gif) |
| [Media](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/media_demo.md) | ![image](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/media_demo.gif) |
| [Chatbot](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/chatbot_demo.md) | ![image](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/chatbot_demo.gif) |
| [Progress](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/progress_demo.md) | ![image](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/progress_demo.gif) |
| [Theme](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/theme_demo.md) | ![image](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/theme_demo.gif) |