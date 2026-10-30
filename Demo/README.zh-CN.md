# 示例

**[English](README.md)** | **简体中文**

十个程序，每个只讲一件事。`cd` 进其中一个目录再运行：

```bash
cd Demo/Quickstart
dotnet run          # 页面在 http://127.0.0.1:7860
```

在仓库根目录执行 `dotnet run --project Demo/Quickstart` 效果一样。Ctrl-C 停止。

每个示例都是独立的程序，而不是共享工程里的一个文件；每个都引用 `src/Gradio.Net/Gradio.Net.csproj`
而不是已发布的包 —— 所以示例跑的就是这棵树里的代码，而一个示例就是一个可以整个复制走再改的目录。

| 示例 | 展示了什么 | 改这里 |
| --- | --- | --- |
| [Quickstart](Quickstart/) | `gr.Interface`：一个函数、两个输入、一个输出，没有布局代码。 | `Greet` —— 处理函数；页面由它的签名生成。 |
| [Blocks](Blocks/) | 手写的页面：`Row` 和 `Column`、两个文本框、一个按钮、两个事件，以及一个 `Tabs`/`Tab` 栏。 | `Shout`，以及说明每个事件读写哪些组件的列表。 |
| [Chat](Chat/) | `gr.ChatInterface`，处理函数从某个端点流式取回答案。 | `Answer` —— 它发出的请求。`LocalModel.cs` 负责回答。 |
| [Streaming](Streaming/) | 生成器处理函数：每次 `yield` 替换输出，同时 `Progress` 推进进度条。 | `Type` —— 每一小段是什么，以及它们到达得多快。 |
| [Form](Form/) | 八个字段由一个按钮一起提交，其中一个字段决定另一个字段提供什么（`gr.Update`）。 | `Describe` —— 表单给出的回答。 |
| [Files](Files/) | 上传一个 CSV，把它的行显示成表格，再把同样的行下载成制表符分隔的文本。 | `Convert` —— 那个读、改、写一肩挑的函数。 |
| [Media](Media/) | 一张图、一段视频、一段声音交给处理函数再交回来 —— 不做转换 —— 外加视频播放器会发出的四个信号。 | `Same` —— 每一行文件都绑定的处理函数；`Count` —— 播放器的信号做什么。 |
| [Multipage](Multipage/) | 一个应用、多个页面：`demo.Route("About")`、提供导航的 navbar，以及每个页面上各有一个端点。 | 路由的名字，或它里面的 markdown。 |
| [Themes](Themes/) | `demo.Theme = gr.Themes.Soft(...)` —— 一行就是一个预设，上面还能再加八项选择。 | 预设，以及给它的两项选择。 |
| [Charts](Charts/) | 用处理函数返回的列画出的图表，以及原样透传的 plotly 绘图。 | `Visits` —— 那些列；`Bars` —— 那张绘图。 |

## 这些是拿来读的

这些示例是写来读、写来改的，所以它们不同时承担「协议一漂移就失败」的职责。关于它们被检查的是：每个都是
一个程序、构建会编译它、并且它不访问回环之外的地址、也不读取配置或环境变量；见 `DemoProjectTests`。

## 天然离线

没有示例读取 token，也没有示例访问回环之外的地址。唯一一个要和服务器说话的示例自己起一个服务器：
`Chat/LocalModel.cs` 像一个 OpenAI 兼容端点那样回答，监听的是操作系统挑的端口，所以不会和你已有的
服务器撞车。

真实模型就是同一个处理函数换一个地址 —— 这正是那个示例的意义：指向你自己的服务时，变的只有地址，
别的什么都不变。

## 同时跑两个

除非另有指定，每个示例都启动在 7860 上，所以要么一个一个跑，要么给第二个换端口：

```csharp
demo.Launch(serverPort: 7861);
```

## .NET 版本

示例面向 `net10.0`，也就是本仓库构建用的 SDK。包本身面向 `net8.0`、`net9.0` 和 `net10.0`，
所以要在更老的 SDK 上跑示例，把它 `.csproj` 里的 `TargetFramework` 改成那个版本即可。
