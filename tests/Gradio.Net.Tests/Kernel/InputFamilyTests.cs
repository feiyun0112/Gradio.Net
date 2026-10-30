using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;
using DateTime = Gradio.Net.DateTime;
namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The text and number family as a handler and as the frontend see it: what a value becomes on the
/// way in, what a handler's result becomes on the way out, and what a component computes when the
/// application says nothing.
/// </summary>
/// <remarks>
/// The manifest page compares every prop of every component against the Python version, so nothing
/// here asserts what the config carries. What it asserts is the half of a component a config cannot
/// show: the defaults that are computed rather than stored, the conversion of a value into what a
/// handler asked for, and the refusals — each of them a pure computation with no HTTP in it.
/// </remarks>
public sealed class InputFamilyTests
{
    [Fact]
    public void A_slider_takes_a_hundredth_of_its_range_as_its_step()
    {
        // The default range is a hundred wide, so the step is one; a range of ten and a half is
        // three orders of magnitude down from a hundred, so the step is a tenth.
        Assert.Equal(1, new Slider().Step);
        Assert.Equal(0.1, new Slider { Minimum = -5, Maximum = 5.5 }.Step);

        // And a slider nobody gave a value to starts at the bottom of its range.
        Assert.Equal(0, new Slider().Value);
        Assert.Equal(-5, new Slider { Minimum = -5, Maximum = 5.5 }.Value);
    }

    [Fact]
    public void A_slider_refuses_a_range_that_is_not_positive()
    {
        var slider = new Slider { Minimum = 5, Maximum = 5 };

        Assert.Throws<ArgumentException>(() => slider.Step);
    }

    [Fact]
    public void A_slider_rounds_its_value_and_refuses_one_outside_its_range()
    {
        var slider = new Slider
        {
            Minimum = 0,
            Maximum = 10,
            Value = 1.234,
            Precision = 2,
        };

        Assert.Equal(1.23, slider.Value);
        Assert.Equal(1.23, slider.Preprocess(JsonValue.Create(1.234)));

        Assert.Throws<ArgumentOutOfRangeException>(() => slider.Preprocess(JsonValue.Create(20)));
    }

    /// <summary>
    /// A number field with no decimals hands over a whole number, which is what the Python version
    /// hands over.
    /// </summary>
    /// <remarks>
    /// The Python version's <c>round_to_precision</c> returns <c>int(round(num, 0))</c> at zero
    /// precision, so a Python handler annotated <c>int</c> receives one. A <see cref="long"/>
    /// rather than an <see cref="int"/> here, because a C# number field has no bound on its range
    /// and a value that did not fit would throw instead of rounding; the run converts it to
    /// whatever the handler declared.
    /// </remarks>
    [Fact]
    public void A_number_hands_over_a_whole_number_when_its_precision_is_zero()
    {
        var whole = new Number { Precision = 0 };
        var fractional = new Number { Precision = 2 };

        Assert.Equal(3L, whole.Preprocess(JsonValue.Create(3.4)));
        Assert.Equal(3.4, fractional.Preprocess(JsonValue.Create(3.4)));

        // And a number is rounded on the way out as well, which is what the field shows.
        Assert.Equal("4", whole.Postprocess(3.6)!.ToJsonString());
        Assert.Equal("3.6", fractional.Postprocess(3.6)!.ToJsonString());
        Assert.Equal("3.46", fractional.Postprocess(3.456)!.ToJsonString());
    }

    [Fact]
    public void A_checkbox_shows_its_label_even_when_the_application_hides_it()
    {
        // The Python version's checkbox turns the label on whenever there is one: a labelled box with its
        // label hidden shows an unexplained tick.
        Assert.True(new Checkbox { Label = "Agree", ShowLabel = false }.ShowLabel);
        Assert.True(new Checkbox { Label = "Agree", Container = false }.ShowLabel);

        // A box with no label has none to show.
        Assert.False(new Checkbox { Container = false }.ShowLabel);
        Assert.True(new Checkbox().ShowLabel);
    }

    [Fact]
    public void A_checkbox_group_reads_a_single_value_as_a_list_of_one()
    {
        var group = new CheckboxGroup
        {
            Choices = [new Choice("a"), new Choice("b")],
            Value = "b",
        };

        Assert.Equal(["b"], Assert.IsAssignableFrom<IReadOnlyList<object?>>(group.Value));

        // Nothing chosen is an empty list rather than an absent value.
        Assert.Empty(Assert.IsAssignableFrom<IReadOnlyList<object?>>(new CheckboxGroup().Value));
    }

    [Fact]
    public void A_checkbox_group_reports_positions_when_it_says_index()
    {
        var group = new CheckboxGroup
        {
            Choices = [new Choice("a"), new Choice("b")],
            Type = "index",
        };

        Assert.Equal([1], Assert.IsAssignableFrom<IReadOnlyList<object?>>(group.Preprocess(
            new JsonArray("b"))));

        Assert.Throws<ArgumentException>(() => group.Preprocess(new JsonArray("c")));
    }

    /// <summary>
    /// A value the checkbox group does not offer is refused with the Python version's message.
    /// </summary>
    /// <remarks>
    /// The Python version raises <c>f"Value: {value!r} (type: {type(value)}) is not in the list of
    /// choices: {choice_values}"</c>, so the text names the value as Python's <c>repr</c> spells
    /// it, the type Python would have given it, and the values on offer. The three parts are what
    /// makes the report actionable, and they are why the message is reproduced rather than
    /// approximated. The suffix .NET appends to an <see cref="ArgumentException"/> is the one part
    /// that cannot be the Python version's.
    /// </remarks>
    [Fact]
    public void A_checkbox_group_names_the_value_it_refuses_the_way_upstream_does()
    {
        var group = new CheckboxGroup
        {
            Choices = [new Choice("a"), new Choice("b"), new Choice(2)],
        };

        var refused = Assert.Throws<ArgumentException>(
            () => group.Preprocess(new JsonArray("c")));

        Assert.StartsWith(
            "Value: 'c' (type: <class 'str'>) is not in the list of choices: ['a', 'b', 2]",
            refused.Message,
            StringComparison.Ordinal);

        // A number keeps the spelling the client wrote: "7" is an int and "7.5" a float, which is
        // what the Python version's parser makes of them and what the message says.
        Assert.StartsWith(
            "Value: 7 (type: <class 'int'>) is not in the list of choices: ['a', 'b', 2]",
            Assert.Throws<ArgumentException>(() => group.Preprocess(new JsonArray(7))).Message,
            StringComparison.Ordinal);

        Assert.StartsWith(
            "Value: 7.5 (type: <class 'float'>) is not in the list of choices: ['a', 'b', 2]",
            Assert.Throws<ArgumentException>(() => group.Preprocess(new JsonArray(7.5))).Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void A_choice_pair_is_the_label_then_the_value()
    {
        // The Python version's own shape: gr.Radio(choices=[("Two", 2)]) offers "Two" and hands the
        // handler 2. The pair is written label-first, which is the opposite of the constructor.
        var radio = gr.Radio(choices: [("Two", 2), ("Three", 3)]);

        Assert.Equal("Two", radio.Choices[0].Label);
        Assert.Equal(2, radio.Choices[0].Value);
        Assert.Equal("Three", radio.Choices[1].Label);
        Assert.Equal(3, radio.Choices[1].Value);
    }

    [Fact]
    public void A_dropdown_starts_on_its_first_choice_unless_it_is_told_otherwise()
    {
        var choices = new[] { new Choice("small"), new Choice("medium") };

        // Not set: the first choice. Set to null: nothing selected. No choices at all: nothing.
        Assert.Equal("small", new Dropdown { Choices = choices }.Value);
        Assert.Null(new Dropdown { Choices = choices, Value = null }.Value);
        Assert.Null(new Dropdown().Value);
        Assert.Empty(Assert.IsAssignableFrom<IReadOnlyList<object?>>(
            new Dropdown { Choices = choices, Multiselect = true }.Value));
    }

    [Fact]
    public void A_multiple_dropdown_reads_a_single_value_as_a_list_of_one()
    {
        var text = new Dropdown
        {
            Choices = [new Choice("small"), new Choice("medium")],
            Multiselect = true,
            Value = "medium",
        };

        Assert.Equal(["medium"], Assert.IsAssignableFrom<IReadOnlyList<object?>>(text.Value));

        // A number is wrapped the same way: the API schema says a multiple dropdown holds a list,
        // and a value described as a bare number would contradict it.
        var number = new Dropdown
        {
            Choices = [new Choice(1), new Choice(2)],
            Multiselect = true,
            Value = 2,
        };

        Assert.Equal([2.0], Assert.IsAssignableFrom<IReadOnlyList<object?>>(number.Value).Select(Convert.ToDouble));
    }

    [Fact]
    public void A_dropdown_takes_a_value_that_is_not_on_its_list_only_when_it_allows_one()
    {
        var choices = new[] { new Choice("small") };

        var open = new Dropdown { Choices = choices, AllowCustomValue = true };
        Assert.Equal("huge", open.Preprocess(JsonValue.Create("huge")));

        // A dropdown that allows custom values has to be typeable, whatever the application said.
        Assert.True(new Dropdown { AllowCustomValue = true, Filterable = false }.Filterable);

        var closed = new Dropdown { Choices = choices };
        Assert.Throws<ArgumentException>(() => closed.Preprocess(JsonValue.Create("huge")));
    }

    /// <summary>
    /// A value the dropdown does not offer is refused with the Python version's message — both of
    /// them.
    /// </summary>
    /// <remarks>
    /// The Python version has two: a value inside a list is named with <c>repr</c> and its type,
    /// and a value on its own is named with <c>str</c>, which drops a text value's quotes. The
    /// second reads like an oversight, and reproducing it is the point: a client that compares the
    /// two messages against the Python version's has to find the same text.
    /// </remarks>
    [Fact]
    public void A_dropdown_names_the_value_it_refuses_the_way_upstream_does()
    {
        var multiple = new Dropdown
        {
            Choices = [new Choice("small"), new Choice("medium")],
            Multiselect = true,
        };

        Assert.StartsWith(
            "Value: 'huge' (type: <class 'str'>) is not in the list of choices: ['small', 'medium']",
            Assert.Throws<ArgumentException>(
                () => multiple.Preprocess(new JsonArray("small", "huge"))).Message,
            StringComparison.Ordinal);

        var single = new Dropdown { Choices = [new Choice("small")] };

        Assert.StartsWith(
            "Value: huge is not in the list of choices: ['small']",
            Assert.Throws<ArgumentException>(
                () => single.Preprocess(JsonValue.Create("huge"))).Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void A_dropdown_refuses_a_number_of_choices_that_is_not_positive()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Dropdown { NumChoicesShown = 0 });
    }

    [Fact]
    public void A_radio_reports_positions_when_it_says_index()
    {
        var radio = new Radio
        {
            Choices = [new Choice("x"), new Choice(1), new Choice(2.5)],
            Type = "index",
        };

        // The value arrived from a browser as one JSON number, and it still names the choice that
        // was written as an integer.
        Assert.Equal(1, radio.Preprocess(JsonValue.Create(1.0)));
        Assert.Equal(2, radio.Preprocess(JsonValue.Create(2.5)));

        Assert.Throws<ArgumentException>(() => radio.Preprocess(JsonValue.Create(7)));
    }

    /// <summary>
    /// A value the radio does not offer is refused with the Python version's message.
    /// </summary>
    /// <remarks>
    /// The radio raises the same message the checkbox group does — one value rather than a list of
    /// them — so this is the second call site of <see cref="Choice.Refuse"/>.
    /// </remarks>
    [Fact]
    public void A_radio_names_the_value_it_refuses_the_way_upstream_does()
    {
        var radio = new Radio { Choices = [new Choice("red"), new Choice("green")] };

        var refused = Assert.Throws<ArgumentException>(
            () => radio.Preprocess(JsonValue.Create(7)));

        Assert.StartsWith(
            "Value: 7 (type: <class 'int'>) is not in the list of choices: ['red', 'green']",
            refused.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void A_multimodal_textbox_is_an_empty_field_until_it_is_given_something()
    {
        var value = Assert.IsType<MultimodalText>(new MultimodalTextbox().Value);

        Assert.Equal(string.Empty, value.Text);
        Assert.Empty(value.Files);

        // A handler that answers with text alone still produces both halves of the value.
        var posted = Assert.IsType<JsonObject>(new MultimodalTextbox().Postprocess("hi"));

        Assert.Equal("hi", posted["text"]!.GetValue<string>());
        Assert.Empty(Assert.IsType<JsonArray>(posted["files"]));
    }

    [Fact]
    public void A_multimodal_textbox_refuses_a_file_it_was_not_told_to_accept()
    {
        var field = new MultimodalTextbox { FileTypes = [".png"] };
        var picture = new JsonObject
        {
            ["text"] = string.Empty,
            ["files"] = new JsonArray(new JsonObject { ["path"] = "/tmp/cat.png" }),
        };
        var document = new JsonObject
        {
            ["text"] = string.Empty,
            ["files"] = new JsonArray(new JsonObject { ["path"] = "/tmp/notes.txt" }),
        };

        Assert.IsType<MultimodalText>(field.Preprocess(picture));
        Assert.Throws<ArgumentException>(() => field.Preprocess(document));

        // An entry that names a kind of file accepts every media type of that kind.
        var images = new MultimodalTextbox { FileTypes = ["image"] };
        Assert.IsType<MultimodalText>(images.Preprocess(picture));
    }

    /// <summary>
    /// A file the field does not accept is refused with the Python version's message.
    /// </summary>
    /// <remarks>
    /// The Python version raises <c>f"Invalid file type: {f.mime_type}. Please upload a file that
    /// is one of these formats: {self.file_types}"</c>, so the kinds are listed as Python writes a
    /// list — with their quotes — rather than joined into prose.
    /// </remarks>
    [Fact]
    public void A_multimodal_textbox_names_the_file_it_refuses_the_way_upstream_does()
    {
        var field = new MultimodalTextbox { FileTypes = [".png", ".jpg"] };
        var document = new JsonObject
        {
            ["text"] = string.Empty,
            ["files"] = new JsonArray(new JsonObject { ["path"] = "/tmp/notes.txt" }),
        };

        Assert.StartsWith(
            "Invalid file type: text/plain. Please upload a file that is one of these formats: ['.png', '.jpg']",
            Assert.Throws<ArgumentException>(() => field.Preprocess(document)).Message,
            StringComparison.Ordinal);

        // The media type is the one the client gave the file when it gave one, which is where
        // the Python version reads it from: a payload that says what it uploaded is believed over its name.
        var claimed = new JsonObject
        {
            ["text"] = string.Empty,
            ["files"] = new JsonArray(new JsonObject
            {
                ["path"] = "/tmp/notes.txt",
                ["mime_type"] = "application/x-custom",
            }),
        };

        Assert.StartsWith(
            "Invalid file type: application/x-custom. Please upload a file that is one of these formats",
            Assert.Throws<ArgumentException>(() => field.Preprocess(claimed)).Message,
            StringComparison.Ordinal);
    }

    /// <summary>
    /// A textbox binds every signal the Python version declares for one — including the two that
    /// carry data of their own.
    /// </summary>
    /// <remarks>
    /// The Python version's textbox raises eight: change, input, select, submit, focus, blur, stop
    /// and copy. The last two are the ones a config cannot show, because nothing about a textbox
    /// says it can be selected until something listens — which is what binding <c>select</c>
    /// changes.
    /// </remarks>
    [Fact]
    public void A_textbox_binds_the_signals_upstream_declares()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        Textbox field;
        Textbox other;

        using (blocks.BeginRender())
        {
            field = blocks.Add(new Textbox { Label = "A" });
            other = blocks.Add(new Textbox { Label = "B" });
        }

        Assert.False(field.Selectable);

        var signals = new List<EventTrigger>
        {
            field.Events.Change,
            field.Events.Input,
            field.Events.Submit!,
            field.Events.Focus,
            field.Events.Blur,
            field.Events.Stop,
            field.Events.Select,
            field.Events.Copy,
        };

        // One handler for all eight, which is also the only way to reach the two that carry data:
        // their payload arrives as a parameter of the handler rather than as an input.
        var binding = blocks.On(
            signals,
            fn: (Func<string, object?>)(value => value), inputs: [field], outputs: [other]);

        Assert.Equal(
            ["change", "input", "submit", "focus", "blur", "stop", "select", "copy"],
            binding.Targets.Select(target => target.EventName));

        // Binding the selection is what makes the component offer it: the Python version sets the flag from
        // the listener's callback, and the frontend draws the affordance from the config.
        Assert.True(field.Selectable);
    }

    [Fact]
    public void A_date_and_time_reads_a_moment_relative_to_now()
    {
        var field = new DateTime { Type = "datetime" };

        var moment = Assert.IsType<System.DateTime>(
            field.Preprocess(JsonValue.Create("now-5d")));

        Assert.InRange(
            (System.DateTime.Now - moment).TotalDays,
            4.9,
            5.1);

        // A spelling that is not a relative moment is refused rather than read as a date, with
        // the Python version's message: it is what a user who typed "now-5w" is shown.
        var refused = Assert.Throws<ArgumentException>(
            () => field.Preprocess(JsonValue.Create("now-5w")));

        Assert.StartsWith("Invalid 'now' time format", refused.Message, StringComparison.Ordinal);

        // Text that is not a moment at all is refused by the format the field expects, as Python's
        // `strptime` refuses it — with the format spelled the way Python spells one.
        var unparsed = Assert.Throws<ArgumentException>(
            () => field.Preprocess(JsonValue.Create("the day before yesterday")));

        Assert.StartsWith(
            "time data 'the day before yesterday' does not match format '%Y-%m-%d %H:%M:%S'",
            unparsed.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void A_code_editor_carries_its_code_without_the_whitespace_around_it()
    {
        var code = new Code { Value = "  x = 1  ", Lines = 10, MaxLines = 3 };

        Assert.Equal("x = 1", code.Value);

        // The editor is never shorter than the lines it was told to show.
        Assert.Equal(10, code.MaxLines);

        Assert.Throws<ArgumentException>(() => new Code { Language = "cobol" });
    }

    [Fact]
    public void A_date_and_time_carries_text_as_it_was_given_and_formats_a_moment()
    {
        // Text is never reformatted: a handler that asked for text gets the spelling it was given.
        Assert.Equal("2020-10-01", new DateTime { Value = "2020-10-01", IncludeTime = false }.Value);

        var moment = new System.DateTime(2020, 10, 1, 5, 20, 15);

        Assert.Equal(
            "2020-10-01 05:20:15",
            new DateTime { Value = moment }.Value);
        Assert.Equal(
            "2020-10-01",
            new DateTime { Value = moment, IncludeTime = false }.Value);

        Assert.Throws<ArgumentException>(() => new DateTime { Type = "epoch" });
    }

    [Fact]
    public void A_choice_compares_numbers_by_value()
    {
        var pair = new Choice(1, "One");

        Assert.Equal("One", pair.Label);

        // A bare value is its own label.
        Assert.Equal("a", new Choice("a").Label);

        // A number that came back from a browser is a double, and it still names the integer choice.
        Assert.True(pair.Matches(1.0));
        Assert.False(pair.Matches(2.0));

        Assert.Equal(1, Choice.IndexOf([new Choice("a"), pair], 1.0));
    }
}
