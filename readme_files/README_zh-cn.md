
<div>

[English](../README.md)

</div>

# Gradio.NET: 使用 .NET 生成机器学习 Web 应用

Gradio for .NET – [Gradio](https://github.com/gradio-app/gradio) 的 .NET 移植，Gradio是一个开源 Python 包，允许您为机器学习模型、API 或任何任意 Python 函数快速构建演示或 Web 应用程序。*无需任何 JavaScript、CSS 经验！*

使用Gradio，您可以基于您的机器学习模型或数据科学工作流快速创建一个漂亮的用户界面，让用户可以”尝试“拖放他们自己的图像、粘贴文本、录制他们自己的声音，并通过浏览器与您的演示程序进行交互。

![demo](./demo.gif)

只需几行 .NET 代码即可创建像上面这样的精美演示，让我们开始吧 💫

### 快速开始

- 1. 创建 ASP.NET Core Web API 项目。

- 2. 安装 NuGet **Gradio.Net**。

- 3. 在 Program.cs 中输入以下示例代码:


```C#
App.Launch(await CreateBlocks());

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

**如果您想在现有项目中使用 **Gradio.Net****

可以使用`AddGradio`和 `UseGradio`扩展方法：

```C#
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGradio();

var app = builder.Build();

app.UseGradio(await CreateBlocks());

app.Run();
```

### 当前进展

截至目前，Gradio.Net 只是一个 M.V.P. 版本，更多的 Gradio 组件将在稍后移植。

- **Blocks**
- **Row**
- **Markdown**
- **Textbox**(event not implemented)
- **Button**
