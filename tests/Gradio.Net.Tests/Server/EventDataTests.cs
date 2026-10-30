using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The payload an event carries about itself: which message a user picked, which key they pressed,
/// which file they deleted — and the handler actually receiving it.
/// </summary>
/// <remarks>
/// <para>
/// A binding's handler is handed the values of the components it reads, in order, and an
/// event-data parameter is filled from what the browser sent instead. That makes these classes part
/// of the protocol rather than a convenience: a type the runner does not know is a payload that
/// arrives as <see langword="null"/>, silently. "The class exists" and "the handler gets it" are
/// therefore two claims, and both are asserted here.
/// </para>
/// <para>
/// The runs go through the queue like any other, with the payload in the join body — the one seam
/// this framework tests on — so what is proven is the whole path: request, run, handler, output.
/// </para>
/// </remarks>
public sealed class EventDataTests
{
    /// <summary>The Python version's nine, by the name each class carries here.</summary>
    private static readonly string[] _upstreamFamily =
    [
        "CopyData",
        "DeletedFileData",
        "DownloadData",
        "EditData",
        "KeyUpData",
        "LikeData",
        "RetryData",
        "SelectData",
        "UndoData",
    ];

    [Fact]
    public void The_family_is_the_nine_upstream_declares()
    {
        var family = typeof(EventData).Assembly
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(EventData)))
            .Select(type => type.Name)
            .Order(StringComparer.Ordinal)
            .ToArray();

        // A family that grew or shrank is a decision somebody has to make: the Python version's list is what
        // an event can say about itself, and a class with no event behind it would be a payload
        // nobody can read.
        Assert.Equal(_upstreamFamily, family);
    }

    /// <summary>
    /// Every class of the family reaches a handler that asks for it, one run per class.
    /// </summary>
    /// <remarks>
    /// The runner fills an event-data parameter by its type, so a class the runner does not know
    /// arrives as <see langword="null"/> — silently, because nothing on the path can tell "this
    /// event carried nothing" from "nobody wired this type up". Listing the classes proves nothing
    /// about that; each one is run through a real binding here, and the handler answers with the
    /// class it was given. The two runs above prove the *fields* arrive; this proves the wiring.
    /// </remarks>
    [Fact]
    public async Task Every_class_of_the_family_reaches_a_handler_that_asks_for_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        (string Api, Func<Blocks, (Component Source, Textbox Output)> Build,
            Func<Component, Textbox, Delegate, string, Dependency> Bind,
            Delegate Handler, Type Data)[] cases =
        [
            ("selected", Chat,
                (source, output, fn, apiName) => ((Chatbot)source).Select(fn, [], [output], apiName: apiName),
                (Func<SelectData, string>)(data => data.GetType().Name), typeof(SelectData)),
            ("keyed", Field,
                (source, output, fn, apiName) => ((Dropdown)source).KeyUp(fn, [], [output], apiName: apiName),
                (Func<KeyUpData, string>)(data => data.GetType().Name), typeof(KeyUpData)),
            ("deleted", Files,
                (source, output, fn, apiName) => ((File)source).Delete(fn, [], [output], apiName: apiName),
                (Func<DeletedFileData, string>)(data => data.GetType().Name), typeof(DeletedFileData)),
            ("liked", Chat,
                (source, output, fn, apiName) => ((Chatbot)source).Like(fn, [], [output], apiName: apiName),
                (Func<LikeData, string>)(data => data.GetType().Name), typeof(LikeData)),
            ("retried", Chat,
                (source, output, fn, apiName) => ((Chatbot)source).Retry(fn, [], [output], apiName: apiName),
                (Func<RetryData, string>)(data => data.GetType().Name), typeof(RetryData)),
            ("undone", Chat,
                (source, output, fn, apiName) => ((Chatbot)source).Undo(fn, [], [output], apiName: apiName),
                (Func<UndoData, string>)(data => data.GetType().Name), typeof(UndoData)),
            ("edited", Chat,
                (source, output, fn, apiName) => ((Chatbot)source).Edit(fn, [], [output], apiName: apiName),
                (Func<EditData, string>)(data => data.GetType().Name), typeof(EditData)),
            ("downloaded", Files,
                (source, output, fn, apiName) => ((File)source).Download(fn, [], [output], apiName: apiName),
                (Func<DownloadData, string>)(data => data.GetType().Name), typeof(DownloadData)),
            ("copied", Chat,
                (source, output, fn, apiName) => ((Chatbot)source).Copy(fn, [], [output], apiName: apiName),
                (Func<CopyData, string>)(data => data.GetType().Name), typeof(CopyData)),
        ];

        foreach (var one in cases)
        {
            await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
            {
                using (blocks.BeginRender())
                {
                    var (source, output) = one.Build(blocks);

                    one.Bind(source, output, one.Handler, one.Api);
                }
            });

            using var queue = new QueueClient(host.BaseAddress);
            var fnIndex = await queue.EndpointAsync(one.Api, cancellationToken);

            var completed = await QueueRuns.CompletedAsync(
                queue,
                fnIndex,
                [],
                one.Api,
                cancellationToken,
                JsonNode.Parse("""{"index": 1, "value": "x", "key": "Enter", "input_value": "x", "file": {"path": "/tmp/x.txt"}}"""));

            Assert.Equal(one.Data.Name, completed["output"]!["data"]![0]!.GetValue<string>());
        }

        static (Component, Textbox) Chat(Blocks blocks)
        {
            var chat = blocks.Add(new Chatbot());
            var output = blocks.Add(new Textbox { Label = "Answer" });

            return (chat, output);
        }

        static (Component, Textbox) Field(Blocks blocks)
        {
            var field = blocks.Add(new Dropdown { Choices = [new Choice("abc")] });
            var output = blocks.Add(new Textbox { Label = "Answer" });

            return (field, output);
        }

        static (Component, Textbox) Files(Blocks blocks)
        {
            var files = blocks.Add(new File());
            var output = blocks.Add(new Textbox { Label = "Answer" });

            return (files, output);
        }
    }

    [Fact]
    public void Each_class_reads_the_payload_the_browser_sends()
    {
        Assert.Equal(2, new SelectData(Json("""{"index": 2, "value": "b"}""")).Index);
        Assert.Equal("b", new SelectData(Json("""{"index": 2, "value": "b"}""")).Value);

        // A selection says whether the entry was picked or unpicked, and a payload that says nothing
        // was a pick — which is the Python version's default, and what every component but a checkbox sends.
        Assert.True(new SelectData(Json("""{"index": 2, "value": "b"}""")).Selected);
        Assert.False(new SelectData(Json("""{"index": 2, "value": "b", "selected": false}""")).Selected);

        Assert.Equal("Enter", new KeyUpData(Json("""{"key": "Enter", "input_value": "abc"}""")).Key);
        Assert.Equal("abc", new KeyUpData(Json("""{"key": "Enter", "input_value": "abc"}""")).InputValue);

        Assert.True(new LikeData(Json("""{"index": 1, "liked": true}""")).Liked as bool?);

        // A file arrives as a structure, and is handed to the type the protocol already carries
        // files in rather than flattened to a path here.
        Assert.Equal("/tmp/gone.txt", new DeletedFileData(FilePayload("gone")).File?.Path);
        Assert.Equal("/tmp/taken.txt", new DownloadData(FilePayload("taken")).File?.Path);

        // A position is one number or a pair of them, depending on whether the exchange the user
        // acted on was a message on its own or a question with its answer.
        Assert.Equal([1, 2], new RetryData(Json("""{"index": [1, 2], "value": "hi"}""")).Indices);
        Assert.Equal(1, new RetryData(Json("""{"index": [1, 2], "value": "hi"}""")).Index);
        Assert.Equal("hi", new RetryData(Json("""{"index": [1, 2], "value": "hi"}""")).Value);

        Assert.Equal([3], new UndoData(Json("""{"index": 3}""")).Indices);
        Assert.Equal(3, new UndoData(Json("""{"index": 3}""")).Index);

        Assert.Equal([1, 2], new EditData(Json("""{"index": [1, 2], "value": "fixed"}""")).Indices);
        Assert.Equal("fixed", new EditData(Json("""{"index": [1, 2], "value": "fixed"}""")).Value);

        Assert.Equal("copied", new CopyData(Json("""{"value": "copied"}""")).Value);
    }

    [Fact]
    public async Task A_handler_that_asks_which_message_was_picked_receives_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        Chatbot chat = null!;
        Textbox answer = null!;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                chat = blocks.Add(new Chatbot());
                answer = blocks.Add(new Textbox { Label = "Answer" });
            }

            chat.Select(
                (Func<SelectData, string>)(data => $"picked {data.Index}: {data.Value}"),
                inputs: [],
                outputs: [answer],
                apiName: "picked");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("picked", cancellationToken);

        var completed = await QueueRuns.CompletedAsync(
            queue,
            fnIndex,
            [],
            "selecting",
            cancellationToken,
            JsonNode.Parse("""{"index": 1, "value": "the second one"}"""));

        Assert.Equal("picked 1: the second one", completed["output"]!["data"]![0]!.GetValue<string>());
    }

    [Fact]
    public async Task A_handler_that_asks_which_key_was_pressed_receives_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        Dropdown field = null!;
        Textbox answer = null!;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                field = blocks.Add(new Dropdown { Choices = [new Choice("abc"), new Choice("def")] });
                answer = blocks.Add(new Textbox { Label = "Answer" });
            }

            field.KeyUp(
                (Func<KeyUpData, string>)(data => $"{data.Key} after '{data.InputValue}'"),
                inputs: [],
                outputs: [answer],
                apiName: "keyed");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("keyed", cancellationToken);

        var completed = await QueueRuns.CompletedAsync(
            queue,
            fnIndex,
            [],
            "typing",
            cancellationToken,
            JsonNode.Parse("""{"key": "Enter", "input_value": "abcd"}"""));

        Assert.Equal("Enter after 'abcd'", completed["output"]!["data"]![0]!.GetValue<string>());
    }

    /// <summary>
    /// A handler that names the base class receives the payload itself.
    /// </summary>
    /// <remarks>
    /// One event's payload is <see cref="EventData"/> and nothing more — the Python version's
    /// <c>click</c> carries a <c>gr.EventData</c> — so a handler that names the base has to be
    /// filled rather than handed <see langword="null"/>. The same question is what a custom HTML
    /// component's own script raises, which is why the base is worth being able to name.
    /// </remarks>
    [Fact]
    public async Task A_handler_that_names_the_base_class_receives_the_payload()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            Textbox field;
            Textbox answer;

            using (blocks.BeginRender())
            {
                field = blocks.Add(new Textbox { Label = "A" });
                answer = blocks.Add(new Textbox { Label = "Answer" });
            }

            // The selection is what a textbox raises when the user selects text in it, and the two
            // overloads below are the ones a textbox did not have before this batch.
            field.Select(
                (Func<EventData, string>)(data => $"raw: {data.Raw!.ToJsonString()}"),
                inputs: [field],
                outputs: [answer],
                apiName: "selected");

            field.Copy(
                (Func<CopyData, string>)(data => $"copied: {data.Value}"),
                inputs: [field],
                outputs: [answer],
                apiName: "copied");
        });

        using var queue = new QueueClient(host.BaseAddress);

        var selected = await QueueRuns.CompletedAsync(
            queue,
            await queue.EndpointAsync("selected", cancellationToken),
            ["hello"],
            "selecting",
            cancellationToken,
            JsonNode.Parse("""{"index": 0, "value": "hel"}"""));

        Assert.Equal(
            """raw: {"index":0,"value":"hel"}""",
            selected["output"]!["data"]![0]!.GetValue<string>());

        var copied = await QueueRuns.CompletedAsync(
            queue,
            await queue.EndpointAsync("copied", cancellationToken),
            ["hello"],
            "copying",
            cancellationToken,
            JsonNode.Parse("""{"value": "hel"}"""));

        Assert.Equal("copied: hel", copied["output"]!["data"]![0]!.GetValue<string>());
    }

    private static JsonNode Json(string json) => JsonNode.Parse(json)!;

    /// <summary>A deletion or download payload, whose one field is a file.</summary>
    /// <param name="name">The file's name, which the assertion reads back.</param>
    private static JsonNode FilePayload(string name) =>
        JsonNode.Parse("""{"file": {"path": "/tmp/""" + name + """.txt", "meta": {"type": "gradio.FileData"}}}""")!;
}
