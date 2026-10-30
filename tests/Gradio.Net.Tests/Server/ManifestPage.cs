using System.Text.Json.Nodes;
using Gradio.Net;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The manifest page the config and description tests are about.
/// </summary>
/// <remarks>
/// <para>
/// A manifest page is a page holding several components, so that one request returns the whole of
/// them and a single comparison covers every parameter of every one.
/// </para>
/// <para>
/// It is written out here as a page an application could write, so that what the tests assert on
/// is the tree the server produces for an application rather than a document assembled for them.
/// </para>
/// </remarks>
internal static class ManifestPage
{
    /// <summary>The custom HTML tags the second chat keeps, which is a value the default is not.</summary>
    private static readonly IReadOnlyList<string> KeptTags = ["thinking"];

    /// <summary>The two values the multiple dropdown starts with.</summary>
    private static readonly IReadOnlyList<object?> MultiselectValue = ["medium"];

    /// <summary>The option the coloured checkbox group starts with.</summary>
    private static readonly IReadOnlyList<string> ColoredValue = ["b"];

    /// <summary>The title the page and the OpenAPI document carry.</summary>
    internal const string Title = "Manifest page";

    /// <summary>Builds the page into an application.</summary>
    /// <param name="blocks">The application to render into.</param>
    internal static void Build(Blocks blocks)
    {
        blocks.Title = Title;

        // The case fixes this switch on both sides, so the field is compared rather than
        // neutralised.
        blocks.AnalyticsEnabled = true;

        Textbox name;
        Textbox notes;
        Number age;

        using (blocks.BeginRender())
        {
            name = blocks.Add(new Textbox { Label = "Name", Placeholder = "Your name" });
            notes = blocks.Add(new Textbox { Label = "Notes", Lines = 4, Placeholder = "Anything else" });
            age = blocks.Add(new Number { Label = "Age", Value = 0 });
            blocks.Add(new Number
            {
                Label = "Score",
                Value = 1.5,
                Precision = 2,
                Minimum = 0,
                Maximum = 10,
            });

            // The media components, with a parameter the default would not produce on every one
            // that has a choice to make. None of them is given a value: a value is copied into the
            // cache, and the config would then carry an absolute path.
            blocks.Add(new File
            {
                Label = "Attachment",
                FileTypes = [".txt", ".csv"],
                Height = 120,
            });
            blocks.Add(new File
            {
                Label = "Bundle",
                FileCount = "multiple",
                Type = "binary",
                AllowReordering = true,
            });
            blocks.Add(new Image
            {
                Label = "Photo",
                Type = "filepath",
                Format = "png",
                Sources = ["upload", "clipboard"],
                Width = 240,
                Height = 180,
                Placeholder = "Drop an image",
                AltText = "a photo",
                WebcamOptions = new WebcamOptions
                {
                    Mirror = false,
                    Constraints = new JsonObject { ["width"] = 640 },
                },
            });

            // The rest of the media family, with a parameter the default would not produce on
            // every one that has a choice to make. As above, none of them is given a file as a
            // value: a file is copied into the cache and the config would carry an absolute path.
            // The one value given is the audio gallery's, and it is an address rather than a file,
            // so nothing is copied. The one path that has to be there is the file explorer's root,
            // and the case normalizes it, because the directory a recording ran in belongs to the
            // run rather than to the application.
            blocks.Add(new ImageEditor
            {
                Height = 240,
                Width = 320,
                ImageMode = "RGB",
                Sources = ["upload"],
                Type = "filepath",
                Label = "Editor",
                Buttons = ["download"],
                Scale = 2,
                MinWidth = 200,
                Placeholder = "Draw here",
                Transforms = ["crop"],
                Eraser = new EraserOptions { DefaultSize = 12 },
                Brush = new BrushOptions
                {
                    DefaultSize = 8,
                    Colors = ["#111111", "#222222"],
                    DefaultColor = "#222222",
                    ColorMode = "fixed",
                },
                Format = "png",
                Layers = new LayerOptions
                {
                    AllowAdditionalLayers = false,
                    Layers = ["Only"],
                    Disabled = true,
                },
                CanvasSize = [640, 480],
                FixedCanvas = true,
                WebcamOptions = new WebcamOptions
                {
                    Mirror = false,
                    Constraints = new JsonObject { ["width"] = 640 },
                },
            });
            blocks.Add(new ImageSlider
            {
                Format = "png",
                Height = 200,
                Width = 300,
                ImageMode = "RGBA",
                Type = "filepath",
                Label = "Slider",
                Buttons = ["fullscreen"],
                Scale = 2,
                MinWidth = 220,
                SliderPosition = 25,
                MaxHeight = 320,
            });
            blocks.Add(new Video
            {
                Format = "mp4",
                Sources = ["webcam"],
                Height = 240,
                Width = 320,
                Label = "Movie",
                Scale = 2,
                MinWidth = 200,
                WebcamOptions = new WebcamOptions { Mirror = false },
                IncludeAudio = false,
                Autoplay = true,
                Buttons = ["share"],
                Loop = true,
                Streaming = true,
                Watermark = new WatermarkOptions { Watermark = "mark.png", Position = "top-left" },
                PlaybackPosition = 2.5,
            });
            blocks.Add(new Audio
            {
                Sources = ["upload"],
                Type = "filepath",
                Label = "Sound",
                Scale = 2,
                MinWidth = 200,
                Format = "mp3",
                Autoplay = true,
                Editable = false,
                Buttons = ["download"],
                WaveformOptions = new WaveformOptions
                {
                    WaveformColor = "#111111",
                    WaveformProgressColor = "#222222",
                    TrimRegionColor = "#333333",
                    ShowRecordingWaveform = false,
                    SkipLength = 10,
                    SampleRate = 48000,
                },
                Loop = true,
                Recording = true,
                Subtitles = new Subtitle[] { new() { Start = 0, End = 1, Text = "hello" } },
                PlaybackPosition = 1.5,
            });
            blocks.Add(new AudioGallery
            {
                AudioUrls = ["https://example.com/a.wav", "https://example.com/b.wav"],
                Value = "https://example.com/b.wav",
                Labels = ["One", "Two"],
                Columns = 2,
                Label = "Sounds",
            });
            blocks.Add(new Gallery
            {
                Format = "png",
                FileTypes = ["image"],
                Label = "Pictures",
                Container = false,
                Scale = 2,
                MinWidth = 200,
                Columns = 3,
                Rows = 2,
                Height = 300,
                AllowPreview = false,
                Preview = true,
                SelectedIndex = 1,
                ObjectFit = "cover",
                Buttons = ["download"],
                Interactive = false,
                Type = "filepath",
                FitColumns = false,
                Sources = ["upload", "clipboard"],
            });
            blocks.Add(new FileExplorer
            {
                Glob = "*.txt",
                FileCount = "single",
                RootDir = ".",
                IgnoreGlob = "*.py",
                Label = "Files",
                ShowLabel = false,
                Scale = 2,
                MinWidth = 200,
                Height = 200,
                MaxHeight = 300,
                MinHeight = 100,
            });
            blocks.Add(new Model3D
            {
                DisplayMode = "wireframe",
                ClearColor = [0.1, 0.2, 0.3, 0.4],
                CameraPosition = [1, 2, 3],
                ZoomSpeed = 2,
                PanSpeed = 3,
                Height = 240,
                Label = "Model",
                ElemId = "the-model",
                Scale = 2,
                MinWidth = 200,
            });

            // The chat, twice: once with nothing but a label, so that the defaults are compared,
            // and once with every parameter that has a value to choose set to one the default would
            // not produce. The conversation it starts with is text only — a file block would name a
            // path on this machine, which is a path no other machine has.
            blocks.Add(new Chatbot { Label = "Chat" });
            blocks.Add(new Chatbot
            {
                Value = [new ChatMessage("Hi", ChatRole.User), new ChatMessage("Hello")],
                Label = "Conversation",
                Height = 300,
                Resizable = true,
                MaxHeight = 500,
                MinHeight = 200,
                Editable = "all",
                LatexDelimiters = [new LatexDelimiter("$", "$", display: false)],
                Rtl = true,
                Buttons = ["copy_all"],
                Watermark = "generated",
                AvatarImages = new AvatarImages("https://example.com/user.png", null),
                SanitizeHtml = false,
                RenderMarkdown = false,
                FeedbackOptions = ["Good", "Bad"],
                FeedbackValue = ["Good", null],
                LineBreaks = false,
                Layout = "panel",
                Placeholder = "Ask something",
                Examples = [new ChatExample { Text = "Hello", DisplayText = "Say hello" }],
                AllowFileDownloads = false,
                GroupConsecutiveMessages = false,
                AllowTags = KeptTags,
                ReasoningTags = [new ReasoningTag("<think>", "</think>")],
                LikeUserMessage = true,
                Scale = 2,
                MinWidth = 240,
                Autoscroll = false,
            });

            // The display family: Markdown with its defaults, and a second Markdown with every
            // parameter that has a value to choose set to one the default would not produce. The
            // math, the code highlighting and the inline HTML are what the frontend does, so the
            // value is plain text — a file would be copied into the cache and the config would
            // carry an absolute path.
            blocks.Add(new Markdown { Label = "Notes" });
            blocks.Add(new Markdown
            {
                Value = "## Heading\n\nSome **bold** text and $x^2$.",
                Label = "Rendered",
                Rtl = true,
                LatexDelimiters = [new LatexDelimiter("$", "$", display: false)],
                SanitizeHtml = false,
                LineBreaks = true,
                HeaderLinks = true,
                Height = 200,
                MaxHeight = 400,
                MinHeight = 100,
                Buttons = ["copy"],
                Padding = true,
                Scale = 2,
                MinWidth = 200,
            });

            // A dialogue with its defaults, and a second one with parameters the default would not
            // produce. The value is a string because the formatter decides its shape; nothing
            // here interprets it. The second one is interactive, because the Python version clears the note
            // of a dialogue the user cannot edit — the default — so a note is only a prop when the
            // dialogue is one.
            blocks.Add(new Dialogue { Label = "Transcript" });
            blocks.Add(new Dialogue
            {
                Value = "speaker: Hello!",
                Type = "list",
                Speakers = ["user", "assistant"],
                Tags = ["greeting"],
                Separator = " | ",
                ColorMap = new Dictionary<string, string> { ["greeting"] = "#00ff00" },
                Label = "Conversation",
                Info = "A transcript",
                Placeholder = "Say something",
                Scale = 2,
                MinWidth = 200,
                Interactive = true,
                Autoscroll = false,
                MaxLines = 10,
                Buttons = ["copy"],
                SubmitBtn = "Send",
                UiMode = "dialogue",
            });

            AddInputs(blocks);
            AddButtons(blocks, name, notes, age);
            AddDataFamily(blocks);
            AddPlots(blocks);
            AddLayouts(blocks);
            AddCustomHtml(blocks);
            AddUtilityComponents(blocks);
        }
    }

    /// <summary>
    /// The text and number family, once each and twice where the defaults are the point.
    /// </summary>
    /// <param name="blocks">The application being rendered.</param>
    /// <remarks>
    /// <para>
    /// Every component of the family appears, with the parameters that make it more than a text
    /// field: a slider whose step and precision are set, a group that reports indices and offers a
    /// select-all box, a dropdown that takes several values and values of its own, a multimodal box
    /// with the submit and stop buttons a streaming handler needs, a date field with no time and a
    /// time zone, and a code editor whose lines wrap and whose line numbers are hidden.
    /// </para>
    /// <para>
    /// The slider, the checkbox group and the dropdown appear a second time with nothing set at all,
    /// because what those three compute when nobody says anything — the step, the empty list, the
    /// absence of a value — is as much a part of the parameter mapping as anything an application
    /// asks for.
    /// </para>
    /// <para>
    /// The code editor and the coloured checkbox group come last because neither is a form
    /// component: a block that is not one ends the run of fields that get grouped, so putting them
    /// earlier would split the form in two and change the layout tree the case compares.
    /// </para>
    /// </remarks>
    private static void AddInputs(Blocks blocks)
    {
        blocks.Add(new SimpleTextbox
        {
            Label = "Plain",
            Placeholder = "Say something",
            Rtl = true,
            MinWidth = 120,
        });
        blocks.Add(new Slider
        {
            Minimum = -5,
            Maximum = 5.5,
            Value = 1.25,
            Step = 0.25,
            Precision = 2,
            Label = "Level",
            Info = "Pick a level",
            Buttons = ["reset"],
        });
        blocks.Add(new Slider());
        blocks.Add(new Checkbox { Value = true, Label = "Agree", Info = "Tick to agree" });
        blocks.Add(new CheckboxGroup
        {
            Choices = [new Choice("a"), new Choice("b"), new Choice(2, "Two")],
            Value = "b",
            ShowSelectAll = true,
            Label = "Letters",
        });
        blocks.Add(new CheckboxGroup());
        blocks.Add(new Radio
        {
            Choices = [new Choice("x"), new Choice(1), new Choice(2.5)],
            Value = 1,
            Type = "index",
            Rtl = true,
            Label = "One of them",
        });
        blocks.Add(new Dropdown
        {
            Choices = [new Choice("small"), new Choice("medium"), new Choice("large")],
            Value = MultiselectValue,
            Multiselect = true,
            MaxChoices = 2,
            AllowCustomValue = true,
            NumChoicesShown = 5,
            Label = "Many",
        });
        blocks.Add(new Dropdown());
        blocks.Add(new SimpleDropdown
        {
            Choices = [new Choice("first"), new Choice(2), new Choice(3.5)],
            Value = 2,
            Label = "Simple",
        });
        blocks.Add(new MultimodalTextbox
        {
            Value = new MultimodalText("hello"),
            Sources = ["upload", "microphone"],
            FileTypes = [".png"],
            FileCount = "multiple",
            Lines = 2,
            MaxLines = 4,
            SubmitBtn = "Go",
            StopBtn = true,
            TextAlign = "right",
            Rtl = true,
            MaxPlainTextLength = 500,
            Label = "Chat box",
        });
        blocks.Add(new ColorPicker { Value = "#ff0000", Label = "Tint" });
        blocks.Add(new DateTime
        {
            Value = "2020-10-01 05:20:15",
            IncludeTime = false,
            Type = "string",
            Timezone = "Europe/Paris",
            Label = "When",
            Buttons = ["reset"],
        });
        blocks.Add(new Code
        {
            Value = "  x = 1  ",
            Language = "python",
            Lines = 10,
            MaxLines = 3,
            WrapLines = true,
            ShowLineNumbers = false,
            Autocomplete = true,
            Label = "Snippet",
        });
        blocks.Add(new ColoredCheckboxGroup
        {
            Choices = ["a", "b"],
            Colors = ["#111111", "#222222"],
            Value = ColoredValue,
            Label = "Coloured",
        });
    }

    /// <summary>
    /// The button family, written as the case's Python demo writes it.
    /// </summary>
    /// <param name="blocks">The application being rendered.</param>
    /// <param name="name">The field the clear button clears first.</param>
    /// <param name="notes">The field the clear button clears second.</param>
    /// <param name="age">The field the clear button clears third.</param>
    /// <remarks>
    /// <para>
    /// Every component of the family is here except the sign-in button: the Python version refuses
    /// to start an application that holds one without a Hugging Face login, so there is nothing to
    /// compare it against.
    /// </para>
    /// <para>
    /// Three of the buttons bind their own events when they are added, so this page is the one
    /// manifest page with dependencies on it: the clear button binds the JavaScript that empties the
    /// fields, and the share button binds the two clicks that copy a link and put its label back.
    /// They are compared like any other field, which is the point — a button's behaviour is part of
    /// what the config says about it.
    /// </para>
    /// <para>
    /// Two things are deliberately left out. No icon is set on any button, because an icon is a
    /// file copied into the cache and the config would then carry an absolute path that differs
    /// between any two machines — and because the share button's default icon is one of the Python
    /// version's own, from inside its package. And the clear button names no state component: a
    /// state's props carry a callback, whose the Python version spelling is a function repr with an
    /// address in it, which no second run reproduces.
    /// </para>
    /// </remarks>
    private static void AddButtons(Blocks blocks, Textbox name, Textbox notes, Number age)
    {
        blocks.Add(new Button());
        blocks.Add(new Button
        {
            Value = "Open",
            Variant = "primary",
            Size = "sm",
            Link = "https://example.com/docs",
            LinkTarget = "_blank",
            Interactive = false,
            Visible = Visibility.Hidden,
            ElemId = "the-button",
            ElemClasses = ["wide", "tall"],
            Scale = 2,
            MinWidth = 120,
        });
        blocks.Add(new ClearButton
        {
            Components = [name, notes, age],
            Value = "Clear the fields",
            Variant = "primary",
            ApiName = "clear_fields",
        });
        blocks.Add(new DuplicateButton());
        blocks.Add(new DownloadButton { Label = "Download", Variant = "stop", Size = "sm" });
        blocks.Add(new UploadButton { Label = "Upload one", Type = "binary" });
        blocks.Add(new UploadButton
        {
            Label = "Upload many",
            FileCount = "multiple",
            FileTypes = [".txt", ".csv"],
            Variant = "primary",
        });
        blocks.Add(new DeepLinkButton { Value = "Share", CopiedValue = "Copied" });
    }

    /// <summary>
    /// The data family, once each and twice for the table, whose defaults are the point.
    /// </summary>
    /// <param name="blocks">The application being rendered.</param>
    /// <remarks>
    /// Every value here is text or a number, and the annotated image is given no value at all: a
    /// value naming a file would be copied into the cache, and the config would then carry an
    /// absolute path that differs between any two machines.
    /// </remarks>
    private static void AddDataFamily(Blocks blocks)
    {
        blocks.Add(new Dataframe
        {
            Value = new object?[] { new object?[] { "a", 1 }, new object?[] { "b", 2 } },
            Headers = ["Letter", "Number"],
            RowCount = (2, "fixed"),
            ColumnCount = 2,
            Datatype = new object?[] { "str", "number" },
            Type = "array",
            LatexDelimiters = [new LatexDelimiter("$$", "$$", display: false)],
            Label = "Table",
            MaxHeight = 320,
            Scale = 2,
            MinWidth = 200,
            Interactive = false,
            Wrap = true,
            LineBreaks = false,
            ColumnWidths = new object?[] { "80px", "120px" },
            Buttons = ["copy"],
            ShowRowNumbers = false,
            MaxChars = 40,
            ShowSearch = "search",
            PinnedColumns = 1,
            StaticColumns = [1],
        });
        blocks.Add(new Dataframe());
        // The two components the dataset holds are blocks of the page as well, because the Python version
        // builds them inside the page: what the dataset holds is a list of components, and a
        // component built in a page is one of the page's blocks.
        var word = blocks.Add(new Textbox { Label = "Word" });
        var count = blocks.Add(new Number { Label = "Count" });

        blocks.Add(new Dataset
        {
            Components = [word, count],
            Samples = new IReadOnlyList<object?>[]
            {
                new object?[] { "cat", 3 },
                new object?[] { "dog", 5 },
            },
            Headers = ["Animal", "How many"],
            Type = "index",
            Layout = "table",
            SamplesPerPage = 2,
            Scale = 2,
            MinWidth = 200,
            Label = "Examples",
            SampleLabels = ["First", "Second"],
        });
        blocks.Add(new JSON
        {
            Value = new JsonObject
            {
                ["a"] = new JsonArray(1, 2),
                ["b"] = null,
            },
            Label = "Data",
            Open = true,
            ShowIndices = false,
            Height = 200,
            MaxHeight = 300,
            MinHeight = 100,
            Scale = 2,
            MinWidth = 200,
            Buttons = [],
        });
        blocks.Add(new ParamViewer
        {
            Value = new JsonObject
            {
                ["array"] = new JsonObject
                {
                    ["type"] = "numpy",
                    ["description"] = "the array",
                    ["default"] = "None",
                },
            },
            Language = "typescript",
            Linkify = ["numpy"],
            Header = "Arguments",
            AnchorLinks = "default",
            MaxHeight = 400,
        });
        blocks.Add(new HighlightedText
        {
            Value = new HighlightedToken[]
            {
                new("Hello "),
                new("world", "greeting"),
                new("!"),
            },
            ColorMap = new Dictionary<string, string> { ["greeting"] = "#00ff00" },
            ShowLegend = false,
            ShowInlineCategory = false,
            CombineAdjacent = true,
            AdjacentSeparator = "|",
            ShowWhitespaces = false,
            Label = "Tags",
            Scale = 2,
            MinWidth = 200,
            Interactive = false,
            Rtl = true,
            Buttons = ["copy"],
        });
        blocks.Add(new AnnotatedImage
        {
            Format = "png",
            ShowLegend = false,
            Height = 240,
            Width = 320,
            ColorMap = new Dictionary<string, string> { ["cat"] = "#ff0000" },
            Label = "Segments",
            Scale = 2,
            MinWidth = 200,
            Buttons = ["fullscreen"],
        });
        blocks.Add(new Label
        {
            Value = new Dictionary<string, object?> { ["cat"] = 0.7, ["dog"] = 0.3 },
            NumTopClasses = 1,
            LabelText = "Guess",
            Scale = 2,
            MinWidth = 200,
            Color = "#ff0000",
            ShowHeading = false,
            Buttons = ["copy"],
        });
    }

    /// <summary>
    /// The plot family: a chart with nothing set, one with every parameter that has a value to
    /// choose set, the two other marks, and the two shapes of a plot.
    /// </summary>
    /// <param name="blocks">The application being rendered.</param>
    /// <remarks>
    /// <para>
    /// The second line plot is drawn from a table, which is the case's proof of the value's shape:
    /// the columns, the rows in those columns' order, and the kind of each column — a number, a
    /// category, a moment — which the Python version infers from a pandas dtype and this side
    /// infers from the values themselves. The two other marks are given tables of their own,
    /// because the mark is part of the value rather than of the block: a bar plot's rows are drawn
    /// as bars and a scatter plot's as points.
    /// </para>
    /// <para>
    /// The two plots come last of the family for the same reason: the first has nothing set, so its
    /// defaults are compared, and the second carries a drawing an application already had — which
    /// is the passthrough, and the only shape a plot's value takes here.
    /// </para>
    /// </remarks>
    private static void AddPlots(Blocks blocks)
    {
        var table = new Dictionary<string, IReadOnlyList<object?>>
        {
            ["x"] = [1, 2, 3],
            ["y"] = [4.5, 5.5, 6.5],
            ["series"] = ["a", "b", "a"],
            ["when"] =
            [
                new System.DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new System.DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                new System.DateTime(2020, 1, 3, 0, 0, 0, DateTimeKind.Utc),
            ],
        };

        blocks.Add(new LinePlot());
        blocks.Add(new LinePlot
        {
            Value = table,
            X = "x",
            Y = "y",
            Color = "series",
            Title = "Sales",
            XTitle = "Month",
            YTitle = "Units",
            ColorTitle = "Region",
            XBin = 10,
            YAggregate = "mean",
            ColorMap = new Dictionary<string, string> { ["a"] = "#00ff00", ["b"] = "#0000ff" },
            ColorsInLegend = ["a"],
            XLim = [0, null],
            YLim = [0, 100],
            XLabelAngle = 45,
            YLabelAngle = -30,
            XAxisFormat = ".2e",
            YAxisFormat = "~g",
            XAxisLabelsVisible = Visibility.Hidden,
            Caption = "A caption",
            Sort = "-x",
            Tooltip = "all",
            Height = 300,
            Label = "Trend",
            Scale = 2,
            MinWidth = 200,
            Buttons = ["fullscreen", "export"],
        });

        // The two other marks are drawn from the same two columns, written the way an application
        // writes a table: a dictionary from a name to that column's values.
        var small = new Dictionary<string, object?[]>
        {
            ["x"] = [1, 2, 3],
            ["y"] = [4, 5, 6],
        };

        blocks.Add(new BarPlot { Value = small, X = "x", Y = "y" });
        blocks.Add(new ScatterPlot { Value = small, X = "x", Y = "y" });

        blocks.Add(new Plot());
        blocks.Add(new Plot
        {
            Value = new PlotData("matplotlib", Figure),
            Format = "png",
            Label = "Figure",
        });
    }

    /// <summary>The one-pixel PNG the second plot carries, as the case's drawing.</summary>
    private const string Figure =
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQ"
        + "AAAABJRU5ErkJggg==";

    /// <summary>
    /// The nine layouts, nested inside one another, so that one document describes all of them.
    /// </summary>
    /// <param name="blocks">The application being rendered.</param>
    /// <remarks>
    /// <para>
    /// They come after the components for two reasons: a layout changes where the blocks written
    /// after it go, and every parameter of every layout is set to a value the default would not
    /// produce, so a mapping that writes a default is caught rather than passing.
    /// </para>
    /// <para>
    /// The nesting is the point. A row holding two columns, one of which holds a group with an
    /// accordion and a sidebar in it, is the shape that exercises the grouping rule at depth: each
    /// container groups what is written into it when it closes, so the ids and the layout tree say
    /// whether that happened in the right order. The compact column is there because it is the one
    /// layout that switches the grouping off, the draggable holds two groups with nothing in them,
    /// and the tabs and the walkthrough hold the items they are allowed to hold.
    /// </para>
    /// </remarks>
    private static void AddLayouts(Blocks blocks)
    {
        var row = blocks.Add(new Row
        {
            Variant = "panel",
            Scale = 2,
            Height = 240,
            MaxHeight = "20rem",
            MinHeight = 120,
            EqualHeight = true,
            ShowProgress = true,
            ElemId = "the-row",
            ElemClasses = ["wide"],
        });

        using (blocks.Enter(row))
        {
            // A form component written straight into a row is grouped into a form of the row's
            // own, and that form is as wide as the field it holds — which is the only way the
            // case can cover the form, because the Python version does not export `Form` for an application
            // to write.
            blocks.Add(new Textbox { Label = "In the row", Scale = 2, MinWidth = 150 });

            var column = blocks.Add(new Column
            {
                Scale = 3,
                MinWidth = 200,
                Variant = "panel",
                ShowProgress = true,
                ElemId = "the-column",
            });

            using (blocks.Enter(column))
            {
                // A group is the one layout that says nothing but where its children go; hidden is
                // the third value a `visible` can take, and the only one a boolean cannot say.
                var group = blocks.Add(new Group
                {
                    ElemId = "the-group",
                    Visible = Visibility.Hidden,
                });

                using (blocks.Enter(group))
                {
                    var accordion = blocks.Add(new Accordion
                    {
                        Label = "Details",
                        Open = false,
                        Height = 160,
                        MaxHeight = "12rem",
                    });

                    using (blocks.Enter(accordion))
                    {
                        blocks.Add(new Textbox { Label = "Inside the accordion" });
                    }

                    var sidebar = blocks.Add(new Sidebar
                    {
                        Label = "Tools",
                        Open = false,
                        Width = "18rem",
                        Position = "right",
                    });

                    using (blocks.Enter(sidebar))
                    {
                        blocks.Add(new Number { Label = "Inside the sidebar" });
                    }
                }
            }

            // The compact column is drawn as one block, so the two fields in it stay where they
            // were written instead of being grouped into a form of their own.
            var compact = blocks.Add(new Column { Variant = "compact", Scale = 1, MinWidth = 120 });

            using (blocks.Enter(compact))
            {
                blocks.Add(new Textbox { Label = "Compact A" });
                blocks.Add(new Textbox { Label = "Compact B" });
            }
        }

        var draggable = blocks.Add(new Draggable { Orientation = "row" });

        using (blocks.Enter(draggable))
        {
            blocks.Add(new Group());
            blocks.Add(new Group());
        }

        var tabs = blocks.Add(new Tabs
        {
            Selected = 2,
            OverflowBehavior = "wrap",
            ElemId = "the-tabs",
        });

        using (blocks.Enter(tabs))
        {
            var first = blocks.Add(new Tab
            {
                Label = "One",
                TabId = 1,
                Alignment = "left",
                Scale = 2,
                Height = 120,
                MaxHeight = "8rem",
                RenderChildren = true,
            });

            using (blocks.Enter(first))
            {
                blocks.Add(new Textbox { Label = "In tab one" });
            }

            var second = blocks.Add(new Tab
            {
                Label = "Two",
                TabId = 2,
                Alignment = "right",
                Interactive = false,
                Visible = false,
            });

            using (blocks.Enter(second))
            {
                blocks.Add(new Number { Label = "In tab two" });
            }
        }

        var walkthrough = blocks.Add(new Walkthrough { Selected = 2 });

        using (blocks.Enter(walkthrough))
        {
            var start = blocks.Add(new WalkthroughStep
            {
                Label = "Start",
                StepId = 1,
                Scale = 2,
            });

            using (blocks.Enter(start))
            {
                blocks.Add(new Textbox { Label = "In step one" });
            }

            var finish = blocks.Add(new WalkthroughStep
            {
                Label = "Finish",
                StepId = 2,
                Interactive = false,
            });

            using (blocks.Enter(finish))
            {
                blocks.Add(new Number { Label = "In step two" });
            }
        }
    }

    /// <summary>
    /// The HTML component, and a custom one an application defines for itself.
    /// </summary>
    /// <param name="blocks">The application being rendered.</param>
    /// <remarks>
    /// <para>
    /// They come last because both are containers that are not layouts: written after the layouts,
    /// they are the page's last root children and move no id the rest of the page depends on.
    /// </para>
    /// <para>
    /// The first is the Python version's own HTML component with every parameter that has a value
    /// to choose set to one the default would not produce — a template of its own, a stylesheet, a
    /// load script, a head, two heights, and one server function, whose name is what the config's
    /// <c>server_fns</c> carries. The second is <see cref="Callout"/>, which is what an application
    /// writes when it wants a component of its own: a subclass of the HTML component with two
    /// parameters the Python version's HTML does not have, carried both as props of their own and
    /// in the dictionary the template reads.
    /// </para>
    /// </remarks>
    private static void AddCustomHtml(Blocks blocks)
    {
        blocks.Add(new Html
        {
            Value = "<p>Anything the application writes</p>",
            Label = "Note",
            HtmlTemplate = "<div class=\"note\">${value}</div>",
            CssTemplate = ".note { padding: var(--spacing-md); }",
            JsOnLoad =
                "element.querySelectorAll('.note').forEach(node => "
                + "node.addEventListener('click', () => trigger('click')));",
            ApplyDefaultCss = false,
            MinHeight = 80,
            MaxHeight = 200,
            Padding = true,
            Autoscroll = true,
            Head = "<script src=\"https://example.com/widget.js\"></script>",
            ServerFunctions = new Dictionary<string, Delegate>
            {
                ["describe"] = (Func<JsonNode?, JsonNode?>)(data => data?.DeepClone()),
            },
            Scale = 2,
            MinWidth = 200,
        });

        blocks.Add(new Callout
        {
            Value = "Everything is fine.",
            Heading = "Status",
            Tone = "warning",
            Label = "A component of the application's own",
        });
    }

    /// <summary>
    /// The utility components: the value an endpoint carries, the any-JSON component, the clock
    /// and the workflow canvas.
    /// </summary>
    /// <param name="blocks">The application being rendered.</param>
    /// <remarks>
    /// They come last for the reason the custom-HTML ones do: they are the page's last root
    /// children and move no id the rest of the page depends on. Each appears with the defaults and
    /// again with a parameter the default would not produce, because the two halves are equally
    /// part of the mapping: the first API component's example is its value and the second's is
    /// <c>...</c>; the second fallback's note, width and hidden label are what the Python version
    /// writes when an application asks for them; the second timer starts stopped; and the second
    /// canvas carries a server function and the id of the view a read-only visitor is shown.
    /// </remarks>
    private static void AddUtilityComponents(Blocks blocks)
    {
        blocks.Add(new Api
        {
            Value = JsonValue.Create("hello"),
            Schema = new JsonObject { ["type"] = "string" },
            Label = "first",
        });

        blocks.Add(new Api
        {
            Schema = new JsonObject { ["type"] = "number" },
            Label = "second",
        });

        blocks.Add(new Fallback
        {
            Value = JsonValue.Create("x"),
            Label = "fb",
        });

        blocks.Add(new Fallback
        {
            Value = new JsonObject { ["a"] = 1 },
            Label = "fb2",
            Info = "note",
            Scale = 2,
            MinWidth = 120,
            Interactive = true,
            ShowLabel = false,
        });

        blocks.Add(new Timer());

        blocks.Add(new Timer
        {
            Value = 5,
            Active = false,
        });

        blocks.Add(new WorkflowCanvas());

        blocks.Add(new WorkflowCanvas
        {
            Value = "{\"nodes\":[]}",
            Label = "flow",
            ShowLabel = true,
            Container = true,
            ServerFunctions = new Dictionary<string, Delegate>
            {
                ["describe"] = (Func<JsonNode?, JsonNode?>)(data => data?.DeepClone()),
            },
            AppView = "the-model",
        });
    }
}
