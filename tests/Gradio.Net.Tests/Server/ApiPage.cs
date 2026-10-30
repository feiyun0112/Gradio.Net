using Gradio.Net;
namespace Gradio.Net.Tests.Server;

/// <summary>
/// An application whose endpoints are described by its handlers' signatures rather than by
/// components: the shape <c>gr.api</c> exists for.
/// </summary>
/// <remarks>
/// <para>
/// Seven endpoints, chosen so that each says something a component could not: one that takes and
/// returns the primitive types, one that takes a collection and returns a tuple, one that is
/// deliberately not exposed, one that is callable and left out of the documentation, one that takes
/// a file, and the two an agent reads instead of calling — a resource and a prompt. The same
/// application is what the API documents and the MCP endpoint are observed against, because both are
/// built from the one list of endpoints and a difference between them would be a difference in what
/// the application offers.
/// </para>
/// <para>
/// The handlers are methods rather than lambdas, and that is the point rather than a style: a
/// method's parameters carry the names a client calls them by, while a lambda's do not exist. A
/// lambda would still be described — its parameters would be named by position — but it would not
/// show what an application that names its arguments gets.
/// </para>
/// </remarks>
internal static class ApiPage
{
    /// <summary>The title the application serves.</summary>
    internal const string Title = "Api App";

    /// <summary>The name of the endpoint that takes a collection and returns a tuple.</summary>
    internal const string SliceName = "add_and_slice";

    /// <summary>The name of the endpoint nothing may call.</summary>
    internal const string HiddenName = "hidden";

    /// <summary>The name of the endpoint that is callable and left out of the documentation.</summary>
    internal const string QuietName = "quiet";

    /// <summary>Writes the application.</summary>
    /// <param name="blocks">The application to write into.</param>
    /// <param name="mcp">Whether the application also offers itself to agents.</param>
    internal static void Build(Blocks blocks, bool mcp = false)
    {
        using (blocks.BeginRender())
        {
            blocks.Title = Title;
            blocks.McpServer = mcp;

            blocks.Api(new Func<int, int, int>(Add), apiName: "add");

            blocks.Api(
                new Func<int, int, IReadOnlyList<int>, (int, string)>(AddAndSlice),
                apiName: SliceName, apiDescription: "Adds two numbers and slices a list.");

            blocks.Api(new Func<string, string>(Hidden), apiName: HiddenName, apiVisibility: ApiVisibility.Private);

            blocks.Api(new Func<string, string>(Quiet), apiName: QuietName, apiVisibility: ApiVisibility.Undocumented);

            blocks.Api(new Func<string, string>(Weather));

            blocks.Api(new Func<string, int, string>(Summarise));

            blocks.Api(new Func<string, string, string>(Greet));

            blocks.Api(new Func<FileData, string>(DescribeFile));
        }
    }

    /// <summary>Adds two numbers.</summary>
    /// <param name="a">The first.</param>
    /// <param name="b">The second.</param>
    /// <returns>The sum.</returns>
    /// <remarks>
    /// The one tool that also reports its answer as structured content, which is the half of a tool
    /// descriptor a signature cannot say and an attribute has to.
    /// </remarks>
    [McpTool(StructuredOutput = true)]
    internal static int Add(int a, int b) => a + b;

    /// <summary>Adds two numbers and slices a list with them.</summary>
    /// <param name="a">Where the slice starts.</param>
    /// <param name="b">Where it ends.</param>
    /// <param name="items">What to slice.</param>
    /// <returns>The sum, and the slice.</returns>
    /// <remarks>
    /// The handler is typed to prove that what it receives is what it declared: a JSON number
    /// arrives as a double, so a handler that added them without the conversion would be a handler
    /// this framework could not call.
    /// </remarks>
    internal static (int, string) AddAndSlice(int a, int b, IReadOnlyList<int> items) =>
        (a + b, string.Join(",", items.Skip(a).Take(b - a)));

    /// <summary>An endpoint nothing may call.</summary>
    /// <param name="text">Anything.</param>
    /// <returns>The text.</returns>
    internal static string Hidden(string text) => text;

    /// <summary>An endpoint that is callable and left out of the documentation.</summary>
    /// <param name="text">Anything.</param>
    /// <returns>The text.</returns>
    internal static string Quiet(string text) => text;

    /// <summary>The weather in a city, which an agent reads rather than calls.</summary>
    /// <param name="city">The city.</param>
    /// <returns>What the weather is.</returns>
    [McpResource("weather://{city}", Description = "The weather in a city.", MimeType = "text/plain")]
    internal static string Weather(string city) => $"Sunny in {city}";

    /// <summary>A prompt an agent fills in.</summary>
    /// <param name="text">What to summarise.</param>
    /// <param name="maxWords">How long the summary may be.</param>
    /// <returns>The instruction.</returns>
    [McpPrompt(Description = "Summarise a text.")]
    internal static string Summarise(string text, int maxWords = 20) =>
        $"Summarise the following in at most {maxWords} words: {text}";

    /// <summary>Greets someone, with a greeting the caller may leave out.</summary>
    /// <param name="name">Who to greet.</param>
    /// <param name="greeting">What to greet them with.</param>
    /// <returns>The greeting.</returns>
    /// <remarks>
    /// The default is what makes an argument optional in a tool's schema, and a call that omits it
    /// has to run as if it had been spelled out — which is what a test can only see through a value
    /// that ends up in the answer.
    /// </remarks>
    internal static string Greet(string name, string greeting = "Hello") => $"{greeting}, {name}";

    /// <summary>Describes the file it is given.</summary>
    /// <param name="file">The file, which an agent sends as an address.</param>
    /// <returns>Where the file is.</returns>
    internal static string DescribeFile(FileData file) => file.Path ?? string.Empty;
}
