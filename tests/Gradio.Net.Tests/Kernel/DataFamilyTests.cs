using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Server;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The data family as a handler and as the frontend see it: what a value becomes on the way in,
/// what a handler's result becomes on the way out, and what a component computes when the
/// application says nothing.
/// </summary>
/// <remarks>
/// The manifest page compares every prop of every component against the Python version, so nothing
/// here asserts what the config carries. What it asserts is the half of a component a config cannot
/// show: the defaults that are computed rather than stored, the conversion of a value into what a
/// handler asked for, and the refusals.
/// </remarks>
public sealed class DataFamilyTests
{
    /// <summary>A one-pixel PNG, which is the smallest base image a box can be drawn over.</summary>
    private const string OnePixelPng =
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==";

    /// <summary>The columns a table with nothing set names.</summary>
    private static readonly string[] _positionalHeaders = ["1", "2", "3"];

    /// <summary>The columns a table names when the application names them.</summary>
    private static readonly string[] _namedHeaders = ["a", "b"];

    /// <summary>What a table infers each column of a mixed row holds.</summary>
    private static readonly string[] _inferredDatatypes = ["str", "number", "bool"];

    /// <summary>The headers a table of three columns pads a single name out to.</summary>
    private static readonly string[] _paddedHeaders = ["a", "2", "3"];

    /// <summary>The headers a table of one column keeps from a list of three.</summary>
    private static readonly string[] _truncatedHeaders = ["a"];

    /// <summary>The headers a CSV file's first row names.</summary>
    private static readonly string[] _csvHeaders = ["a", "b"];

    /// <summary>The headers a dataset takes from the labels of the components it holds.</summary>
    private static readonly string[] _datasetHeaders = ["Word", "Count"];

    /// <summary>The headers of a dataset one of whose components has no label.</summary>
    private static readonly string[] _partlyNamedHeaders = ["", "Count"];

    /// <summary>The headers of a dataset whose first component has no label.</summary>
    private static readonly string[] _unlabelledFirstHeader = ["", "Word"];

    /// <summary>The first sample of the dataset the tests use.</summary>
    private static readonly object?[] _catSample = ["cat", 3];

    /// <summary>The second sample of the dataset the tests use.</summary>
    private static readonly object?[] _dogSample = ["dog", 5];

    /// <summary>A sample the dataset the tests use does not hold.</summary>
    private static readonly object?[] _cowSample = ["cow", 7];

    /// <summary>The samples of the dataset the tests use.</summary>
    private static readonly IReadOnlyList<object?>[] _samples = [_catSample, _dogSample];

    [Fact]
    public void A_table_names_its_columns_by_position_when_the_application_names_none()
    {
        // The Python version's default table is three columns wide, and an unnamed column is called by the
        // position it holds — one-based, as text.
        Assert.Equal(_positionalHeaders, new Dataframe().Headers);

        // Naming the columns moves the count with them, so the two cannot disagree by accident.
        var named = new Dataframe { Headers = ["a", "b"] };

        Assert.Equal(_namedHeaders, named.Headers);
        Assert.Equal(2, ((IReadOnlyList<object?>)named.ColumnCount!)[0]);
    }

    /// <summary>
    /// A table hands a handler rows when its type names the array shape, and a table otherwise.
    /// </summary>
    /// <remarks>
    /// The Python version's four names are two shapes: <c>array</c> and <c>numpy</c> are the rows,
    /// and <c>pandas</c> and <c>polars</c> are a table. C# has no numpy to be a second shape of, so
    /// both array names answer with rows. What the test pins is the part that could silently
    /// differ: a number arrives as a number rather than as the text the table draws, and the rows
    /// do not carry the headers, because a row of cells is not a table.
    /// </remarks>
    [Fact]
    public void A_table_hands_a_handler_rows_when_its_type_names_the_array_shape()
    {
        var document = new JsonObject
        {
            ["headers"] = new JsonArray("name", "count"),
            ["data"] = new JsonArray(new JsonArray("cat", 7)),
        };

        foreach (var type in new[] { "array", "numpy" })
        {
            var rows = Assert.IsAssignableFrom<IReadOnlyList<IReadOnlyList<object?>>>(
                new Dataframe { Type = type }.Preprocess(document.DeepClone()));

            var row = Assert.Single(rows);

            Assert.Equal("cat", row[0]);
            Assert.Equal(7.0, row[1]);
        }

        // The table shapes — what a handler reads unless it says otherwise — carry the headers.
        foreach (var type in new[] { "pandas", "polars" })
        {
            var table = Assert.IsType<DataframeTable>(
                new Dataframe { Type = type }.Preprocess(document.DeepClone()));

            Assert.Equal(["name", "count"], table.Headers);
            Assert.Equal("cat", Assert.Single(table.Rows)[0]);
        }
    }

    /// <summary>
    /// The rows a handler was given are what the browser is sent when it hands them back.
    /// </summary>
    /// <remarks>
    /// A handler that reads rows returns rows — the commonest thing an application does with a
    /// table is nothing at all — so the way out has to take the shape the way in produced. This is
    /// the whole trip: what an <c>array</c> handler reads, put back through the component that has
    /// the headers, and out as the document the frontend draws.
    /// </remarks>
    [Fact]
    public void A_table_hands_back_the_rows_a_handler_was_given()
    {
        var document = new JsonObject
        {
            ["headers"] = new JsonArray("name", "count"),
            ["data"] = new JsonArray(
                new JsonArray("cat", 7),
                new JsonArray("dog", 3)),
        };

        var rows = new Dataframe { Type = "array" }.Preprocess(document.DeepClone());

        var sent = Assert.IsType<JsonObject>(
            new Dataframe { Headers = ["name", "count"] }.Postprocess(rows));

        Assert.Equal(
            ["name", "count"],
            sent["headers"]!.AsArray().Select(header => header!.GetValue<string>()));

        var data = sent["data"]!.AsArray();

        Assert.Equal(2, data.Count);
        Assert.Equal("cat", data[0]![0]!.GetValue<string>());
        Assert.Equal(7.0, data[0]![1]!.GetValue<double>());
        Assert.Equal("dog", data[1]![0]!.GetValue<string>());
    }

    /// <summary>
    /// A column width is written as pixels unless it already says what it is.
    /// </summary>
    /// <remarks>
    /// The Python version's rule, and it is the kind of thing a config comparison cannot catch: the
    /// manifest page is given widths that are already spelled out, so the normalisation would never
    /// run. A bare number is a number of pixels; anything already ending in <c>px</c> or <c>%</c>,
    /// or spelled <c>auto</c>, is left as it is.
    /// </remarks>
    [Fact]
    public void A_table_writes_a_bare_column_width_as_pixels()
    {
        var table = new Dataframe { ColumnWidths = new object?[] { 80, "120px", "50%", "auto" } };

        Assert.Equal(["80px", "120px", "50%", "auto"], table.ColumnWidths);
    }

    [Fact]
    public void A_table_refuses_headers_that_do_not_match_its_column_count()
    {
        var table = new Dataframe { Headers = ["a", "b", "c"], ColumnCount = 2 };

        Assert.Throws<ArgumentException>(() => table.Headers);
    }

    [Fact]
    public void A_table_ignores_the_deprecated_column_count_it_is_given()
    {
        // The Python version accepts `col_count`, warns, and then overwrites it with `column_count`; the
        // config carries both keys with the same value, which is the pair the count resolves to.
        var table = new Dataframe { ColumnCount = 2, ColCount = (5, "fixed") };
        var count = (IReadOnlyList<object?>)table.ColCount!;

        Assert.Equal(2, count[0]);
        Assert.Equal("dynamic", count[1]);
    }

    [Fact]
    public void A_table_infers_what_each_column_holds_from_its_first_row()
    {
        // A table infers only when it is asked to; with nothing said it reports one type, as
        // the Python version's default does.
        var table = new Dataframe
        {
            Datatype = "auto",
            Value = new object?[] { new object?[] { "a", 1, true } },
        };

        Assert.Equal(_inferredDatatypes, Assert.IsType<string[]>(table.Datatype));
    }

    [Fact]
    public void A_table_pads_and_truncates_its_headers_to_the_width_of_its_rows()
    {
        // A table whose rows are wider than its headers names the extra columns by position.
        var wide = new object?[] { new object?[] { 1, 2, 3 } };
        var padded = Assert.IsType<JsonObject>(new Dataframe { Headers = ["a"] }.Postprocess(wide));

        Assert.Equal(
            _paddedHeaders,
            padded["headers"]!.AsArray().Select(header => header!.GetValue<string>()));

        // And one whose headers are wider than its rows drops the headers it has no column for.
        var narrow = new object?[] { new object?[] { 1 } };
        var truncated = Assert.IsType<JsonObject>(
            new Dataframe { Headers = ["a", "b", "c"] }.Postprocess(narrow));

        Assert.Equal(
            _truncatedHeaders,
            truncated["headers"]!.AsArray().Select(header => header!.GetValue<string>()));
    }

    [Fact]
    public void A_table_refuses_a_type_it_does_not_know_but_carries_a_search_it_does_not()
    {
        // The Python version validates the result type — the value's shape is its own to produce — and does
        // not validate the search, which is the frontend's to draw. The difference is deliberate:
        // refusing what the Python version serves would make a page unreachable here.
        Assert.Throws<ArgumentException>(() => new Dataframe { Type = "csv" });
        Assert.Equal("fuzzy", new Dataframe { ShowSearch = "fuzzy" }.ShowSearch);
    }

    [Fact]
    public void A_table_reads_a_document_with_no_data_as_a_table_with_no_rows()
    {
        // The Python version's `value.get("data", [[]])`: a document that names only its headers is an empty
        // table rather than a refusal.
        var value = Assert.IsType<JsonObject>(
            new Dataframe().Postprocess(new JsonObject { ["headers"] = new JsonArray("a") }));

        Assert.Empty(value["data"]!.AsArray());
    }

    [Fact]
    public void A_table_reads_a_csv_path_as_its_first_row_of_headers_and_the_rest_as_data()
    {
        using var folder = new TempFolder();
        var path = folder.Write("table.csv", "a,b\n1,\"two, three\"\n4,5\n");

        var value = Assert.IsType<JsonObject>(new Dataframe().Postprocess(path));

        Assert.Equal(
            _csvHeaders,
            value["headers"]!.AsArray().Select(header => header!.GetValue<string>()));

        // Two data rows, and a quoted field is one cell rather than two.
        Assert.Equal(2, value["data"]!.AsArray().Count);
        Assert.Equal("two, three", value["data"]![0]![1]!.GetValue<string>());
    }

    [Fact]
    public void A_dataset_names_its_headers_after_the_components_it_holds()
    {
        var dataset = new Dataset
        {
            Components = [new Textbox { Label = "Word" }, new Number { Label = "Count" }],
        };

        Assert.Equal(_datasetHeaders, dataset.Headers);

        // A component with no label is a column with no name, which is the Python version's rule: the list is
        // empty only when every component is unnamed.
        var unnamed = new Dataset { Components = [new Textbox(), new Number { Label = "Count" }] };

        Assert.Equal(_partlyNamedHeaders, unnamed.Headers);
        Assert.Empty(new Dataset { Components = [new Textbox()] }.Headers);
    }

    [Fact]
    public void A_dataset_hands_a_handler_the_sample_its_index_names()
    {
        var dataset = new Dataset
        {
            Components = [new Textbox { Label = "Word" }, new Number { Label = "Count" }],
            Samples = _samples,
        };

        var picked = dataset.Preprocess(JsonValue.Create(1));
        Assert.Equal(_dogSample, Assert.IsAssignableFrom<IReadOnlyList<object?>>(picked));

        // What a handler asked for is what it gets: the values, the index, or both.
        dataset.Type = "index";
        Assert.Equal(1, dataset.Preprocess(JsonValue.Create(1)));

        dataset.Type = "tuple";
        var pair = Assert.IsAssignableFrom<ITuple>(dataset.Preprocess(JsonValue.Create(1)));
        Assert.Equal(1, pair[0]);
        Assert.Equal(_dogSample, Assert.IsAssignableFrom<IReadOnlyList<object?>>(pair[1]));

        // An index is answered with itself before the sample is looked up, so a type that wants
        // only the index survives an index the dataset no longer holds — as the Python version's does.
        dataset.Type = "index";
        Assert.Equal(9, dataset.Preprocess(JsonValue.Create(9)));

        // And a type that is none of the three hands the handler nothing, which is what the Python version's
        // chain of `elif`s does.
        dataset.Type = "weird";
        Assert.Null(dataset.Preprocess(JsonValue.Create(0)));
    }

    [Fact]
    public void A_dataset_copies_the_parameters_of_the_components_it_holds_and_nothing_else()
    {
        // The Python version recovers the copy through the component's signature, so a key its config carries
        // that is not a constructor parameter — an image's `streamable` — is not in it.
        var dataset = new Dataset { Components = [new Image(), new Textbox { Label = "Word" }] };
        var props = Assert.IsAssignableFrom<IReadOnlyList<JsonObject>>(dataset.ComponentProps);

        Assert.False(props[0].ContainsKey("streamable"));
        Assert.True(props[0].ContainsKey("sources"));

        // The value is the one key the copy leaves out: a dataset draws the sample it was given
        // rather than the component's own default.
        Assert.False(props[1].ContainsKey("value"));
        Assert.Equal(_unlabelledFirstHeader, dataset.Headers);
    }

    [Fact]
    public void A_dataset_answers_a_sample_with_the_index_it_holds()
    {
        var dataset = new Dataset
        {
            Components = [new Textbox(), new Number()],
            Samples = _samples,
        };

        Assert.Equal(1, dataset.Postprocess(_dogSample)!.GetValue<int>());

        // A sample the dataset does not hold has no index, which is the Python version's answer: the
        // application is expected to return a new dataset instead.
        Assert.Null(dataset.Postprocess(_cowSample));
    }

    [Fact]
    public void A_json_viewer_reads_a_string_as_json_and_refuses_one_that_is_not()
    {
        var viewer = new JSON();
        var parsed = Assert.IsType<JsonObject>(viewer.Postprocess("{\"a\": 1}"));

        Assert.Equal(1, parsed["a"]!.GetValue<int>());

        Assert.Throws<ArgumentException>(() => viewer.Postprocess("{"));
        Assert.Null(viewer.Postprocess(null));
    }

    [Fact]
    public void A_parameter_viewer_carries_a_language_it_does_not_know()
    {
        // The Python version stores the language and lets the frontend highlight with it; there is no list it
        // is checked against.
        Assert.Equal("cobol", new ParamViewer { Language = "cobol" }.Language);

        // The two parameters that have a default an application would not otherwise get.
        var viewer = new ParamViewer();
        Assert.Equal("Parameters", viewer.Header);
        Assert.False(Assert.IsType<bool>(viewer.AnchorLinks));
    }

    [Fact]
    public void Highlighted_text_drops_the_whitespace_it_is_told_not_to_show()
    {
        var text = new HighlightedText
        {
            Value = new HighlightedToken[] { new("Hello "), new("world", "greeting") },
        };

        var shown = Assert.IsType<JsonArray>(text.Postprocess(text.Value));
        Assert.Equal("Hello ", shown[0]!["token"]!.GetValue<string>());

        text.ShowWhitespaces = false;
        var trimmed = Assert.IsType<JsonArray>(text.Postprocess(text.Value));
        Assert.Equal("Hello", trimmed[0]!["token"]!.GetValue<string>());
    }

    [Fact]
    public void Highlighted_text_merges_neighbours_of_the_same_class_when_it_is_asked_to()
    {
        var text = new HighlightedText
        {
            Value = new HighlightedToken[] { new("one", "a"), new("two", "a"), new("three", "b") },
            CombineAdjacent = true,
            AdjacentSeparator = "|",
        };

        var merged = Assert.IsType<JsonArray>(text.Postprocess(text.Value));

        Assert.Equal(2, merged.Count);
        Assert.Equal("one|two", merged[0]!["token"]!.GetValue<string>());
        Assert.Equal("a", merged[0]!["class_or_confidence"]!.GetValue<string>());
    }

    [Fact]
    public void A_classification_keeps_the_most_confident_classes_it_was_asked_for()
    {
        var value = new Dictionary<string, object?> { ["cat"] = 0.2, ["dog"] = 0.7, ["cow"] = 0.1 };
        var label = new Label { Value = value, NumTopClasses = 2 };

        var classified = Assert.IsType<JsonObject>(label.Postprocess(value));

        Assert.Equal("dog", classified["label"]!.GetValue<string>());

        var confidences = Assert.IsType<JsonArray>(classified["confidences"]);
        Assert.Equal(2, confidences.Count);
        Assert.Equal("dog", confidences[0]!["label"]!.GetValue<string>());
        Assert.Equal(0.7, confidences[0]!["confidence"]!.GetValue<double>());
    }

    [Fact]
    public void A_classification_with_no_value_is_an_empty_map_and_a_bare_label_has_no_confidences()
    {
        var label = new Label();

        Assert.Empty(Assert.IsType<JsonObject>(label.Postprocess(null)));

        var bare = Assert.IsType<JsonObject>(label.Postprocess("cat"));
        Assert.Equal("cat", bare["label"]!.GetValue<string>());
        Assert.Null(bare["confidences"]);
    }

    [Fact]
    public void An_annotated_image_draws_a_box_into_a_picture_the_size_of_its_base_image()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        var basePath = folder.Write("base.png", Convert.FromBase64String(OnePixelPng));
        using var blocks = RenderAnnotatedImage(provider, folder, out var component);

        component.Value = (basePath, new[] { new Annotation((0, 0, 1, 1), "cat") });

        var described = Assert.IsType<JsonObject>(component.Value);
        var mask = described["annotations"]![0]!;

        Assert.Equal("cat", mask["label"]!.GetValue<string>());

        // The mask is a PNG of the base image's size: the box is painted into a picture rather than
        // sent as coordinates, which is what the frontend draws.
        var maskPath = mask["image"]!["path"]!.GetValue<string>();
        Assert.NotEqual(basePath, maskPath);
        Assert.True(MediaCache.IsInFolder(maskPath, folder.Folder));

        var bytes = System.IO.File.ReadAllBytes(maskPath);
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal(1, System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(16, 4)));
        Assert.Equal(1, System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(20, 4)));
    }

    /// <summary>
    /// A base image in a format the header readers did not know is measured by decoding it.
    /// </summary>
    /// <remarks>
    /// The mask has to be the size of the base image, so its size has to be known. PNG and JPEG
    /// carry theirs in a header this can read without decoding; everything else is decoded, which
    /// is what the Python version does for every format and what makes a WebP base a picture rather
    /// than a refusal.
    /// </remarks>
    [Fact]
    public void An_annotated_image_measures_a_base_image_the_header_readers_did_not_know()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        var webp = RasterImage.Open(TestImages.Png(3, 2)).ToBytes("webp");
        var basePath = folder.Write("base.webp", webp);
        using var blocks = RenderAnnotatedImage(provider, folder, out var component);

        component.Value = (basePath, new[] { new Annotation((0, 0, 2, 1), "cat") });

        var described = Assert.IsType<JsonObject>(component.Value);
        var maskPath = described["annotations"]![0]!["image"]!["path"]!.GetValue<string>();
        var mask = RasterImage.Open(maskPath);

        Assert.Equal(3, mask.Width);
        Assert.Equal(2, mask.Height);
    }

    [Fact]
    public void An_annotated_image_refuses_a_box_over_an_image_whose_size_it_cannot_read()
    {
        var component = new AnnotatedImage
        {
            Value = ("https://example.com/base.png", new[] { new Annotation((0, 0, 10, 10), "cat") }),
        };

        // The size is not known until something fetches the picture, which is the file endpoint's
        // subject; the refusal says so rather than drawing a mask of the wrong size.
        Assert.Throws<NotSupportedException>(() => component.Value);
    }

    [Fact]
    public void An_annotated_image_hands_a_handler_the_paths_the_client_sent()
    {
        var value = new AnnotatedImage().Preprocess(new JsonObject
        {
            ["image"] = new JsonObject { ["path"] = "/tmp/base.png" },
            ["annotations"] = new JsonArray(
                new JsonObject
                {
                    ["image"] = new JsonObject { ["path"] = "/tmp/mask.png" },
                    ["label"] = "cat",
                }),
        });

        var pair = Assert.IsAssignableFrom<ITuple>(value);
        Assert.Equal("/tmp/base.png", pair[0]);

        var annotations = Assert.IsAssignableFrom<IReadOnlyList<Annotation>>(pair[1]);
        Assert.Single(annotations);
        Assert.Equal("/tmp/mask.png", annotations[0].Mask);
        Assert.Equal("cat", annotations[0].Label);
    }

    /// <summary>Renders an annotated image into an application that keeps its files in a folder of the test's own.</summary>
    /// <param name="provider">The container the application is built from.</param>
    /// <param name="folder">The application's upload folder.</param>
    /// <param name="component">The component, as added.</param>
    /// <returns>The application, which the caller disposes.</returns>
    private static Blocks RenderAnnotatedImage(
        IServiceProvider provider,
        TempFolder folder,
        out AnnotatedImage component)
    {
        var blocks = provider.CreateBlocksWithContainers();
        blocks.UploadFolder = folder.Folder;

        using (blocks.BeginRender())
        {
            var image = new AnnotatedImage();
            component = blocks.Add(image);
        }

        return blocks;
    }
}
