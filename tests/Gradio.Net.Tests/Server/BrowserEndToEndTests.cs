using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Playwright;
using Xunit;
using Timer = Gradio.Net.Timer;
namespace Gradio.Net.Tests.Server;

/// <summary>
/// The protocol layer, observed the only way it can be: a browser loads the served page, runs the
/// vendored frontend, and a person's input comes back as a person's output.
/// </summary>
/// <remarks>
/// <para>
/// Every other test in this project asks the server questions. This one asks the client: the page
/// is loaded by a real browser, the compiled frontend boots from the config it finds there, and a
/// click travels the whole path — <c>queue/join</c>, the event stream, the answer written back into
/// a component. That path is the reason the frontend can be the Python version's own bundle, byte
/// for byte, and no document comparison can stand in for it.
/// </para>
/// <para>
/// The browser is one that is already installed — Chrome, then Edge — so nothing is downloaded and
/// the test stage needs no browser provisioning. Where neither is present the test skips rather
/// than fails: this test is the end-to-end confirmation where a browser exists.
/// </para>
/// </remarks>
public sealed class BrowserEndToEndTests
{
    /// <summary>The channels to try, in the order a machine is likely to have them.</summary>
    private static readonly string[] _channels = ["chrome", "msedge"];

    [Fact]
    public async Task Typing_into_the_served_page_and_clicking_its_button_produces_the_answer()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ClickExample.Build);

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await ClickThroughAsync(page, host.BaseAddress);

        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// The smallest application there is — one field, nothing bound to it — loads and takes typing.
    /// </summary>
    /// <remarks>
    /// The first vertical line of the generated mapping seen from the browser: a block is declared,
    /// the source generator turns the Python parameter list into props, the config carries them,
    /// and the vendored frontend draws a field from that config alone. Nothing else is on the page,
    /// so a
    /// failure here is about the protocol rather than about a component's parameters — which is
    /// what makes it the case to keep working before any of the larger ones.
    /// </remarks>
    [Fact]
    public async Task A_page_holding_one_textbox_boots_and_takes_typing()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(new Textbox { Label = "Name", Placeholder = "type here" });
                }
            });

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        // The field is drawn by the frontend from the config, so waiting for it is waiting for the
        // protocol to have worked. One field on the page, and one only: a config that described a
        // second block would show up here.
        var fields = page.Locator("textarea, input");
        await fields.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await Assertions.Expect(fields).ToHaveCountAsync(1);

        // The placeholder the application set, which is the generated mapping's work arriving in
        // the document: the member, the prop it becomes, and the attribute the frontend draws from
        // it are three steps, and this is the last one.
        await Assertions.Expect(fields.First).ToHaveAttributeAsync("placeholder", "type here");

        await fields.First.FillAsync("Ada");

        Assert.Equal("Ada", await fields.First.InputValueAsync());
        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// The application's own <c>load</c> handler runs when the page opens, once per session.
    /// </summary>
    /// <remarks>
    /// The Python version's <c>demo.load</c> is a binding on the application rather than on a
    /// component, so nothing a document comparison looks at can show it working: the target is the
    /// root, and it is the frontend that raises the signal once it has mounted. What the page shows
    /// afterwards is the proof — a value the handler computed on the server, in a field nobody
    /// typed into.
    /// </remarks>
    [Fact]
    public async Task The_page_runs_what_it_bound_to_its_own_opening()
    {
        var runs = 0;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                Textbox field;

                using (blocks.BeginRender())
                {
                    field = blocks.Add(new Textbox { Label = "Loaded" });
                }

                // The Python version's `demo.load`: a handler on the application itself, run when the page has
                // mounted. Its target is the root, so the frontend is what dispatches it — which is
                // the half no document comparison can show.
                blocks.Load(
                    (Func<string>)(() =>
                    {
                        Interlocked.Increment(ref runs);

                        return "ready";
                    }),
                    outputs: [field]);
            });

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        var field = page.Locator("textarea, input").First;
        await field.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        // The value the application computed on the server is in the field: the page raised the
        // signal, the run went through the queue, and the answer was written back.
        await Eventually.IsTrueAsync(
            async () => await field.InputValueAsync() == "ready",
            TimeSpan.FromSeconds(20),
            TestContext.Current.CancellationToken);

        Assert.Equal("ready", await field.InputValueAsync());

        // Once, not once per heartbeat and not once per run of something else: the session is the
        // scope of this signal, and it has exactly one opening.
        Assert.Equal(1, runs);
        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// The collector the other tests end with notices what a broken page would report.
    /// </summary>
    /// <remarks>
    /// Every other test here asserts that nothing went wrong, which is only worth something if the
    /// thing doing the watching can see it. Nothing on a working page logs an error or asks for a
    /// file that is not there, so this page is made to do both — the console message from the page
    /// itself, the missing asset by asking for one — and the collector has to report both.
    /// </remarks>
    [Fact]
    public async Task The_failure_collector_notices_a_console_error_and_a_missing_resource()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ClickExample.Build);

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        await page.EvaluateAsync("() => console.error('the collector should see this')");
        await page.EvaluateAsync("() => fetch('/assets/not-in-the-tree.txt')");

        var sawBoth = await Eventually.IsTrueAsync(
            () => Task.FromResult(failures.Entries.Count >= 2),
            TimeSpan.FromSeconds(15),
            TestContext.Current.CancellationToken);

        Assert.True(
            sawBoth,
            $"The collector reported: {string.Join("; ", failures.Entries.Select(entry => $"{entry.Kind}: {entry.Detail}"))}");

        Assert.Contains(failures.Entries, entry => entry.Kind == PageFailureKind.Console);
        Assert.Contains(
            failures.Entries,
            entry => entry.Kind == PageFailureKind.Resource && entry.Detail.Contains("404", StringComparison.Ordinal));
    }

    /// <summary>
    /// The same interaction, with the page rendered by the SSR runtime instead of by the .NET
    /// server.
    /// </summary>
    /// <remarks>
    /// <para>
    /// SSR puts a second server in front of this one, and the page it serves is a different
    /// document: a SvelteKit render that hydrates into the same frontend. Everything the browser
    /// then asks for goes through that proxy, so a click reaching an answer is the proof that the
    /// arrangement is transparent rather than merely similar.
    /// </para>
    /// <para>
    /// Skips where there is no runtime or no browser, as the tests above skip without one.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task Typing_into_the_page_the_ssr_runtime_renders_produces_the_answer()
    {
        var node = SsrNode.Locate();
        Assert.SkipUnless(node is not null, "no Node 20+ runtime is installed, so SSR cannot be exercised here.");

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ClickExample.Build,
            options: options =>
            {
                options.SsrMode = true;
                options.SsrNodePath = node;
            });

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await ClickThroughAsync(page, host.BaseAddress);

        Assert.Empty(failures.Entries);
    }

    /// <summary>Loads the page, types, clicks the button and reads the answer.</summary>
    /// <param name="page">The browser page to drive.</param>
    /// <param name="address">Where the application is served.</param>
    /// <returns>A task that completes once the answer is on screen.</returns>
    private static async Task ClickThroughAsync(IPage page, Uri address)
    {
        await page.GotoAsync(address.ToString());

        // The two fields appear only once the frontend has booted and read the config, so waiting
        // for them is waiting for the protocol to have worked.
        var fields = page.Locator("textarea, input");
        await fields.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        Assert.Equal(2, await fields.CountAsync());

        await fields.First.FillAsync(ClickExample.Typed);

        // The button the textbox draws for itself, which is what a person clicks.
        var submit = page.Locator("button.submit-button");
        await submit.First.ClickAsync();

        await page.WaitForFunctionAsync(
            $"() => [...document.querySelectorAll('textarea, input')][1]?.value === '{ClickExample.Answered}'");

        Assert.Equal(ClickExample.Typed, await fields.First.InputValueAsync());
        Assert.Equal(ClickExample.Answered, await fields.Nth(1).InputValueAsync());
    }

    /// <summary>
    /// A streamed answer is visible while it is being written: the chat shows the first piece
    /// before the server has produced the second.
    /// </summary>
    /// <remarks>
    /// This is the claim a chat user makes that no message comparison can settle: that the answer
    /// appears incrementally rather than all at once when the handler finishes. The handler holds
    /// its second piece back until the test releases it, so "the first piece is on screen while the
    /// second does not exist yet" is an observation rather than a race.
    /// </remarks>
    [Fact]
    public async Task A_streamed_answer_appears_in_the_chat_before_it_is_finished()
    {
        var example = new ChatStreamExample();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            example.Build);

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        var prompt = page.Locator("textarea, input").First;
        await prompt.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await prompt.FillAsync(ChatStreamExample.Prompt);
        await page.Locator("button.submit-button").First.ClickAsync();

        // The first piece reaches the chat while the handler is still inside the stream.
        await page.GetByText(ChatStreamExample.First).First.WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        Assert.Empty(await page.GetByText(ChatStreamExample.Second).AllAsync());

        // And the second piece arrives once the handler produces it, in the same bubble: the
        // frontend applied the edit rather than drawing a second message.
        example.Release();

        await page.GetByText(ChatStreamExample.Second).First.WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// The navbar draws the application's pages and switching between them loads the other page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A multipage application is only multipage in the browser: the navbar is drawn by the
    /// frontend from the <c>pages</c> array and the navbar's own props, and a link is an ordinary
    /// navigation to the other page's URL. Nothing in a document comparison can show that the two
    /// halves meet — that the pages the server names are the pages the frontend offers, and that
    /// following one of them lands on that page's components.
    /// </para>
    /// <para>
    /// The application offers two pages and serves three: the third is reachable by URL and left
    /// out of the navbar, which is what the second assertion is about.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task The_navbar_offers_the_pages_and_switching_between_them_loads_the_other_page()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            Multipage.Build);

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        // The navbar is drawn from the pages the config names, and from the navbar component's own
        // props once that component has mounted — which is what renames the home page to "Start".
        // Waiting for that name is waiting for both halves to have arrived; the links themselves
        // appear as soon as the pages do.
        var startLink = page.Locator(".nav-holder a", new() { HasTextString = "Start" });
        await startLink.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        // The page that asked not to be offered is served but not drawn.
        Assert.Empty(await page.Locator(".nav-holder a", new() { HasTextString = "Internal" }).AllAsync());

        var aboutLink = page.Locator(".nav-holder a", new() { HasTextString = "About" });
        await aboutLink.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        // Following it is a navigation, not a re-render: the frontend reloads the page, and the
        // server answers the other page's URL with the other page's document. The click is
        // dispatched and the address bar is watched, rather than waiting for a click-triggered
        // navigation to reach its load state: the page keeps a stream open for the life of the
        // page, and the load state is not what this test is about — the address it landed on is.
        await aboutLink.EvaluateAsync("anchor => anchor.click()");

        var moved = await Eventually.IsTrueAsync(
            () => Task.FromResult(page.Url.EndsWith("/about", StringComparison.Ordinal)),
            TimeSpan.FromSeconds(10),
            TestContext.Current.CancellationToken);

        Assert.True(moved, $"the browser is at '{page.Url}' after following the navbar's About link");

        await page.GetByText("About the row").First.WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        // The home page's components are not on this page: each URL carries its own document, and
        // the frontend renders what that document holds.
        Assert.Empty(await page.GetByText("Name", new() { Exact = true }).AllAsync());

        // The navbar names the page it is on: the link the browser is looking at is the marked one,
        // which is how the frontend tells a reader which page of the application this is.
        var active = page.Locator(".nav-holder a.active");

        await active.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        Assert.Equal("About", (await active.First.TextContentAsync())?.Trim());

        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// Every component of the text and number family takes input from a person, in a browser.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The manifest page proves what the components say to the frontend; this asks the frontend
    /// what it makes of it. Each assertion is a person's action and the field's answer: text typed
    /// into a textbox, a slider nudged with the keyboard, a box ticked, an option chosen from a
    /// list, and the two components whose value is a set of ticked boxes.
    /// </para>
    /// <para>
    /// The code editor is asserted by what it draws rather than by typing into it: its content is a
    /// highlighted document rather than a text field, and what this case has to show is that the
    /// value reached it.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task Every_component_of_the_text_and_number_family_takes_input_in_the_browser()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            InputFamily.Build);

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        // The textbox: text a person types is the text the field holds.
        var name = page.GetByLabel("Name");
        await name.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await name.FillAsync(InputFamily.Typed);
        Assert.Equal(InputFamily.Typed, await name.InputValueAsync());

        // The number field: the same, with a number in it.
        var age = page.GetByLabel("Age");
        await age.FillAsync(InputFamily.Counted);
        Assert.Equal(InputFamily.Counted, await age.InputValueAsync());

        // The slider: one step to the right is one step on the range the application gave.
        var level = page.Locator("input[type='range']").First;
        await level.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        var before = await level.InputValueAsync();
        await level.FocusAsync();
        await page.Keyboard.PressAsync("ArrowRight");
        var after = await level.InputValueAsync();
        Assert.NotEqual(before, after);

        // The checkbox: a click ticks it.
        var agree = page.GetByLabel("Agree");
        await agree.CheckAsync();
        Assert.True(await agree.IsCheckedAsync());

        // The checkbox group: the option is a label a person clicks, and the box inside it is what
        // the component reads.
        await page.GetByText("alpha", new PageGetByTextOptions { Exact = true }).First.ClickAsync();
        Assert.True(await page.Locator("input[type='checkbox'][name='alpha']").First.IsCheckedAsync());

        // The dropdown: opening it offers the choices, and choosing one puts it in the field.
        var size = page.GetByLabel("Size");
        await size.ClickAsync();
        await page.Locator($"[data-testid='dropdown-option'][aria-label='{InputFamily.Chosen}']").First.ClickAsync();
        Assert.Equal(InputFamily.Chosen, await size.InputValueAsync());

        // The radio: choosing an option is a click on its label.
        await page.GetByText(InputFamily.Picked, new PageGetByTextOptions { Exact = true }).First.ClickAsync();
        Assert.True(await page.Locator($"input[type='radio'][value='{InputFamily.Picked}']").First.IsCheckedAsync());

        // The multimodal field: the text half is a text area a person types into.
        var message = page.Locator(".multimodal-textbox textarea").First;
        await message.FillAsync(InputFamily.Message);
        Assert.Equal(InputFamily.Message, await message.InputValueAsync());

        // The date and time field: the moment typed into it is the moment it holds.
        var when = page.Locator(".timebox input.time").First;
        await when.FillAsync(InputFamily.Moment);
        Assert.Equal(InputFamily.Moment, await when.InputValueAsync());

        // The colour picker: the swatch opens a picker, and the picker has a field to type a colour
        // into.
        await page.Locator(".dialog-button").First.ClickAsync();

        var tint = page.Locator(".color-picker input[type='text']").First;
        await tint.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await tint.FillAsync(InputFamily.Tint);
        Assert.Equal(InputFamily.Tint, await tint.InputValueAsync());

        // The code editor: the value the application gave is what it draws.
        await page.GetByText("x = 1", new PageGetByTextOptions { Exact = false }).First.WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        // The coloured checkbox group: its own template draws the boxes, and a click on one of them
        // is what its JavaScript turns into the component's value.
        var ticked = page.Locator($"input[type='checkbox'][value='{InputFamily.Ticked}']").First;
        await ticked.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await ticked.CheckAsync();
        Assert.True(await ticked.IsCheckedAsync());

        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// The button family does what each button is for, in a browser.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Four behaviours, four observations. The clear button empties the fields it was given without
    /// asking the server; the upload button sends a file to the upload endpoint and reports it; the
    /// download button hands the browser a file; and the share button asks the server for a link and
    /// shows the label it was given for having copied one, then goes back to its own.
    /// </para>
    /// <para>
    /// The share button's click is also a request, and it is asserted as one: the script the button
    /// binds fetches the deep-link endpoint, so a page whose endpoint were missing would copy an
    /// address with nothing in it. A request that answers 4xx or 5xx anywhere on the page counts as
    /// a failure here like any other — including the one the second click makes, which is why that
    /// route had to be built before this test could keep its promise.
    /// </para>
    /// <para>
    /// What is not asserted is the label coming back a second later. That is the button's second
    /// click: a run on the server whose result arrives through the queue, which the queue's own
    /// tests observe. Here the request is what is asserted, and that it is answered.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task The_button_family_clears_uploads_downloads_and_copies_a_link()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ButtonFamily.Build);

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        var deepLinks = new List<string>();
        var restores = new List<string>();
        page.Response += (_, response) =>
        {
            if (response.Url.Contains("/gradio_api/deep_link", StringComparison.Ordinal))
            {
                deepLinks.Add(response.Url);
            }

            if (response.Url.Contains("/gradio_api/run/", StringComparison.Ordinal))
            {
                restores.Add(response.Url);
            }
        };

        await page.GotoAsync(host.BaseAddress.ToString());

        var name = page.GetByLabel("Name");
        await name.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await name.FillAsync(ButtonFamily.Typed);
        await page.GetByLabel("Count").FillAsync(ButtonFamily.Counted);

        Assert.Equal(ButtonFamily.Typed, await name.InputValueAsync());

        // The clear button: the fields it names are empty afterwards, and the ones it does not name
        // are not touched.
        await page.GetByRole(AriaRole.Button, new() { NameString = ButtonFamily.Cleared }).ClickAsync();

        var cleared = await Eventually.IsTrueAsync(
            async () => await name.InputValueAsync() == string.Empty
                && await page.GetByLabel("Count").InputValueAsync() == string.Empty,
            TimeSpan.FromSeconds(10),
            TestContext.Current.CancellationToken);

        Assert.True(cleared, "the clear button emptied the fields it was given");

        // The upload button: a file a person picks is uploaded, and the handler's answer is drawn.
        // The picker is found by the button it belongs to, which is what the frontend names it after.
        var picker = page.GetByTestId("Upload one-upload-button");

        await picker.SetInputFilesAsync(
            new FilePayload
            {
                Name = ButtonFamily.Uploaded,
                MimeType = "text/plain",
                Buffer = "the bytes a person uploads"u8.ToArray(),
            });

        var got = page.GetByLabel("Got");

        var uploaded = await Eventually.IsTrueAsync(
            async () => await got.InputValueAsync() == ButtonFamily.Uploaded,
            TimeSpan.FromSeconds(20),
            TestContext.Current.CancellationToken);

        Assert.True(uploaded, "the upload button reported the file it was given");

        // The second upload button takes several files, and the browser is told which kinds to
        // offer: that is what the application's `file_types` becomes, and what a person's file
        // dialog filters by.
        // The second button's picker, and the filter the application gave it: that is what a
        // person's file dialog is offered, and what the server refuses a file against.
        var many = page.GetByTestId("Upload many-upload-button");

        await many.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached });

        Assert.Equal(".txt", await many.GetAttributeAsync("accept"));
        Assert.NotNull(await many.GetAttributeAsync("multiple"));

        await many.SetInputFilesAsync(
        [
            new FilePayload
            {
                Name = "first.txt",
                MimeType = "text/plain",
                Buffer = "one"u8.ToArray(),
            },
            new FilePayload
            {
                Name = "second.txt",
                MimeType = "text/plain",
                Buffer = "two"u8.ToArray(),
            },
        ]);

        var listed = page.GetByLabel("Many");

        Assert.True(
            await Eventually.IsTrueAsync(
                async () => await listed.InputValueAsync() == "2: first.txt, second.txt",
                TimeSpan.FromSeconds(20),
                TestContext.Current.CancellationToken),
            $"the multiple upload handed the handler {await listed.InputValueAsync()}");

        // The download button: clicking it hands the browser a file with the name the file has and
        // the bytes it holds.
        var served = await GetAsync();

        Assert.Equal(ButtonFamily.Downloaded, served.Name);
        Assert.Equal(ButtonFamily.DownloadedText, served.Text);

        // A file a handler made: the button serves what the handler answered, not what the page was
        // built with — which is the half of a download that a page's own value cannot show.
        await page.GetByRole(AriaRole.Button, new() { NameString = "Prepare" }).ClickAsync();

        var ready = page.GetByLabel("Status");

        Assert.True(
            await Eventually.IsTrueAsync(
                async () => await ready.InputValueAsync() == ButtonFamily.Ready,
                TimeSpan.FromSeconds(20),
                TestContext.Current.CancellationToken),
            "the prepare button's run finished");

        var handed = await GetAsync();

        Assert.Equal(ButtonFamily.Handed, handed.Name);
        Assert.Equal(ButtonFamily.HandedText, handed.Text);

        // The share button: one request for a link, the label it was given, and the label it
        // started with once the second click has run.
        var share = page.GetByRole(AriaRole.Button, new() { NameString = "Share" });

        await share.ClickAsync();

        var copied = await Eventually.IsTrueAsync(
            async () => await page.GetByRole(AriaRole.Button, new() { NameString = ButtonFamily.Copied })
                .CountAsync() == 1,
            TimeSpan.FromSeconds(10),
            TestContext.Current.CancellationToken);

        Assert.True(copied, "the share button showed the label it copies under");

        // The response is raised on the driver's thread, so the label can change a moment before
        // the request is reported here; waiting for the report keeps the assertion about the
        // request rather than about which thread got there first.
        var askedForLink = await Eventually.IsTrueAsync(
            () => Task.FromResult(deepLinks.Count == 1),
            TimeSpan.FromSeconds(10),
            TestContext.Current.CancellationToken);

        Assert.True(askedForLink, "the share button asked the server for a link");
        Assert.Single(deepLinks);

        // The second click is the one that puts the label back, and it is a server run rather than
        // a script: it reached the route, which is as far as this batch goes.
        var asked = await Eventually.IsTrueAsync(
            () => Task.FromResult(restores.Count == 1),
            TimeSpan.FromSeconds(10),
            TestContext.Current.CancellationToken);

        Assert.True(asked, "the share button asked the server to put its label back");

        Assert.Empty(failures.Entries);

        // Clicking the download button and reading what the browser got: the name is what the file
        // is called, and the text is what it holds — the half a name cannot show.
        async Task<(string Name, string Text)> GetAsync()
        {
            var download = await page.RunAndWaitForDownloadAsync(() =>
                page.GetByRole(AriaRole.Button, new() { NameString = "Get" }).ClickAsync());

            var path = await download.PathAsync();

            Assert.NotNull(path);

            return (download.SuggestedFilename, await System.IO.File.ReadAllTextAsync(
                path!,
                TestContext.Current.CancellationToken));
        }
    }

    /// <summary>
    /// A recording made in the browser reaches the handler.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The microphone and the camera are the two sources a server never sees directly: what a person
    /// records is turned into a blob by the frontend, uploaded like a file and sent as a file object.
    /// The server side of that is the upload endpoint, which other tests cover; what this covers is
    /// the whole path — a real <c>MediaRecorder</c> in a real browser, the upload, the handler.
    /// </para>
    /// <para>
    /// The browser is started with Chromium's fake capture devices, which is what makes this
    /// runnable on a machine with no camera and no microphone: the device produces a test pattern
    /// and a tone, and everything after it is the real path. The context is given the permissions
    /// the page would otherwise have to ask for, because a headless browser has nobody to ask.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_recording_made_in_the_browser_reaches_the_handler()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                Audio microphone;
                Textbox reported;

                using (blocks.BeginRender())
                {
                    microphone = blocks.Add(new Audio { Label = "Mic", Sources = ["microphone"] });
                    reported = blocks.Add(new Textbox { Label = "Got" });
                }

                // What the handler says about the recording: that it arrived, and that it is a file
                // with bytes in it. The format is the browser's to choose, so it is reported rather
                // than asserted here.
                microphone.Change(
                    path => $"{Path.GetExtension(path)} {new FileInfo(path).Length > 0}",
                    [microphone],
                    [reported]);
            });

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so nothing can be recorded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(
            driver,
            [
                // A camera and a microphone that always exist, and a page that is never asked for
                // permission — the two switches that make capture testable without hardware.
                "--use-fake-device-for-media-stream",
                "--use-fake-ui-for-media-stream",
            ]);

        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed.");

        await using var running = browser!;

        await using var context = await running.NewContextAsync(new BrowserNewContextOptions
        {
            Permissions = ["microphone", "camera"],
        });

        var page = await context.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        // The recorder's two buttons by what they are rather than by what they say: their text is
        // translated, and a test that looked for a word would pass in one language and fail in the
        // next. The classes are the component's own, and stable across a build.
        var record = page.Locator("button.record-button").First;

        await record.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached });
        await record.ClickAsync(new LocatorClickOptions { Force = true });

        // Long enough for the recorder to produce a blob with something in it: the fake device
        // emits a tone, and a recorder stopped within a frame produces an empty file.
        await Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);

        await page.Locator("button.stop-button").First.ClickAsync();

        var got = page.GetByLabel("Got");

        var captured = await Eventually.IsTrueAsync(
            async () => (await got.InputValueAsync()).EndsWith(" True", StringComparison.Ordinal),
            TimeSpan.FromSeconds(30),
            TestContext.Current.CancellationToken);

        Assert.True(captured, $"the handler was given the recording: '{await got.InputValueAsync()}'");
        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// A chart is drawn in the browser, and a gesture on the drawing runs its handler.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The manifest page proves what a chart says to the frontend; this asks the frontend what it
    /// makes of it. Three things have to happen for the assertion to hold: the value has to reach
    /// the component, the charting library the bundle carries has to draw it, and the drawing has to
    /// be something a person can act on — the double click the component declares, dispatched by the
    /// frontend and answered by a handler through the queue.
    /// </para>
    /// <para>
    /// The chart is waited for by its marks rather than by a container class name: the marks are
    /// what the charting library produces once it has a table to draw, so their appearance is the
    /// evidence that all three of those happened.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_chart_is_drawn_in_the_browser_and_a_double_click_runs_its_handler()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            PlotExample.Build);

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        var marks = page.Locator("svg.marks").First;
        await marks.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        // The gesture the component declares. It is dispatched on the drawing and bubbles to the
        // element the component listens on, which is the same path a person's double click takes.
        await marks.DblClickAsync();

        await page.WaitForFunctionAsync(
            $"() => [...document.querySelectorAll('textarea, input')][0]?.value === '{PlotExample.Answer}'");

        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// A timer really ticks, at the interval the application set.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The interval belongs to the frontend: the config says how long it is and whether the timer
    /// is active, and the vendored Timer component starts a browser interval that dispatches
    /// <c>tick</c> each time. Nothing on the server wakes up on its own, so a document comparison
    /// can only show that the two numbers are right — that ticks arrive, and keep arriving, is a
    /// browser's question.
    /// </para>
    /// <para>
    /// The handler counts the ticks on the server and the test waits for more than one, which is
    /// what "at an interval" means rather than "fired once". The field it writes is the rest of
    /// the path: the tick was dispatched by the browser, queued, run, and its answer written back.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_timer_ticks_at_the_interval_the_application_set()
    {
        var ticks = 0;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    var timer = blocks.Add(new Timer { Value = 0.2 });
                    var output = blocks.Add(new Textbox { Label = "Ticks" });

                    timer.Tick(
                        () =>
                        {
                            Interlocked.Increment(ref ticks);

                            return "ticked";
                        },
                        [output]);
                }
            });

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        var field = page.GetByLabel("Ticks");
        await field.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        var ticked = await Eventually.IsTrueAsync(
            () => Task.FromResult(Volatile.Read(ref ticks) >= 2),
            TimeSpan.FromSeconds(15),
            TestContext.Current.CancellationToken);

        Assert.True(ticked, $"the timer ticked {Volatile.Read(ref ticks)} time(s) in fifteen seconds");

        await page.WaitForFunctionAsync(
            "() => [...document.querySelectorAll('textarea, input')][0]?.value === 'ticked'");

        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// A stylesheet the application names paints the page, which is what "takes effect" means.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The config carrying the CSS is only the first half: it has to reach the page, be injected by
    /// the frontend, and win over the theme's own rules — and each of those is a place where a
    /// stylesheet can be carried and never applied. The file is read by the application rather than
    /// written into <c>Css</c>, so the whole path is the one an application takes.
    /// </para>
    /// <para>
    /// What is asserted is the browser's computed colour, not the text of a style element: a rule
    /// that was injected and then overridden would pass a text comparison and still not paint.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task Css_from_a_file_the_application_names_paints_the_page()
    {
        using var folder = new TempFolder();
        var sheet = folder.Write("own.css", "body { background-color: rgb(1, 2, 3); }");

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(new Textbox { Label = "Anything" });
                }

                blocks.CssPaths = [sheet];
            });

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        Assert.True(
            await Eventually.IsTrueAsync(
                async () => await page.EvaluateAsync<string>(
                    "() => getComputedStyle(document.body).backgroundColor") == "rgb(1, 2, 3)",
                TimeSpan.FromSeconds(20),
                TestContext.Current.CancellationToken),
            $"the page's background is {await page.EvaluateAsync<string>(
                "() => getComputedStyle(document.body).backgroundColor")}");

        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// A timer stops when a handler says so, which is the other half of a clock that ticks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Python version offers one way to stop a timer: answer with it, and the props the
    /// frontend merges carry <c>active: false</c>, which is what clears the browser's interval. The
    /// document shows the binding and the two numbers; whether the interval really stops is a
    /// browser's question, because the interval is the browser's.
    /// </para>
    /// <para>
    /// The count is read after the click, waited on, and read again: a timer that kept ticking
    /// would show a larger number, so the assertion is about ticks that <em>stop arriving</em>
    /// rather than about one that arrived.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_timer_stops_when_a_handler_answers_with_it_switched_off()
    {
        var ticks = 0;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    var timer = blocks.Add(new Timer { Value = 0.2 });
                    var field = blocks.Add(new Textbox { Label = "Ticks" });
                    var stop = blocks.Add(new Button { Value = "Stop" });

                    timer.Tick(
                        () =>
                        {
                            var count = Interlocked.Increment(ref ticks);

                            return count.ToString(CultureInfo.InvariantCulture);
                        },
                        [field]);

                    // Answering with the timer is how the Python version stops one: the frontend merges the
                    // props of the component a handler returned, and `active` is one of them.
                    stop.Click(
                        () => new Timer { Value = 0.2, Active = false },
                        [timer]);
                }
            });

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        Assert.True(
            await Eventually.IsTrueAsync(
                () => Task.FromResult(Volatile.Read(ref ticks) >= 2),
                TimeSpan.FromSeconds(15),
                TestContext.Current.CancellationToken),
            $"the timer ticked {Volatile.Read(ref ticks)} time(s) in fifteen seconds");

        await page.GetByRole(AriaRole.Button, new() { NameString = "Stop" }).ClickAsync();

        // Long enough for several more intervals: a clock still running at a fifth of a second
        // would show it, and the run that stops it has to have been answered first.
        await Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);

        var stopped = Volatile.Read(ref ticks);

        await Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);

        Assert.Equal(stopped, Volatile.Read(ref ticks));
        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// A reader changes the language in the settings panel, and the page's text changes with it —
    /// without the page being reloaded.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the half of translating that only a browser can observe. The server's part is the
    /// catalogue and the markers, which it serves as they are; whether a marker
    /// actually reads as text, and whether changing the language changes it, is decided by the
    /// frontend at run time — and a build that served the right dictionary but a marker the
    /// frontend did not recognise would pass every document comparison and still show
    /// <c>__i18n__{…}</c> to a reader.
    /// </para>
    /// <para>
    /// The language is changed twice, through the settings panel's own language field, so what is
    /// asserted is switching rather than a page that happened to load in one language. The field's
    /// label is the application's, from its own catalogue: the frontend's chrome is translated from
    /// the messages it ships with, which is not this framework's to prove. A value written into the
    /// page's JavaScript context before the switch is read back after it: a reload rebuilds that
    /// context, so the surviving value is what tells switching from reloading without depending on
    /// which language a fresh page would happen to start in — the frontend keeps the choice in
    /// memory only, so a reload starts again from the browser's own language
    /// (<see cref="A_refresh_serves_the_page_in_the_language_the_browser_asks_for"/>).
    /// </para>
    /// </remarks>
    [Fact]
    public async Task Changing_the_language_in_the_settings_translates_the_page_without_a_reload()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                var i18n = new I18n(
                    ("en", new Dictionary<string, string> { ["name_label"] = "Your Name" }),
                    ("es", new Dictionary<string, string> { ["name_label"] = "Tu Nombre" }),
                    ("de", new Dictionary<string, string> { ["name_label"] = "Dein Name" }));

                blocks.I18n = i18n;

                using (blocks.BeginRender())
                {
                    blocks.Add(new Textbox { Label = i18n["name_label"] });
                }
            });

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        // The field is named by the marker the server wrote, translated by the frontend: seeing
        // "Your Name" rather than the marker is the first half of the claim.
        await page.GetByLabel("Your Name").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        // A reload would replace the page's JavaScript context, so a value written into it here is
        // the direct evidence that the two switches happen in place. The labels alone would not do:
        // the frontend remembers the reader's language, so a page that reloaded into Spanish would
        // show the new label and drop the old one exactly as a switch does.
        await page.EvaluateAsync("() => { window.__i18nSwitchedInPlace = 'kept'; }");

        await ChooseLanguageAsync(page, "Esp", "Tu Nombre");

        // The first language is gone rather than merely joined by the second.
        Assert.Empty(await page.GetByLabel("Your Name").AllAsync());

        await ChooseLanguageAsync(page, "Deu", "Dein Name");

        Assert.Empty(await page.GetByLabel("Tu Nombre").AllAsync());
        Assert.Equal("kept", await page.EvaluateAsync<string>("() => window.__i18nSwitchedInPlace"));
        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// A refresh serves the page again in the language the browser asks for, which is not
    /// necessarily the one the reader picked in the settings panel.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is what a refresh actually does with the translations, and it is the Python version's
    /// behaviour rather than a choice of this framework: the settings panel's language field calls
    /// the frontend's <c>changeLocale</c>, which sets an in-memory store and writes nothing
    /// anywhere — no <c>localStorage</c>, no <c>sessionStorage</c>, no cookie — so the next page
    /// load starts from <c>navigator.language</c> again. The vendored assets are the Python
    /// version's, byte for byte, so a framework that remembered the choice would have to inject
    /// script into a page it does not own and would be a behaviour no the Python version release
    /// has.
    /// </para>
    /// <para>
    /// What a refresh does preserve is the catalogue: the page is translated again, in the language
    /// the reader's own browser asks for. A reader whose browser is Spanish therefore keeps
    /// Spanish across a refresh, which is the part of "the language is still the new one" that
    /// holds. A second browser, in a language the application has no catalogue for, is the control:
    /// it shows the English fallback, so a page that always came back in Spanish — the reading
    /// this test has to rule out — would fail here rather than pass on the first page alone.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_refresh_serves_the_page_in_the_language_the_browser_asks_for()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                var i18n = new I18n(
                    ("en", new Dictionary<string, string> { ["name_label"] = "Your Name" }),
                    ("es", new Dictionary<string, string> { ["name_label"] = "Tu Nombre" }));

                blocks.I18n = i18n;

                using (blocks.BeginRender())
                {
                    blocks.Add(new Textbox { Label = i18n["name_label"] });
                }
            });

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;

        // A reader whose browser is Spanish, which is what a fresh load translates from: the
        // locale is emulated per context rather than taken from whatever this machine happens to
        // run, so the assertion does not depend on the machine's own language.
        await using var context = await running.NewContextAsync(
            new BrowserNewContextOptions { Locale = "es-ES" });

        var page = await context.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        // The catalogue reached the browser and was applied at boot, before any switch.
        await page.GetByLabel("Tu Nombre").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await ChooseLanguageAsync(page, "Eng", "Your Name");

        Assert.Empty(await page.GetByLabel("Tu Nombre").AllAsync());

        await page.ReloadAsync();

        // The choice in the panel was the page's, not the browser's, so the refresh reads the
        // browser again. Pinned rather than left unasserted: if a future release stores the
        // choice, this is the line that says so.
        await page.GetByLabel("Tu Nombre").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        Assert.Empty(await page.GetByLabel("Your Name").AllAsync());
        Assert.Empty(failures.Entries);

        // A browser the application has no catalogue for: the page still boots, and the marker
        // reads as the English fallback rather than as a marker or as Spanish.
        await using var french = await running.NewContextAsync(
            new BrowserNewContextOptions { Locale = "fr-FR" });

        var frenchPage = await french.NewPageAsync();
        frenchPage.SetDefaultTimeout(20_000);

        var frenchFailures = PageFailures.Watch(frenchPage);

        await frenchPage.GotoAsync(host.BaseAddress.ToString());

        await frenchPage.GetByLabel("Your Name").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        Assert.Empty(await frenchPage.GetByLabel("Tu Nombre").AllAsync());
        Assert.Empty(frenchFailures.Entries);
    }

    /// <summary>
    /// A reader signs in through the page's own form, uses the application, and signs out again.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The form is the vendored frontend's, drawn from the config an unauthenticated request is
    /// served; it posts the pair to <c>/login</c> and reloads the page when the answer is a success.
    /// So the whole of this test is a browser doing what a person does, which is the only way to see
    /// that the two halves agree: the document that asks for a sign-in and the route that accepts
    /// one.
    /// </para>
    /// <para>
    /// The button's answer names the reader, which is the other half of the ticket: the run carries
    /// who asked, and the handler reads it. A page that signed in but whose runs were anonymous
    /// would answer "nobody greets Ada" here.
    /// </para>
    /// <para>
    /// Signing out is a navigation to <c>/logout</c>, which is how the Python version offers it —
    /// the page has no button for an application with its own accounts — and what it ends is the
    /// sign-in itself: the form comes back because the application no longer knows who is asking.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task Signing_in_through_the_form_opens_the_application_and_signing_out_closes_it()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                blocks.Auth = GradioAuth.OneAccount("alice", "s3cret");
                blocks.AuthMessage = "Sign in to see the demo";

                Textbox name;
                Textbox answer;
                Button greet;

                using (blocks.BeginRender())
                {
                    name = blocks.Add(new Textbox { Label = "Name" });
                    answer = blocks.Add(new Textbox { Label = "Answer" });
                    greet = blocks.Add(new Button { Value = "Greet" });
                }

                greet.Click(
                    (Func<string, Request, string>)((value, request) =>
                        $"{request.UserName ?? "nobody"} greets {value}"),
                    [name],
                    [answer],
                    apiName: "greet");
            });

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed, so the "
            + "page cannot be loaded here.");

        await using var running = browser!;

        // The form's own labels come from the frontend's catalogue for the reader's language, so the
        // browser is told which language to be in rather than left to whatever this machine runs.
        await using var context = await running.NewContextAsync(
            new BrowserNewContextOptions { Locale = "en-US" });

        var page = await context.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        // The application's field, matched whole: the form's own fields are named by the frontend
        // and one of them holds the word "name".
        var nameField = page.GetByLabel("Name", new() { Exact = true });

        await page.GotoAsync(host.BaseAddress.ToString());

        // The form, and nothing of the application behind it.
        await page.GetByText("Sign in to see the demo").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        Assert.Empty(await nameField.AllAsync());

        await page.GetByLabel("username", new() { Exact = true }).FillAsync("alice");
        await page.GetByLabel("password", new() { Exact = true }).FillAsync("s3cret");
        await page.GetByRole(AriaRole.Button, new() { NameString = "Login" }).ClickAsync();

        // The form reloads the page into the application, which is the sign-in having worked.
        await nameField.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        Assert.Empty(await page.GetByText("Sign in to see the demo").AllAsync());

        await nameField.FillAsync("Ada");
        await page.GetByRole(AriaRole.Button, new() { NameString = "Greet" }).ClickAsync();

        var answer = page.GetByLabel("Answer");

        await Eventually.IsTrueAsync(
            async () => await answer.InputValueAsync() == "alice greets Ada",
            TimeSpan.FromSeconds(20),
            TestContext.Current.CancellationToken);

        await page.GotoAsync(new Uri(host.BaseAddress, "logout").ToString());

        await page.GetByText("Sign in to see the demo").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        Assert.Empty(await nameField.AllAsync());
        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// A component that brings its own frontend is asked for its bundle at the route that serves it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The one thing about this extension point that no document comparison can establish: the
    /// vendored frontend, which has never heard of the component, reads the identity out of the
    /// config and asks <c>custom_component/&lt;id&gt;/client/&lt;variant&gt;/index.js</c> for it —
    /// the exact shape of the route. A route that served the same bytes at a different address
    /// would pass every other test in this project and leave a real browser unable to draw the
    /// component.
    /// </para>
    /// <para>
    /// What the bundle contains is deliberately not a component: building one needs a Svelte
    /// toolchain, which this project does not carry. So the assertion is about the
    /// request and its answer, not about a drawing — the files are fetched, and whether they can be
    /// drawn is the bundle author's business.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_custom_component_is_asked_for_its_bundle_at_the_route_that_serves_it()
    {
        using var folder = new TempFolder();
        var templates = Sparkline.WriteBundle(folder);

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(new Sparkline(templates));
                }
            });

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var asked = new ConcurrentBag<string>();
        page.Response += (_, answer) =>
        {
            if (answer.Url.Contains("/custom_component/", StringComparison.Ordinal))
            {
                asked.Add($"{(int)answer.Status} {new Uri(answer.Url).AbsolutePath}");
            }
        };

        await page.GotoAsync(host.BaseAddress.ToString());

        // Three files, because the loader asks for all three before it has a component: the module,
        // its stylesheet and the Svelte runtime it was built against.
        var loaded = await Eventually.IsTrueAsync(
            () => Task.FromResult(asked.Count >= 3),
            TimeSpan.FromSeconds(20),
            TestContext.Current.CancellationToken);

        Assert.True(loaded, $"the page asked for: [{string.Join(", ", asked)}]");

        var prefix = $"/gradio_api/custom_component/{Sparkline.Identity}/client/component/";

        Assert.Contains($"200 {prefix}index.js", asked);
        Assert.Contains($"200 {prefix}style.css", asked);
        Assert.Contains($"200 {prefix}svelte_runtime_entry.js", asked);
    }

    /// <summary>
    /// A page that names an icon installs, because the manifest and every icon it names arrive.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Installability is a claim about a browser, not about a document: the browser reads the
    /// manifest the page links to, resolves each icon's <c>src</c> against the manifest's own
    /// address, fetches it, and decodes it. A manifest whose addresses were relative where they had
    /// to be absolute, or a sized icon route that answered a document instead of an image, would
    /// pass every comparison this project makes and leave a browser offering nothing.
    /// </para>
    /// <para>
    /// So the assertions are the browser's own: the two fetches, the statuses, and the decoded size
    /// of each icon — which is what proves the scaling happened in a client rather than in a test
    /// that trusts the header it was sent.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_page_that_names_an_icon_installs_because_the_manifest_and_its_icons_arrive()
    {
        using var folder = new TempFolder();
        var icon = folder.Write("icon.png", TestImages.Png(64, 64, red: 20, green: 40, blue: 60));

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                blocks.Title = "Installable";
                blocks.FaviconPath = icon;
                blocks.Pwa = true;

                using (blocks.BeginRender())
                {
                    blocks.Add(new Textbox { Label = "Name" });
                }
            });

        var playwright = await CreateAsync();
        Assert.SkipUnless(
            playwright is not null,
            "no browser driver could be started here, so the page cannot be loaded.");

        using var driver = playwright!;

        var browser = await LaunchAsync(driver);
        Assert.SkipUnless(
            browser is not null,
            $"no browser to drive: neither '{string.Join("' nor '", _channels)}' is installed.");

        await using var running = browser!;
        var page = await running.NewPageAsync();
        page.SetDefaultTimeout(20_000);

        var failures = PageFailures.Watch(page);

        await page.GotoAsync(host.BaseAddress.ToString());

        var report = JsonNode.Parse(await page.EvaluateAsync<string>(ReadManifestScript))!.AsObject();

        Assert.True(report["found"]!.GetValue<bool>(), "the page links no manifest");
        Assert.Equal(200, report["manifestStatus"]!.GetValue<int>());
        Assert.Equal("Installable", report["name"]!.GetValue<string>());
        Assert.Equal("./", report["start_url"]!.GetValue<string>());
        Assert.Equal("standalone", report["display"]!.GetValue<string>());

        var icons = report["icons"]!.AsArray();

        Assert.Equal(2, icons.Count);

        // The two addresses a launcher asks for, each answered as a PNG of exactly that size.
        var expected = new[] { ("/pwa_icon/192", 192), ("/pwa_icon/512", 512) };

        for (var index = 0; index < expected.Length; index++)
        {
            var (path, size) = expected[index];
            var entry = icons[index]!.AsObject();

            Assert.Equal(path, entry["src"]!.GetValue<string>());
            Assert.Equal(200, entry["status"]!.GetValue<int>());
            Assert.Equal("image/png", entry["type"]!.GetValue<string>());
            Assert.Equal(size, entry["width"]!.GetValue<int>());
            Assert.Equal(size, entry["height"]!.GetValue<int>());
        }

        // Nothing the page or the manifest asked for was refused.
        Assert.Empty(failures.Entries);
    }

    /// <summary>
    /// Reads the manifest the page links to, and every icon it names, the way a browser does.
    /// </summary>
    /// <remarks>
    /// Run inside the page rather than from the test, because the point is what a client on this
    /// origin can reach: a relative <c>src</c> resolves against the manifest's address, and the
    /// image is decoded by the browser's own decoder rather than by this project's.
    /// </remarks>
    private const string ReadManifestScript =
        """
        async () => {
          const link = document.querySelector('link[rel="manifest"]');
          if (!link) { return JSON.stringify({ found: false }); }

          const manifestResponse = await fetch(link.href);
          const manifest = await manifestResponse.json();

          const icons = [];
          for (const icon of manifest.icons) {
            const url = new URL(icon.src, manifestResponse.url).href;
            const response = await fetch(url);
            const blob = await response.blob();

            let width = -1;
            let height = -1;
            try {
              const bitmap = await createImageBitmap(blob);
              width = bitmap.width;
              height = bitmap.height;
              bitmap.close();
            } catch (error) {
              width = -1;
            }

            icons.push({
              src: icon.src,
              url,
              status: response.status,
              type: response.headers.get('content-type'),
              width,
              height
            });
          }

          return JSON.stringify({
            found: true,
            manifestStatus: manifestResponse.status,
            name: manifest.name,
            start_url: manifest.start_url,
            display: manifest.display,
            icons
          });
        }
        """;

    /// <summary>Picks a language in the settings panel, and waits for the page to be in it.</summary>
    /// <param name="page">The page.</param>
    /// <param name="typed">What to type into the language field, enough to name one language.</param>
    /// <param name="label">The label the application's field should have afterwards.</param>
    /// <returns>A task that completes when the page shows <paramref name="label"/>.</returns>
    /// <remarks>
    /// The panel is opened from the footer's settings button, which is found by its class rather
    /// than by its text: the text is part of what changes with the language.
    /// </remarks>
    private static async Task ChooseLanguageAsync(IPage page, string typed, string label)
    {
        await page.Locator("footer button.settings").ClickAsync();

        var language = page.Locator("input[aria-label=\"Language\"]");
        await language.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await language.ClickAsync();
        await language.FillAsync(string.Empty);
        await language.PressSequentiallyAsync(typed);
        await page.Keyboard.PressAsync("ArrowDown");
        await page.Keyboard.PressAsync("Enter");

        await page.GetByLabel(label).WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        // The panel stays open across a change, so it is closed the way a reader closes it.
        await page.Locator(".backdrop").ClickAsync();
    }

    /// <summary>Starts the browser driver, or returns <see langword="null"/> when it cannot start here.</summary>
    private static async Task<IPlaywright?> CreateAsync()
    {
        try
        {
            return await Playwright.CreateAsync();
        }
        catch (PlaywrightException)
        {
            // The driver is a node process shipped in the package; an environment that cannot run
            // it cannot drive a browser either.
            return null;
        }
    }

    /// <summary>Launches an installed browser, or returns <see langword="null"/> when there is none.</summary>
    /// <summary>Starts a browser, with arguments a test needs on top of the usual ones.</summary>
    /// <param name="playwright">The driver.</param>
    /// <param name="extraArgs">Arguments for a test that needs the browser to behave a certain way.</param>
    /// <returns>The browser, or <see langword="null"/> when no installed channel would start.</returns>
    private static async Task<IBrowser?> LaunchAsync(IPlaywright playwright, string[]? extraArgs = null)
    {
        foreach (var channel in _channels)
        {
            try
            {
                return await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Channel = channel,
                    Headless = true,

                    // Loopback only, so a proxy the machine happens to have configured must not be
                    // put in the middle: a proxied request is sent in the absolute form, which the
                    // SSR runtime refuses, and the test would then depend on the environment.
                    Args = ["--no-proxy-server", .. extraArgs ?? []],
                });
            }
            catch (PlaywrightException)
            {
                // That channel is not installed; the next one may be.
            }
        }

        return null;
    }
}
