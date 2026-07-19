using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Gradio.Net.Components;
using Gradio.Net.Core;
using Gradio.Net.Core.Exceptions;
using Gradio.Net.Core.Flagging;
using Gradio.Net.Core.Layouts;
using Gradio.Net.Events;


namespace Gradio.Net;

public class Interface : Blocks
{
    // Properties
    public object DeepLink { get; set; }
    public int? TimeLimit { get; set; }
    public double StreamEvery { get; set; }
    public string ApiName { get; set; }
    public object ApiDescription { get; set; }
    public string ApiVisibility { get; set; }
    public string InterfaceType { get; set; }
    public bool ApiMode { get; set; }
    public Delegate Fn { get; set; }
    public Delegate Validator { get; set; }
    public double[] FnDurations { get; set; }
    public string Name { get; set; }
    public bool Live { get; set; }
    public string Title { get; set; }
    public string SimpleDescription { get; set; }
    public string Description { get; set; }
    public string Article { get; set; }
    public object Examples { get; set; }
    public bool? CacheExamples { get; set; }
    public string CacheMode { get; set; }
    public object PreloadExample { get; set; }
    public int ExamplesPerPage { get; set; }
    public List<string> ExampleLabels { get; set; }
    public object SubmitBtnParms { get; set; }
    public object StopBtnParms { get; set; }
    public object ClearBtnParams { get; set; }
    public object SimpleServer { get; set; }
    public string FlaggingMode { get; set; }
    public List<Tuple<string, object>> FlaggingOptions { get; set; }
    public FlaggingCallback FlaggingCallback { get; set; }
    public string FlaggingDir { get; set; }
    public string ShowProgress { get; set; }
    public bool Batch { get; set; }
    public int MaxBatchSize { get; set; }
    public bool AllowDuplication { get; set; }
    public object ConcurrencyLimit { get; set; }
    public string Share { get; set; }
    public string ShareUrl { get; set; }
    public string LocalUrl { get; set; }
    public string FaviconPath { get; set; }
    public object I18nInstance { get; set; }
    public List<Component> MainInputComponents { get; set; }
    public List<Component> AdditionalInputComponents { get; set; }
    public List<Component> InputComponents { get; set; }
    public List<Component> OutputComponents { get; set; }
    public object AdditionalInputsAccordionParams { get; set; }
    public object ExamplesHandler { get; set; }

    public static Interface FromPipeline(object pipeline, Dictionary<string, object> kwargs = null)
    {
        var interfaceInfo = Pipelines.LoadFromPipeline(pipeline);
        if (kwargs != null)
        {
            foreach (var kvp in kwargs)
            {
                interfaceInfo[kvp.Key] = kvp.Value;
            }
        }
        // Extract parameters from interfaceInfo dictionary
        var fn = interfaceInfo.ContainsKey("fn") ? interfaceInfo["fn"] : null;
        var inputs = interfaceInfo.ContainsKey("inputs") ? interfaceInfo["inputs"] : null;
        var outputs = interfaceInfo.ContainsKey("outputs") ? interfaceInfo["outputs"] : null;
        var title = interfaceInfo.ContainsKey("title") ? interfaceInfo["title"] : null;

        return new Interface(
            (Delegate)fn,
            inputs,
            outputs,
            title: title
        );
    }

    public Interface(
        Delegate fn,
        object inputs,
        object outputs,
        object examples = null,
        bool? cacheExamples = null,
        string cacheMode = null,
        int examplesPerPage = 10,
        List<string> exampleLabels = null,
        object preloadExample = null,
        bool live = false,
        object title = null,
        string description = null,
        string article = null,
        string flaggingMode = null,
        object flaggingOptions = null,
        string flaggingDir = ".gradio/flagged",
        object flaggingCallback = null,
        bool? analyticsEnabled = null,
        bool batch = false,
        int maxBatchSize = 4,
        string apiVisibility = "public",
        string apiName = "predict",
        object apiDescription = null,
        bool apiMode = false,
        bool allowDuplication = false,
        object concurrencyLimit = null,
        object additionalInputs = null,
        object additionalInputsAccordion = null,
        object submitBtn = null,
        object stopBtn = null,
        object clearBtn = null,
        Tuple<int, int> deleteCache = null,
        string showProgress = "full",
        bool fillWidth = false,
        int? timeLimit = 30,
        double streamEvery = 0.5,
        object deepLink = null,
        Delegate validator = null,
        Dictionary<string, object> kwargs = null
    ) : base(
        title: title?.ToString() ?? "Gradio",
        analyticsEnabled: analyticsEnabled,
        mode: "interface",
        fillHeight: false
    )
    {
        // Initialize properties
        if (deepLink is string deepLinkStr)
        {
            this.DeepLink = new { value = deepLinkStr, render = false, interactive = false };
        }
        else if (deepLink is bool deepLinkBool && deepLinkBool)
        {
            this.DeepLink = new { render = false, interactive = false };
        }
        else if (false && deepLink == null)
        {
            this.DeepLink = new { render = false, interactive = false };
        }
        else if (deepLink is bool deepLinkBoolFalse && !deepLinkBoolFalse)
        {
            this.DeepLink = null;
        }
        else
        {
            this.DeepLink = deepLink;
        }

        this.TimeLimit = timeLimit;
        this.StreamEvery = streamEvery;
        this.ApiName = string.IsNullOrWhiteSpace(apiName) ? "predict" : apiName;
        this.ApiDescription = apiDescription;
        this.ApiVisibility = apiVisibility;
        this.InterfaceType = "STANDARD";

        // Handle inputs and outputs
        if ((inputs == null || (inputs is List<object> && ((List<object>)inputs).Count == 0)) &&
            (outputs == null || (outputs is List<object> && ((List<object>)outputs).Count == 0)))
        {
            throw new ArgumentException("Must provide at least one of `inputs` or `outputs`");
        }
        else if (outputs == null || (outputs is List<object> && ((List<object>)outputs).Count == 0))
        {
            this.OutputComponents = new List<Component>();
            this.InterfaceType = "INPUT_ONLY";
        }
        else if (inputs == null || (inputs is List<object> && ((List<object>)inputs).Count == 0))
        {
            this.InputComponents = new List<Component>();
            this.InterfaceType = "OUTPUT_ONLY";
        }

        // Handle additional inputs
        this.AdditionalInputComponents = new List<Component>();

        static List<object> NormalizeToObjectList(object value)
        {
            if (value == null)
            {
                return new List<object>();
            }

            if (value is string || value is Component)
            {
                return new List<object> { value };
            }

            if (value is IEnumerable enumerable)
            {
                var result = new List<object>();
                foreach (var item in enumerable)
                {
                    result.Add(item!);
                }
                return result;
            }

            return new List<object> { value };
        }

        static Component ToComponent(object value, string paramName)
        {
            return value switch
            {
                Component component => component,
                string componentType => ComponentFactory.CreateComponent(componentType),
                _ => throw new ArgumentException($"Each item in `{paramName}` must be a string or Component, not {value.GetType()}")
            };
        }

        if (additionalInputs != null)
        {
            var additionalInputsList = NormalizeToObjectList(additionalInputs);
            this.AdditionalInputComponents = additionalInputsList.Select(i => ToComponent(i, "additionalInputs")).ToList();
        }

        // Validate inputs and outputs types
        if (inputs != null && !(inputs is IEnumerable) && !(inputs is Component) && !(inputs is string))
        {
            throw new ArgumentException($"inputs must be a string, list, or Component, not {inputs.GetType()}");
        }
        if (outputs != null && !(outputs is IEnumerable) && !(outputs is Component) && !(outputs is string))
        {
            throw new ArgumentException($"outputs must be a string, list, or Component, not {outputs.GetType()}");
        }

        // Process inputs
        var inputList = NormalizeToObjectList(inputs);
        this.MainInputComponents = inputList.Select(i => ToComponent(i, "inputs")).ToList();
        this.InputComponents = new List<Component>(this.MainInputComponents);
        this.InputComponents.AddRange(this.AdditionalInputComponents);

        // Process outputs
        var outputList = NormalizeToObjectList(outputs);
        this.OutputComponents = outputList.Select(o => ToComponent(o, "outputs")).ToList();

        // Python parity: get_component_instance(i, unrender=True)
        // Components created from string types may have auto-rendered into the current
        // (outer) Blocks context. Unrender them so they register properly inside
        // this Interface's own Enter()/Exit() context when Render() is called explicitly.
        var allCreatedComponents = this.InputComponents.Concat(this.OutputComponents).Distinct();
        foreach (var comp in allCreatedComponents)
        {
            if (comp.IsRendered)
            {
                comp.Unrender();
            }
        }

        // Handle state components
        // Find string "state" placeholders in input and output
        var stateInputIndexes = new List<int>();
        var stateOutputIndexes = new List<int>();

        for (int i = 0; i < this.InputComponents.Count; i++)
        {
            var componentType = this.InputComponents[i].GetType().Name.ToLower();
            if (componentType.Contains("state"))
            {
                stateInputIndexes.Add(i);
            }
        }

        for (int i = 0; i < this.OutputComponents.Count; i++)
        {
            var componentType = this.OutputComponents[i].GetType().Name.ToLower();
            if (componentType.Contains("state"))
            {
                stateOutputIndexes.Add(i);
            }
        }

        // Validate state usage - if using state, must have exactly one input and one output
        if (stateInputIndexes.Count > 0 || stateOutputIndexes.Count > 0)
        {
            if (stateInputIndexes.Count != 1 || stateOutputIndexes.Count != 1)
            {
                throw new ArgumentException(
                    "If using 'state', there must be exactly one state input and one state output."
                );
            }

            var stateInputIndex = stateInputIndexes[0];
            var stateOutputIndex = stateOutputIndexes[0];

            // Use same state instance for input and output
            this.OutputComponents[stateOutputIndex] = this.InputComponents[stateInputIndex];

            // Disable cache examples when using state
            if (cacheExamples == true)
            {
                cacheExamples = false;
            }
        }


        // Handle additional inputs accordion
        if (additionalInputsAccordion == null)
        {
            this.AdditionalInputsAccordionParams = new Dictionary<string, object>
            {
                { "label", "Additional Inputs" },
                { "open", false }
            };
        }
        else if (additionalInputsAccordion is string accordionLabel)
        {
            this.AdditionalInputsAccordionParams = new Dictionary<string, object>
            {
                { "label", accordionLabel }
            };
        }
        else if (additionalInputsAccordion is Accordion accordion)
        {
            this.AdditionalInputsAccordionParams = new Dictionary<string, object>
            {
                { "label", accordion.Label },
                { "open", accordion.Open }
            };
        }
        else
        {
            throw new ArgumentException($"The `additional_inputs_accordion` parameter must be a string or gr.Accordion, not {additionalInputsAccordion.GetType()}");
        }

        // Validate components
        foreach (var component in this.InputComponents.Concat(this.OutputComponents))
        {
            if (!(component is Component))
            {
                throw new ArgumentException($"{component} is not a valid input/output component for Interface.");
            }
        }

        // Check for unified interface type
        if (this.InputComponents.Count == this.OutputComponents.Count)
        {
            bool allSame = true;
            for (int i = 0; i < this.InputComponents.Count; i++)
            {
                if (!ReferenceEquals(this.InputComponents[i], this.OutputComponents[i]))
                {
                    allSame = false;
                    break;
                }
            }
            if (allSame)
            {
                this.InterfaceType = "UNIFIED";
            }
        }

        // Set output components to non-interactive by default
        if (this.InterfaceType == "STANDARD" || this.InterfaceType == "OUTPUT_ONLY")
        {
            foreach (var component in this.OutputComponents)
            {
                if (!(component is Component))
                {
                    throw new ArgumentException($"Output component must be a Component, not {component.GetType()}");
                }
                var comp = (Component)component;
                if (comp.Interactive == null)
                {
                    comp.Interactive = false;
                }
            }
        }

        // Set other properties
        this.ApiMode = apiMode;
        this.Fn = fn;
        this.Validator = validator;
        this.FnDurations = new double[] { 0, 0 };
        this.Name = fn?.Method?.Name ?? "fn";
        this.Live = live;
        this.Title = title?.ToString();
        this.SimpleDescription = description != null ? description : null;
        this.Description = description;
        if (article != null)
        {
            this.Article = article;
        }
        this.Examples = examples;
        this.CacheExamples = cacheExamples;
        this.CacheMode = cacheMode;
        this.PreloadExample = preloadExample;
        this.ExamplesPerPage = examplesPerPage;
        this.ExampleLabels = exampleLabels;

        // Handle buttons
        if (submitBtn is Button submitButton)
        {
            this.SubmitBtnParms = new Dictionary<string, object>
            {
                { "value", submitButton.Value },
                { "variant", submitButton.Variant }
            };
        }
        else if (submitBtn is string submitButtonStr)
        {
            this.SubmitBtnParms = new Dictionary<string, object>
            {
                { "value", submitButtonStr },
                { "variant", "primary" }
            };
        }
        else
        {
            throw new ArgumentException($"The submit_btn parameter must be a gr.Button or string, not {submitBtn.GetType()}");
        }

        if (stopBtn is Button stopButton)
        {
            this.StopBtnParms = new Dictionary<string, object>
            {
                { "value", stopButton.Value },
                { "variant", stopButton.Variant }
            };
        }
        else if (stopBtn is string stopButtonStr)
        {
            this.StopBtnParms = new Dictionary<string, object>
            {
                { "value", stopButtonStr },
                { "variant", "stop" },
                { "visible", false }
            };
        }
        else
        {
            throw new ArgumentException($"The stop_btn parameter must be a gr.Button or string, not {stopBtn.GetType()}");
        }

        if (clearBtn == null)
        {
            this.ClearBtnParams = new Dictionary<string, object>
            {
                { "visible", false },
                { "variant", "secondary" }
            };
        }
        else if (clearBtn is Button clearButton)
        {
            this.ClearBtnParams = new Dictionary<string, object>
            {
                { "value", clearButton.Value },
                { "variant", clearButton.Variant }
            };
        }
        else if (clearBtn is string clearButtonStr)
        {
            this.ClearBtnParams = new Dictionary<string, object>
            {
                { "value", clearButtonStr },
                { "variant", "secondary" }
            };
        }
        else
        {
            throw new ArgumentException($"The clear_btn parameter must be a gr.Button, a string, or None, not {clearBtn.GetType()}");
        }

        // Handle flagging mode
        if (flaggingMode == null)
        {
            this.FlaggingMode = Environment.GetEnvironmentVariable("GRADIO_FLAGGING_MODE") ?? "manual";
        }
        else if (new[] { "manual", "never", "auto" }.Contains(flaggingMode))
        {
            this.FlaggingMode = flaggingMode;
        }
        else
        {
            throw new ArgumentException("Invalid value for `flagging_mode` parameter. Must be: 'auto', 'manual', or 'never'.");
        }

        // Handle flagging options
        if (flaggingOptions == null)
        {
            this.FlaggingOptions = new List<Tuple<string, object>> { Tuple.Create("Flag", (object)null) };
        }
        else if (flaggingOptions is List<string> flaggingOptionsList)
        {
            this.FlaggingOptions = flaggingOptionsList.Select(x => Tuple.Create($"Flag as {x}", (object)x)).ToList();
        }
        else if (flaggingOptions is List<Tuple<string, string>> flaggingOptionsTuples)
        {
            this.FlaggingOptions = flaggingOptionsTuples.Select(x => Tuple.Create(x.Item1, (object)x.Item2)).ToList();
        }
        else
        {
            throw new ArgumentException("flagging_options must be a list of strings or list of (string, string) tuples.");
        }

        // Handle flagging callback
        if (flaggingCallback == null)
        {
            this.FlaggingCallback = new CSVLogger();
        }
        else
        {
            this.FlaggingCallback = (FlaggingCallback)flaggingCallback;
        }

        this.FlaggingDir = flaggingDir;
        this.ShowProgress = showProgress;
        this.Batch = batch;
        this.MaxBatchSize = maxBatchSize;
        this.AllowDuplication = allowDuplication;
        this.ConcurrencyLimit = concurrencyLimit;

        // Set up parameter names for components
        var paramNames = new List<string>();
        try
        {
            var methodInfo = fn.Method;
            paramNames = methodInfo.GetParameters().Select(p => p.Name).ToList();
        }
        catch (Exception)
        {
            paramNames = Enumerable.Range(0, this.InputComponents.Count).Select(i => $"Input {i + 1}").ToList();
        }

        // Set component labels
        for (int i = 0; i < Math.Min(this.InputComponents.Count, paramNames.Count); i++)
        {
            var component = this.InputComponents[i];
            var comp = (Component)component;
            if (comp.Label == null)
            {
                comp.Label = paramNames[i];
            }
        }

        for (int i = 0; i < this.OutputComponents.Count; i++)
        {
            var component = this.OutputComponents[i];
            var comp = (Component)component;
            if (comp.Label == null)
            {
                if (this.OutputComponents.Count == 1)
                {
                    comp.Label = "output";
                }
                else
                {
                    comp.Label = $"output {i}";
                }
            }
        }

        // Set up flagging callback
        if (this.FlaggingMode != "never")
        {
            this.FlaggingCallback ??= new CSVLogger();

            if (this.InterfaceType == "UNIFIED")
            {
                var callback = (FlaggingCallback)this.FlaggingCallback;
                callback.Setup(this.InputComponents, this.FlaggingDir);
            }
            else if (this.InterfaceType != "INPUT_ONLY")
            {
                var callback = (FlaggingCallback)this.FlaggingCallback;
                var components = new List<Component>(this.InputComponents);
                components.AddRange(this.OutputComponents);
                callback.Setup(components, this.FlaggingDir);
            }
        }

        // Render the Gradio UI
        this.Enter();
        try
        {
            if (this.DeepLink != null)
            {
                // DeepLink.Activate();
            }
            this.RenderTitleDescription();

            Button submitBtnObj = null;
            Button clearBtnObj = null;
            Button stopBtnObj = null;
            List<Button> flagBtns = null;
            object duplicateBtn = null;
            Column inputComponentColumn = null;

            var mainRow = new Row();
            mainRow.Enter();
            try
            {
                if (this.InterfaceType == "STANDARD" ||
                    this.InterfaceType == "INPUT_ONLY" ||
                    this.InterfaceType == "UNIFIED")
                {
                    var result = this.RenderInputColumn();
                    submitBtnObj = (Button)result.Item1;
                    clearBtnObj = (Button)result.Item2;
                    stopBtnObj = (Button)result.Item3;
                    flagBtns = result.Item4?.Cast<Button>().ToList();
                    inputComponentColumn = result.Item5;
                }
                if (this.InterfaceType == "STANDARD" ||
                    this.InterfaceType == "OUTPUT_ONLY")
                {
                    var result = this.RenderOutputColumn(submitBtnObj);
                    var submitBtnOut = (Button)result.Item1;
                    var clearBtnOut = (Button)result.Item2;
                    duplicateBtn = result.Item3;
                    var stopBtnOut = (Button)result.Item4;
                    var flagBtnsOut = result.Item5;

                    submitBtnObj = submitBtnObj ?? submitBtnOut;
                    clearBtnObj = clearBtnObj ?? clearBtnOut;
                    stopBtnObj = stopBtnObj ?? stopBtnOut;
                    flagBtns = flagBtns ?? (flagBtnsOut?.Cast<Button>().ToList() ?? new List<Button>());
                }
            }
            finally
            {
                mainRow.Exit();
            }

            if (clearBtnObj == null)
            {
                throw new RenderError("Clear button not rendered");
            }

            var submitEvent = this.AttachSubmitEvents(submitBtnObj, stopBtnObj);
            this.AttachClearEvents(clearBtnObj, inputComponentColumn);
            if (duplicateBtn != null)
            {
                // duplicateBtn.Activate();
            }

            this.AttachFlaggingEvents(flagBtns, clearBtnObj, submitEvent);
            // if (submitEvent != null && this.DeepLink != null)
            // {
            //     submitEvent.Then(
            //         () => new DeepLinkButton(interactive: true),
            //         inputs: null,
            //         outputs: new List<object> { this.DeepLink },
            //         js: true,
            //         apiVisibility: "undocumented"
            //     );
            // }
            this.RenderExamples();
            this.RenderArticle();
        }
        finally
        {
            this.Exit();
        }

        // Python parity: Blocks.__init__ calls self.render() when there is an active
        // render context (get_blocks_context() is truthy).  That means writing
        //   gr.Interface(...) inside a `with demo.route(...):` block auto-renders
        // without an explicit .render() call.  Mirror that here: if the Interface
        // was constructed while a Blocks context is active, render it now.
        if (BlockContext.GetRenderContext() != null)
        {
            this.Render();
        }
    }

    public void RenderTitleDescription()
    {
        if (this.Title != null)
        {
            new Markdown($"<h1 style='text-align: center; margin-bottom: 1rem'>{this.Title}</h1>");
        }
        if (this.Description != null)
        {
            new Markdown(this.Description);
        }
    }

    public List<Button> RenderFlagBtns()
    {
        return this.FlaggingOptions.Select(option => new Button(option.Item1)).ToList();
    }

    public Tuple<object, object, object, List<object>, Column> RenderInputColumn()
    {
        object submitBtn = null;
        object clearBtn = null;
        object stopBtn = null;
        List<object> flagBtns = null;

        var inputColumn = new Column();
        inputColumn.Enter();
        try
        {
            var inputComponentColumn = new Column();
            inputComponentColumn.Enter();
            try
            {
                foreach (var component in this.MainInputComponents)
                {
                    component.Render();
                }
                if (this.AdditionalInputComponents.Count > 0)
                {
                    var additionalInputsAccordionParams = this.AdditionalInputsAccordionParams as Dictionary<string, object>;
                    var accordionLabel = additionalInputsAccordionParams != null && additionalInputsAccordionParams.TryGetValue("label", out var labelObj)
                        ? labelObj?.ToString()
                        : null;
                    object openObj = null;
                    var hasOpen = additionalInputsAccordionParams != null && additionalInputsAccordionParams.TryGetValue("open", out openObj);
                    var accordionOpen = hasOpen && openObj is bool openValue ? openValue : true;

                    var additionalInputsContainer = hasOpen
                        ? new Accordion(label: accordionLabel, open: accordionOpen)
                        : new Accordion(label: accordionLabel);

                    additionalInputsContainer.Enter();
                    try
                    {
                        foreach (var component in this.AdditionalInputComponents)
                        {
                            component.Render();
                        }
                    }
                    finally
                    {
                        additionalInputsContainer.Exit();
                    }
                }
            }
            finally
            {
                inputComponentColumn.Exit();
            }

            var actionsRow = new Row();
            actionsRow.Enter();
            try
            {
                if (this.InterfaceType == "STANDARD" || this.InterfaceType == "INPUT_ONLY")
                {
                    clearBtn = new ClearButton(value: "Clear", variant: "secondary");
                    if (!this.Live)
                    {
                        if (this.DeepLink != null && this.InterfaceType == "INPUT_ONLY")
                        {
                            // DeepLink.Render();
                        }
                        submitBtn = new Button("Submit", variant: "primary");

                        // Check if function is a true generator (yield/async yield),
                        // not just any IEnumerable return type such as List<T>.
                        var methodInfo = this.Fn?.Method;
                        bool isGenerator = IsGeneratorFunction(methodInfo);

                        if (isGenerator)
                        {
                            stopBtn = new Button("Stop", variant: "stop", visible: false);
                        }
                    }
                }
                else if (this.InterfaceType == "UNIFIED")
                {
                    clearBtn = new ClearButton(value: "Clear", variant: "secondary");
                    submitBtn = new Button("Submit", variant: "primary");
                    if (this.DeepLink != null)
                    {
                        // DeepLink.Render();
                    }

                    var methodInfo = this.Fn?.Method;
                    bool isGenerator = IsGeneratorFunction(methodInfo);

                    if (isGenerator && !this.Live)
                    {
                        stopBtn = new Button("Stop", variant: "stop");
                    }
                    if (this.FlaggingMode == "manual")
                    {
                        flagBtns = this.RenderFlagBtns().Cast<object>().ToList();
                    }
                    else if (this.FlaggingMode == "auto")
                    {
                        flagBtns = new List<object> { submitBtn };
                    }
                }
            }
            finally
            {
                actionsRow.Exit();
            }
            return Tuple.Create(submitBtn, clearBtn, stopBtn, flagBtns, inputComponentColumn);
        }
        finally
        {
            inputColumn.Exit();
        }
    }

    public Tuple<object, object, object, object, List<object>> RenderOutputColumn(object submitBtnIn)
    {
        var submitBtn = submitBtnIn;
        object clearBtn = null;
        object duplicateBtn = null;
        List<object> flagBtns = null;
        object stopBtn = null;

        var outputColumn = new Column();
        outputColumn.Enter();
        try
        {
            foreach (var component in this.OutputComponents)
            {
                if (component is State)
                {
                    continue;
                }
                component.Render();
            }
            var outputActionsRow = new Row();
            outputActionsRow.Enter();
            try
            {
                if (this.DeepLink != null)
                {
                    // DeepLink.Render();
                }
                if (this.InterfaceType == "OUTPUT_ONLY")
                {
                    clearBtn = new ClearButton(value: "Clear", variant: "secondary");
                    submitBtn = new Button("Generate", variant: "primary");

                    var methodInfo = this.Fn?.Method;
                    bool isGenerator = IsGeneratorFunction(methodInfo);

                    if (isGenerator && !this.Live)
                    {
                        stopBtn = new Button("Stop", variant: "stop", visible: false);
                    }
                }
                if (this.FlaggingMode == "manual")
                {
                    flagBtns = this.RenderFlagBtns().Cast<object>().ToList();
                }
                else if (this.FlaggingMode == "auto")
                {
                    if (submitBtn == null)
                    {
                        throw new RenderError("Submit button not rendered");
                    }
                    flagBtns = new List<object> { submitBtn };
                }

                if (this.AllowDuplication)
                {
                    duplicateBtn = new Button("Duplicate", variant: "secondary");
                }
            }
            finally
            {
                outputActionsRow.Exit();
            }
            return Tuple.Create(submitBtn, clearBtn, duplicateBtn, stopBtn, flagBtns);
        }
        finally
        {
            outputColumn.Exit();
        }
    }

    public void RenderArticle()
    {
        if (this.Article != null)
        {
            new Markdown(this.Article);
        }
    }

    private static bool ComponentHasEvent(Component component, string eventName)
    {
        if (component == null || string.IsNullOrWhiteSpace(eventName))
        {
            return false;
        }

        // Prefer instance-level events if available.
        if (component.Events?.Any(e =>
                string.Equals(e.EventName, eventName, StringComparison.OrdinalIgnoreCase)) == true)
        {
            return true;
        }

        // Check [Events.Event("xxx")] class attributes on the component type.
        Type? type = component.GetType();
        while (type != null)
        {
            var eventAttrs = type.GetCustomAttributes<Events.EventAttribute>(inherit: false);
            if (eventAttrs.Any(a => string.Equals(a.EventName, eventName, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // Fall back to static EVENTS metadata (strings and/or EventListener objects).
            var eventsField = type.GetField("EVENTS", BindingFlags.Public | BindingFlags.Static);
            if (eventsField != null && eventsField.GetValue(null) is IEnumerable eventsEnumerable)
            {
                foreach (var evt in eventsEnumerable)
                {
                    if (evt is string evtName &&
                        string.Equals(evtName, eventName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }

                    if (evt is Events.EventListener evtListener &&
                        string.Equals(evtListener.EventName, eventName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            type = type.BaseType;
        }

        return false;
    }

    private static bool IsGeneratorFunction(MethodInfo? methodInfo)
    {
        if (methodInfo == null)
        {
            return false;
        }

        // Python parity: only treat actual generator/async-generator methods
        // (yield / async yield) as stoppable.
        var isIterator = methodInfo.GetCustomAttributes(typeof(IteratorStateMachineAttribute), inherit: true).Any();
        var isAsyncIterator = methodInfo.GetCustomAttributes(typeof(AsyncIteratorStateMachineAttribute), inherit: true).Any();
        return isIterator || isAsyncIterator;
    }

    public Dependency AttachSubmitEvents(Button submitBtn, Button stopBtn)
    {
        var blocksConfig = Core.Context.GetBlocksContext();
        if (blocksConfig == null)
        {
            throw new InvalidOperationException("Cannot attach submit events outside of a Blocks context");
        }

        if (this.Live)
        {
            if (this.InterfaceType == "OUTPUT_ONLY")
            {
                if (submitBtn == null)
                {
                    throw new RenderError("Submit button not rendered");
                }

                // For output-only interfaces, load on page load and generate on button click
                // Load event
                var loadTarget = new Events.EventListenerMethod(blocksConfig.RootBlock, "load");
                blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod> { loadTarget },
                    fn: this.Fn,
                    inputs: null,
                    outputs: this.OutputComponents
                );

                // Submit button click event
                var clickTarget = new Events.EventListenerMethod(submitBtn, "click");
                var (dep, depIndex) = blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod> { clickTarget },
                    fn: this.Fn,
                    inputs: null,
                    outputs: this.OutputComponents,
                    apiName: this.ApiName,
                    apiVisibility: this.ApiVisibility ?? "public",
                    apiDescription: this.ApiDescription,
                    preprocess: !this.ApiMode,
                    postprocess: !this.ApiMode,
                    batch: this.Batch,
                    maxBatchSize: this.MaxBatchSize,
                    validator: this.Validator
                );

                return new Dependency(submitBtn, dep.GetConfig(), depIndex, this.Fn);
            }
            else
            {
                // For live interfaces with inputs, respond to input changes
                var events = new List<Events.EventListenerMethod>();
                bool streamingEvent = false;

                foreach (var component in this.InputComponents)
                {
                    // Check if component supports streaming
                    var streamingProp = component.GetType().GetProperty("Streaming");
                    bool isStreaming = streamingProp != null && (bool)streamingProp.GetValue(component);

                    // Check if component has stream event in Events list
                    bool hasStreamEvent = ComponentHasEvent(component, "stream");
                    bool hasChangeEvent = ComponentHasEvent(component, "change");

                    if (hasStreamEvent && isStreaming)
                    {
                        events.Add(new Events.EventListenerMethod(component, "stream"));
                        streamingEvent = true;
                    }
                    else if (hasChangeEvent)
                    {
                        events.Add(new Events.EventListenerMethod(component, "change"));
                    }
                }

                var (dep, depIndex) = blocksConfig.SetEventTrigger(
                    targets: events,
                    fn: this.Fn,
                    inputs: this.InputComponents,
                    outputs: this.OutputComponents,
                    apiName: this.ApiName,
                    apiDescription: this.ApiDescription,
                    apiVisibility: this.ApiVisibility ?? "public",
                    preprocess: !this.ApiMode,
                    postprocess: !this.ApiMode,
                    showProgress: streamingEvent ? "hidden" : this.ShowProgress,
                    triggerMode: streamingEvent ? "multiple" : "always_last",
                    connection: streamingEvent ? "stream" : "sse",
                    timeLimit: this.TimeLimit != null ? (float?)this.TimeLimit.Value : null,
                    streamEvery: (float)this.StreamEvery,
                    eventSpecificArgs: streamingEvent ? new List<string> { "stream_every" } : new List<string>(),
                    validator: this.Validator
                );

                return new Dependency(null, dep.GetConfig(), depIndex, this.Fn);
            }
        }
        else
        {
            // Non-live mode
            if (submitBtn == null)
            {
                throw new RenderError("Submit button not rendered");
            }

            // Build triggers list: submit button click + component submit events
            var triggers = new List<Events.EventListenerMethod>
            {
                new Events.EventListenerMethod(submitBtn, "click")
            };

            foreach (var component in this.InputComponents)
            {
                // Check if component has submit event
                bool hasSubmitEvent = ComponentHasEvent(component, "submit");
                // Python parity: textbox supports submit (Enter key) even if event metadata is not fully populated.
                bool isTextbox = component is Textbox;
                if (hasSubmitEvent || isTextbox)
                {
                    triggers.Add(new Events.EventListenerMethod(component, "submit"));
                }

                // Warn about streaming in non-live mode
                var streamingProp = component.GetType().GetProperty("Streaming");
                if (streamingProp != null && (bool)streamingProp.GetValue(component))
                {
                }
            }

            if (stopBtn != null)
            {
                // With stop button: more complex flow
                // 1. Show stop button, hide submit button
                // 2. Run the main function
                // 3. Cleanup: hide stop button, show submit button

                // Step 1: Toggle buttons
                var (toggleDep, toggleIndex) = blocksConfig.SetEventTrigger(
                    targets: triggers,
                    fn: new Func<(object, object)>(() =>
                        (new Dictionary<string, object> { ["visible"] = false, ["__type__"] = "update" },
                         new Dictionary<string, object> { ["visible"] = true, ["__type__"] = "update" })),
                    inputs: null,
                    outputs: new object[] { submitBtn, stopBtn },
                    queue: false,
                    postprocess: false,
                    apiVisibility: "undocumented"
                );

                // Step 2: Run main function (chained with .then)
                var (predictDep, predictIndex) = blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod>(),
                    fn: this.Fn,
                    inputs: this.InputComponents,
                    outputs: this.OutputComponents,
                    apiName: this.ApiName,
                    apiVisibility: this.ApiVisibility ?? "public",
                    apiDescription: this.ApiDescription,
                    scrollToOutput: true,
                    preprocess: !this.ApiMode,
                    postprocess: !this.ApiMode,
                    batch: this.Batch,
                    maxBatchSize: this.MaxBatchSize,
                    concurrencyLimit: this.ConcurrencyLimit,
                    showProgress: this.ShowProgress,
                    validator: this.Validator,
                    triggerAfter: toggleIndex,
                    triggerOnlyOnSuccess: false,
                    triggerOnlyOnFailure: false
                );

                // Step 3: Cleanup
                var (cleanupDep, cleanupIndex) = blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod>(),
                    fn: new Func<(object, object)>(() =>
                        (new Dictionary<string, object> { ["visible"] = true, ["__type__"] = "update" },
                         new Dictionary<string, object> { ["visible"] = false, ["__type__"] = "update" })),
                    inputs: null,
                    outputs: new object[] { submitBtn, stopBtn },
                    queue: false,
                    postprocess: false,
                    apiVisibility: "undocumented",
                    triggerAfter: predictIndex,
                    triggerOnlyOnSuccess: false,
                    triggerOnlyOnFailure: false
                );

                // Stop button: first cleanup (show submit, hide stop)
                var stopTarget = new Events.EventListenerMethod(stopBtn, "click");
                blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod> { stopTarget },
                    fn: new Func<(object, object)>(() =>
                        (new Dictionary<string, object> { ["visible"] = true, ["__type__"] = "update" },
                         new Dictionary<string, object> { ["visible"] = false, ["__type__"] = "update" })),
                    inputs: null,
                    outputs: new object[] { submitBtn, stopBtn },
                    queue: false,
                    postprocess: false,
                    apiVisibility: "undocumented"
                );
                // Stop button: then cancel the predict event (separate cancel dependency)
                blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod> { new Events.EventListenerMethod(stopBtn, "click") },
                    fn: null,
                    inputs: null,
                    outputs: new object[] { },
                    cancels: new List<int> { predictIndex },
                    queue: false,
                    apiVisibility: "private",
                    isCancelFunction: true
                );

                return new Dependency(null, cleanupDep.GetConfig(), cleanupIndex, this.Fn);
            }
            else
            {
                // Without stop button: simple flow
                // Python parity: Interface uses gr.on(triggers, ...) which always produces event_specific_args=[].
                var (dep, depIndex) = blocksConfig.SetEventTrigger(
                    targets: triggers,
                    fn: this.Fn,
                    inputs: this.InputComponents,
                    outputs: this.OutputComponents,
                    apiName: this.ApiName,
                    apiVisibility: this.ApiVisibility ?? "public",
                    apiDescription: this.ApiDescription,
                    scrollToOutput: true,
                    preprocess: !this.ApiMode,
                    postprocess: !this.ApiMode,
                    batch: this.Batch,
                    maxBatchSize: this.MaxBatchSize,
                    concurrencyLimit: this.ConcurrencyLimit,
                    showProgress: this.ShowProgress,
                    validator: this.Validator,
                    eventSpecificArgs: new List<string>()
                );

                return new Dependency(null, dep.GetConfig(), depIndex, this.Fn);
            }
        }
    }

    public void AttachClearEvents(Button clearBtn, Column inputComponentColumn)
    {
        if (clearBtn == null) return;

        var blocksConfig = Core.Context.GetBlocksContext();
        if (blocksConfig == null)
        {
            throw new InvalidOperationException("Cannot attach clear events outside of a Blocks context");
        }

        // Add all input and output components to clear button
        var componentsToAdd = new List<Component>(this.InputComponents);
        componentsToAdd.AddRange(this.OutputComponents);

        if (clearBtn is ClearButton clearButtonTemplate)
        {
            clearButtonTemplate.Add(componentsToAdd);
        }

        // Attach click event to reset column visibility/variant
        var jsCode = "";
        if (this.InterfaceType == "STANDARD" ||
            this.InterfaceType == "INPUT_ONLY" ||
            this.InterfaceType == "UNIFIED")
        {
            jsCode = "() => [{\"variant\": null, \"visible\": true, \"__type__\": \"update\"}]\n            ";
        }

        if (!string.IsNullOrEmpty(jsCode) && inputComponentColumn != null)
        {
            // Python parity: _clear_btn.click(None, [], [input_component_column], js=...)
            blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod> { new Events.EventListenerMethod(clearBtn, "click") },
                fn: null,
                inputs: Array.Empty<object>(),
                outputs: inputComponentColumn,
                js: jsCode,
                queue: false,
                preprocess: false,
                postprocess: false,
                apiVisibility: "undocumented"
            );
        }
    }

    public void AttachFlaggingEvents(List<Button> flagBtns, Button clearBtn, Dependency submitEvent)
    {
        this.FlaggingCallback ??= new CSVLogger();

        if ((flagBtns == null || flagBtns.Count == 0) && this.FlaggingMode == "manual" && this.FlaggingOptions != null)
        {
            var labels = new HashSet<string>(
                this.FlaggingOptions
                    .Where(o => o != null && !string.IsNullOrWhiteSpace(o.Item1))
                    .Select(o => o.Item1)
            );

            flagBtns = this.BlocksDict.Values
                .OfType<Button>()
                .Where(b => b != null && !string.IsNullOrWhiteSpace(b.Value) && labels.Contains(b.Value))
                .ToList();
        }

        if (!(flagBtns != null &&
              (this.InterfaceType == "STANDARD" ||
               this.InterfaceType == "OUTPUT_ONLY" ||
               this.InterfaceType == "UNIFIED")))
        {
            return;
        }

        // Auto flagging mode - flag on successful submit
        if (this.FlaggingMode == "auto")
        {
            if (submitEvent != null && this.FlaggingCallback != null)
            {
                var flagMethod = new FlagMethod(this.FlaggingCallback, "", null, visualFeedback: false);
                var flagComponents = new List<Component>(this.InputComponents);
                flagComponents.AddRange(this.OutputComponents);

                // Python parity: _submit_event.success(flag_method, inputs=..., outputs=None, preprocess=False, queue=False)
                submitEvent.Success(
                    fn: flagMethod.Invoke,
                    inputs: flagComponents,
                    outputs: null,
                    queue: false,
                    apiVisibility: "undocumented",
                    preprocess: false,
                    postprocess: false
                );
            }
            return;
        }

        // Manual flagging mode - attach to each flag button
        if (this.FlaggingMode == "manual" && this.FlaggingOptions != null)
        {
            var blocksConfig = Core.Context.GetBlocksContext();
            if (blocksConfig == null)
            {
                throw new InvalidOperationException("Cannot attach flagging events outside of a Blocks context");
            }

            var flagComponents = this.InterfaceType == "UNIFIED"
                ? new List<Component>(this.InputComponents)
                : new List<Component>(this.InputComponents).Concat(this.OutputComponents).ToList();

            for (int i = 0; i < Math.Min(flagBtns.Count, this.FlaggingOptions.Count); i++)
            {
                var flagBtn = flagBtns[i];
                var (label, value) = this.FlaggingOptions[i];

                if (value != null && !(value is string))
                {
                    throw new ArgumentException($"Flagging option value must be a string, not {value}");
                }

                var flagMethod = new FlagMethod(this.FlaggingCallback, label, value as string);

                // 1) Visual feedback: "Saving..."
                var savingUpdate = new Func<object>(() =>
                    Core.Helpers.Update(
                        kwargs:
                        [
                            new KeyValuePair<string, object>("value", "Saving..."),
                            new KeyValuePair<string, object>("interactive", false)
                        ]
                    )
                );
                blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod> { new Events.EventListenerMethod(flagBtn, "click") },
                    fn: savingUpdate,
                    inputs: null,
                    outputs: flagBtn,
                    queue: false,
                    preprocess: false,
                    postprocess: false,
                    apiName: "lambda",
                    apiVisibility: "undocumented"
                );

                // 2) Execute flag method (no queue)
                blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod> { new Events.EventListenerMethod(flagBtn, "click") },
                    fn: flagMethod.Invoke,
                    inputs: flagComponents,
                    outputs: flagBtn,
                    queue: false,
                    preprocess: false,
                    postprocess: false,
                    apiName: "Flag",
                    componentPropInputs: new List<int>(),
                    apiVisibility: "undocumented"
                );

                // 3) Reset button on clear
                if (clearBtn != null)
                {
                    var resetUpdate = new Func<object>(() =>
                        Core.Helpers.Update(
                            kwargs:
                            [
                                new KeyValuePair<string, object>("value", label),
                                new KeyValuePair<string, object>("interactive", true)
                            ]
                        )
                    );
                    blocksConfig.SetEventTrigger(
                        targets: new List<Events.EventListenerMethod> { new Events.EventListenerMethod(clearBtn, "click") },
                        fn: resetUpdate,
                        inputs: null,
                        outputs: flagBtn,
                        queue: false,
                        preprocess: false,
                        postprocess: false,
                        apiName: "reset",
                        apiVisibility: "undocumented"
                    );
                }
            }
        }
    }

    public void RenderExamples()
    {
        if (this.Examples != null)
        {
            static bool IsStateComponent(Component c)
            {
                // Python: isinstance(c, State)
                // .NET port: best-effort based on type-name.
                return c.GetType().Name.Contains("state", StringComparison.OrdinalIgnoreCase);
            }

            var nonStateInputs = this.InputComponents
                .Where(c => c != null && !IsStateComponent(c))
                .ToList();
            var nonStateOutputs = this.OutputComponents
                .Where(c => c != null && !IsStateComponent(c))
                .ToList();

            var handler = new Core.Examples(
                examples: this.Examples,
                inputs: nonStateInputs,
                outputs: nonStateOutputs,
                fn: this.Fn,
                cacheExamples: this.CacheExamples,
                cacheMode: this.CacheMode,
                examplesPerPage: this.ExamplesPerPage,
                apiMode: this.ApiMode,
                batch: this.Batch,
                exampleLabels: this.ExampleLabels,
                preload: this.PreloadExample,
                initiatedDirectly: false);

            // Python: examples are constructed inside a Blocks context.
            // C#: call Create() to attach events / caching startup hooks.
            handler.Create();
            this.ExamplesHandler = handler;
        }
    }

    public override string ToString()
    {
        return this.Repr();
    }

    public string Repr()
    {
        var repr = new StringBuilder();
        repr.AppendLine($"Gradio Interface for: {this.Name}");
        repr.AppendLine(new string('-', repr.Length - 1));
        repr.AppendLine("inputs:");
        foreach (var component in this.InputComponents)
        {
            repr.AppendLine($"|-{component}");
        }
        repr.AppendLine("outputs:");
        foreach (var component in this.OutputComponents)
        {
            repr.AppendLine($"|-{component}");
        }
        return repr.ToString();
    }
}
