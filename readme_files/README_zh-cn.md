# Gradio.NET: 使用 .NET 生成机器学习 Web 应用 [![main](https://github.com/feiyun0112/Gradio.Net/actions/workflows/main.yml/badge.svg)](https://github.com/feiyun0112/Gradio.Net/actions/workflows/main.yml) [![NuGet](https://img.shields.io/nuget/v/Gradio.Net.svg)](https://nuget.org/packages/Gradio.Net)

**[English](../README.md)** | 简体中文

Gradio for .NET – [Gradio](https://github.com/gradio-app/gradio) 的 .NET 移植，Gradio是一个开源 Python 包，允许您为机器学习模型、API 或任何任意 Python 函数快速构建演示或 Web 应用程序。*无需任何 JavaScript、CSS 经验！*

使用Gradio，您可以基于您的机器学习模型或数据科学工作流快速创建一个漂亮的用户界面，让用户可以”尝试“拖放他们自己的图像、粘贴文本、录制他们自己的声音，并通过浏览器与您的演示程序进行交互。

![demo](./demo.gif)

只需几行 .NET 代码即可创建像上面这样的精美演示，让我们开始吧 💫

### 快速开始

- 1. 创建 ASP.NET Core Web API 项目。

- 2. 安装 NuGet **Gradio.Net.AspNetCore**。

- 3. 在 Program.cs 中输入以下示例代码:


```C#
App.Launch(await CreateBlocks(), config => {
    //默认使用 fonts.googleapis.com
    config.Stylesheets = new string[] {
            "https://fonts.font.im/css2?family=Source+Sans+Pro:wght@400;600&display=swap",
            "https://fonts.font.im/css2?family=IBM+Plex+Mono:wght@400;600&display=swap"
        };
});

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
        await btn.Click(fn: async (input) => gr.Output($"Welcome to Gradio.Net, {input.Data[0]}!"), inputs: new[] { input }, outputs: new[] { output });

        return blocks;
    }
}
```

🎉🎉🎉

**如果您想在现有项目中使用 **Gradio.Net.AspNetCore****

可以使用`AddGradio`和 `UseGradio`扩展方法：

```C#
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGradio();

var app = builder.Build();

app.UseGradio(await CreateBlocks());

app.Run();
```

### Demos

| Source Code | Demo Image |
| ----------- | ---------- |
| [Layout](./layout_demo.md) | ![image](./layout_demo.gif) |
| [Form](./form_demo.md) | ![image](./form_demo.gif) |
| [Media](./media_demo.md) | ![image](./media_demo.gif) |
| [Chatbot](./chatbot_demo.md) | ![image](./chatbot_demo.gif) |
| [Progress](./progress_demo.md) | ![image](./progress_demo.gif) |
