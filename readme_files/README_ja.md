# Gradio.NET: .NETで機械学習Webアプリを構築 [![main](https://github.com/feiyun0112/Gradio.Net/actions/workflows/main.yml/badge.svg)](https://github.com/feiyun0112/Gradio.Net/actions/workflows/main.yml) [![NuGet](https://img.shields.io/nuget/v/Gradio.Net.svg)](https://nuget.org/packages/Gradio.Net)

**[English](https://github.com/feiyun0112/Gradio.Net/blob/main/README.md)** | **[简体中文](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/README_zh-cn.md)** | **日本語**

詳細については、[ガイド](https://github.com/feiyun0112/Gradio.Net/tree/main/docs/guides)を参照してください。

Gradio for .NET – [Gradio](https://github.com/gradio-app/gradio) の .NET 移植版で、機械学習モデル、API、または任意のPython関数のデモやWebアプリケーションを迅速に**構築**するためのオープンソースのPythonパッケージです。*JavaScript、CSS、またはWebホスティングの経験は不要です！*

![demo](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/demo.gif)

上記のような美しいデモを作成するには、ほんの数行の .NET コードが必要です。それでは始めましょう 💫

### 最初のデモを構築する

- 1. ASP.NET Core Web API プロジェクトを作成します。

- 2. NuGet パッケージ **Gradio.Net** をインストールします。

- 3. `Program.cs` にサンプルコードを入力します：

#### `gr.Interface` を使用する（最も簡単な方法）

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

`gr.Interface` は任意の C# 関数を瞬時に UI にラップします — レイアウトコードは不要です。

#### `gr.Blocks` を使用する（完全なレイアウト制御）

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

これで完了です 🎉🎉🎉

#### 既存の ASP.NET Core プロジェクトへの統合

`AddGradio` と `UseGradio` 拡張メソッドを使用します：

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

`gr.Interface` を `app.UseGradio` と組み合わせて使用することもできます：

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

### デモ

| Source Code | Demo Image |
| ----------- | ---------- |
| [Layout](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/layout_demo.md) | ![image](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/layout_demo.gif) |
| [Form](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/form_demo.md) | ![image](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/form_demo.gif) |
| [Media](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/media_demo.md) | ![image](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/media_demo.gif) |
| [Chatbot](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/chatbot_demo.md) | ![image](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/chatbot_demo.gif) |
| [Progress](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/progress_demo.md) | ![image](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/progress_demo.gif) |
| [Theme](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/theme_demo.md) | ![image](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/theme_demo.gif) |
