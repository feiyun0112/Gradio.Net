# Gradio.NET: 使用 .NET 生成机器学习 Web 应用 [![main](https://github.com/feiyun0112/Gradio.Net/actions/workflows/main.yml/badge.svg)](https://github.com/feiyun0112/Gradio.Net/actions/workflows/main.yml) [![NuGet](https://img.shields.io/nuget/v/Gradio.Net.svg)](https://nuget.org/packages/Gradio.Net)

**[English](https://github.com/feiyun0112/Gradio.Net/blob/main/README.md)** | **[简体中文](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/README_zh-cn.md)** | **[日本語](https://github.com/feiyun0112/Gradio.Net/blob/main/readme_files/README_ja.md)**

更多信息请参阅 [指南](https://github.com/feiyun0112/Gradio.Net/tree/main/docs/guides)。

Gradio for .NET – [Gradio](https://github.com/gradio-app/gradio) 的 .NET 移植，Gradio是一个开源 Python 包，允许您为机器学习模型、API 或任何任意 Python 函数快速构建演示或 Web 应用程序。*无需任何 JavaScript、CSS 经验！*

使用Gradio，您可以基于您的机器学习模型或数据科学工作流快速创建一个漂亮的用户界面，让用户可以”尝试“拖放他们自己的图像、粘贴文本、录制他们自己的声音，并通过浏览器与您的演示程序进行交互。

![demo](https://github.com/feiyun0112/Gradio.Net/raw/main/readme_files/demo.gif)

只需几行 .NET 代码即可创建像上面这样的精美演示，让我们开始吧 💫

### 快速开始

- 1. 创建 ASP.NET Core Web API 项目。

- 2. 安装 NuGet 包 **Gradio.Net**。

- 3. 在 `Program.cs` 中输入以下示例代码：

#### 使用 `gr.Interface`（最快方式）

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

`gr.Interface` 可将任意 C# 函数即刻包装成 UI，无需编写任何布局代码。

#### 使用 `gr.Blocks`（完全控制布局）

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

大功告成 🎉🎉🎉

#### 集成到现有 ASP.NET Core 项目

使用 `AddGradio` 和 `UseGradio` 扩展方法：

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

也可以将 `gr.Interface` 与 `app.UseGradio` 结合使用：

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