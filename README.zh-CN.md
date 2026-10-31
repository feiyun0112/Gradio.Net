# Gradio.NET：用 .NET 构建机器学习 Web 应用 [![main](https://github.com/feiyun0112/Gradio.Net/actions/workflows/main.yml/badge.svg)](https://github.com/feiyun0112/Gradio.Net/actions/workflows/main.yml) [![NuGet](https://img.shields.io/nuget/v/Gradio.Net.svg)](https://nuget.org/packages/Gradio.Net)

**[English](README.md)** | **简体中文**

Gradio for .NET —— [Gradio](https://github.com/gradio-app/gradio) 6.30.0 的移植版。Gradio 是一个把函数
变成 Web 应用的 Python 包。同一个前端、同一套协议、同一份配置：浏览器拿到的页面由 Python 版自己编译的前端绘制，
一个字节都不改。

用 Gradio.Net，几行代码就能为模型、API 或任意 C# 方法做出界面：用户拖进自己的图片、粘贴文本、录下声音，
然后通过浏览器和你的代码对话。不用写 JavaScript，不用写 CSS。

只需要几行 .NET 代码。我们开始吧。

### 快速开始

1. 新建一个 ASP.NET Core Web API 项目。
2. 安装 NuGet 包 **Gradio.Net**。
3. 把下面的任一示例放进 `Program.cs` 并运行。

#### 用 `gr.Interface`（最快的方式）

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

`gr.Interface` 把任意 C# 方法包成一个页面 —— 不需要布局代码。它就是 Python 版 quickstart 移植过来的版本：
Python 那边是同样的五行，只是把 `Greet` 写成了 `fn=greet`；这次移植由一条真正运行它的测试守着
（`GrPortingTests`）。

#### 用 `gr.Blocks`（完全掌控布局）

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

`using` 里的每一行 `gr.…(...)` 都是页面上的一行 —— 这就是 Python 版里的 `with gr.Blocks() as demo:`，
而 `gr.Row()` 同时是布局和它的作用域。

#### 放进已有的 ASP.NET Core 项目

`AddGradio()` 注册内核；写好页面，交给 `UseGradio(page)`，宿主就会把它服务出去 —— 路由、静态资源、
队列，一应俱全：

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

`gr.Interface` 的用法完全一样，因为它返回的同样是一个应用：

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

### 一套 API，两种写法

只有一个包，也只有一个应用。`gr` 是一组工厂，不是包装层：它交回来的就是核心类型。

**`gr` —— Python 文件原本就有的写法。** 名字都是 Python 版的，连参数名和默认值也一样，所以一个移植过来的文件
和 Python 相比只差三类改动：`=` 变成 `:`，`with` 变成 `using`，装饰器变成一次调用。上面那些示例用的就是
这一层，移植也从这一层开始。

**底下的类型 —— C# 会给它们的写法。** `gr.Textbox(...)` 返回一个 `Textbox`，`gr.Blocks()` 返回一个
`Blocks`，它们都是普通类型：事件是块上的方法（`button.Click(fn, inputs, outputs)`，布局则是
`tabs.Select(fn, inputs, outputs)`），组件列表是 `Block[]`，处理函数是委托。可以持有它们、传递它们、
继承某个组件 —— 没有任何东西被包装起来。

有意**不**公开的是手工往页面里写。`Blocks.BeginRender`、`Add`、`Reserve`、`Attach` 和 `Enter` 都是
程序集内部的：页面靠调用工厂来写，因为 `gr.Textbox(...)` 是页面上的一行而不是一个值，而
`using (gr.Blocks())` 就是那个右花括号结束的渲染过程。这条规则把 `demo.Add(gr.Textbox(...))` 挡在移植
之外，新写的代码同样适用 —— 往页面上放块只有一条路。

### 示例

| 示例 | 展示了什么 | 改这里 |
| --- | --- | --- |
| [Quickstart](Demo/Quickstart/) | `gr.Interface`：一个函数、两个输入、一个输出，没有布局代码。 | `Greet` —— 处理函数；页面由它的签名生成。 |
| [Blocks](Demo/Blocks/) | 手写的页面：`Row` 和 `Column`、两个文本框、一个按钮、两个事件，以及一个 `Tabs`/`Tab` 栏——它的 `select` 绑定了处理器，也能由处理器切换（`gr.Update`）。 | `Shout`，以及说明每个事件读写哪些组件的列表。 |
| [Chat](Demo/Chat/) | `gr.ChatInterface`，处理函数从某个端点流式取回答案。 | `Answer` —— 它发出的请求。`LocalModel.cs` 负责回答。 |
| [Streaming](Demo/Streaming/) | 生成器处理函数：每次 `yield` 替换输出，同时 `Progress` 推进进度条。 | `Type` —— 每一小段是什么，以及它们到达得多快。 |
| [Form](Demo/Form/) | 八个字段由一个按钮一起提交，其中一个字段决定另一个字段提供什么（`gr.Update`）。 | `Describe` —— 表单给出的回答。 |
| [Files](Demo/Files/) | 上传一个 CSV，把它的行显示成表格，再把同样的行下载成制表符分隔的文本。 | `Convert` —— 那个读、改、写一肩挑的函数。 |
| [Media](Demo/Media/) | 一张图、一段视频、一段声音交给处理函数再交回来 —— 不做转换 —— 外加视频播放器会发出的四个信号。 | `Same` —— 每一行文件都绑定的处理函数；`Count` —— 播放器的信号做什么。 |
| [Multipage](Demo/Multipage/) | 一个应用、多个页面：`demo.Route("About")`、提供导航的 navbar，以及每个页面上各有一个端点。 | 路由的名字，或它里面的 markdown。 |
| [Themes](Demo/Themes/) | `demo.Theme = gr.Themes.Soft(...)` —— 一行就是一个预设，上面还能再加八项选择。 | 预设，以及给它的两项选择。 |
| [Charts](Demo/Charts/) | 用处理函数返回的列画出的图表，以及原样透传的 plotly 绘图。 | `Visits` —— 那些列；`Bars` —— 那张绘图。 |

一共十个程序 —— quickstart、手写页面、聊天、流式、文件、表单、媒体、多页面、主题、图表。`cd` 进任意
一个目录跑 `dotnet run` 即可；每个都离线，每个都引用这棵树里的代码而不是已发布的包。
[`Demo/README.md`](Demo/README.md)（[中文](Demo/README.zh-CN.md)）说明了每个示例是什么，以及该改哪一行。

### 从 Python 过来

下面这张表就是翻译的全部内容：三条命名规则，然后几处 C# 逼着你换写法的地方。移植过来的文件不需要别的。

| Python | C# | 原因 |
| --- | --- | --- |
| `gr.Textbox(value="x")` | `gr.Textbox(value: "x")` | 命名参数用 `:` |
| `with gr.Blocks() as demo:` | `using (var demo = gr.Blocks())` | 没有上下文管理器 |
| `**kwargs` | `IDictionary<string, object?> props` | 没有关键字展开 |
| `@gr.render` / `@gr.mcp.tool` | `gr.Render(...)` / `gr.Mcp.Tool(...)` | 没有装饰器 —— 这两个成员都还不存在，见下面的表 |
| `def fn(..., progress=gr.Progress())` | 处理函数接受一个 `Progress` 参数 | 按注解注入 |
| `gr.on([btn.click, tb.change])` | `gr.On([btn.Events.Click, tb.Events.Change])` | 事件当作**值**用 |
| `tb.value` | `tb.Value` | 工厂返回的就是核心类型 |

成员名保留原来的词，只改大小写（`btn.click` → `btn.Click`，`demo.launch()` → `demo.Launch()`）；
触发点挪到 `Events` 下面，只是因为绑定方法已经占用了那个名字：绑定用 `btn.Click(...)`，合并用
`gr.On([btn.Events.Click, …])`。
