using System.Reflection;
using Gradio.Net.Core;
using Gradio.Net.Core.Layouts;
using Gradio.Net.Components;
using Gradio.Net.I18n;

namespace Gradio.Net
{

    public class ChatInterface : Blocks
    {
        public bool Multimodal { get; set; }
        public Chatbot Chatbot { get; set; }
        public Component Textbox { get; set; }
        public List<Component> AdditionalInputs { get; set; }
        public List<Component> AdditionalOutputs { get; set; }
        public bool Editable { get; set; }
        public object Examples { get; set; }
        public List<ExampleMessage> ExamplesMessages { get; set; }
        public bool RunExamplesOnClick { get; set; }
        public bool? CacheExamples { get; set; }
        public string CacheMode { get; set; }
        public string FlaggingMode { get; set; }
        public object FlaggingOptions { get; set; }
        public string FlaggingDir { get; set; }
        public bool Autofocus { get; set; }
        public bool Autoscroll { get; set; }
        public object SubmitBtn { get; set; }
        public object StopBtn { get; set; }
        public object ConcurrencyLimit { get; set; }
        public string ShowProgress { get; set; }
        public string ApiName { get; set; }
        public object ApiDescription { get; set; }
        public string ApiVisibility { get; set; }
        public bool SaveHistory { get; set; }
        public object Validator { get; set; }

        private object _chatTitle;
        private string _chatDescription;
        private bool _isAsync;
        private bool _isGenerator;
        private bool _providedChatbot;
        private object _originalStopBtn;
        private Button _newChatButton;
        private Components.State _conversationId;
        private Components.State _savedInput;
        private Components.State _nullComponent;
        private Components.State _chatbotState;
        private Components.State _chatbotValue;
        private Button _fakeApiBtn;
        private JsonComponent _apiResponse;
        private Components.BrowserState _savedConversations;

        private object _fn;
        private object Fn
        {
            get => _fn;
            set
            {
                _fn = value;
                _isAsync = CheckIfAsyncFunction(value);
                _isGenerator = CheckIfGeneratorFunction(value);
            }
        }

        public ChatInterface(
            object fn,
            bool multimodal = false,
            Chatbot chatbot = null,
            Component textbox = null,
            object additionalInputs = null,
            object additionalOutputs = null,
            bool editable = false,
            object examples = null,
            object exampleLabels = null,
            object exampleIcons = null,
            bool runExamplesOnClick = true,
            bool? cacheExamples = null,
            string cacheMode = null,
            object title = null,
            string description = null,
            string flaggingMode = null,
            object flaggingOptions = null,
            string flaggingDir = ".gradio/flagged",
            bool? analyticsEnabled = null,
            bool autofocus = true,
            bool autoscroll = true,
            object submitBtn = null,
            object stopBtn = null,
            object concurrencyLimit = null,
            object deleteCache = null,
            string showProgress = "minimal",
            bool fillHeight = true,
            bool fillWidth = false,
            string apiName = null,
            object apiDescription = null,
            string apiVisibility = "public",
            bool saveHistory = false,
            object validator = null
        ) : base(
            analyticsEnabled: analyticsEnabled,
            mode: "chat_interface",
            title: title?.ToString() ?? "Gradio",
            fillHeight: fillHeight,
            fillWidth: fillWidth,
            deleteCache: deleteCache as Tuple<int, int>
        )
        {
            ApiName = apiName;
            ApiDescription = apiDescription;
            ApiVisibility = apiVisibility;
            Multimodal = multimodal;
            ConcurrencyLimit = concurrencyLimit;

            if (fn is ChatInterface chatInterface)
            {
                Fn = chatInterface._fn;
            }
            else
            {
                Fn = fn;
            }

            Validator = validator;
            _providedChatbot = chatbot != null;
            Examples = examples;
            ExamplesMessages = SetupExampleMessages(examples, exampleLabels, exampleIcons);
            RunExamplesOnClick = runExamplesOnClick;
            CacheExamples = cacheExamples;
            CacheMode = cacheMode;
            Editable = editable;
            Autoscroll = autoscroll;
            Autofocus = autofocus;
            _chatTitle = title;
            _chatDescription = description;
            ShowProgress = showProgress;
            SaveHistory = saveHistory;

            AdditionalInputs = Gradio.Net.Utils.Utils.NoneOrSingletonToList(additionalInputs)
                .Select(i => (Component)Gradio.Net.Utils.Utils.GetComponentInstance(i))
                .ToList();
            AdditionalOutputs = Gradio.Net.Utils.Utils.NoneOrSingletonToList(additionalOutputs)
                .Select(i => (Component)i)
                .ToList();

            FlaggingMode = flaggingMode ?? Environment.GetEnvironmentVariable("GRADIO_CHAT_FLAGGING_MODE") ?? "never";
            if (!new[] { "manual", "never" }.Contains(FlaggingMode))
            {
                throw new Exception("Invalid value for `flagging_mode` parameter. Must be: 'manual' or 'never'.");
            }

            FlaggingOptions = flaggingOptions ?? new List<string> { "Like", "Dislike" };
            FlaggingDir = flaggingDir;

            if (textbox is Textbox || textbox is MultimodalTextbox)
            {
                if (textbox is Core.Block tb && tb.IsRendered)
                    tb.Unrender();
            }
            if (chatbot is Chatbot chatbotBlock && chatbotBlock.IsRendered)
            {
                chatbotBlock.Unrender();
            }

            this.Enter();
            try
            {
                _savedConversations = new Components.BrowserState(new List<object>(), storageKey: $"_saved_conversations_{_id}");
                _conversationId = new Components.State(null);
                _savedInput = new Components.State();
                _nullComponent = new Components.State();

                var column = new Column();
                column.Enter();
                try
                {
                    RenderHeader();
                    if (SaveHistory)
                    {
                        var saveHistoryRow = new Row(scale: 1);
                        saveHistoryRow.Enter();
                        try
                        {
                            RenderHistoryArea();
                            var innerColumn = new Column(scale: 6);
                            innerColumn.Enter();
                            try
                            {
                                RenderChatbotArea(chatbot, textbox, submitBtn, stopBtn);
                                RenderFooter();
                            }
                            finally
                            {
                                innerColumn.Exit();
                            }
                        }
                        finally
                        {
                            saveHistoryRow.Exit();
                        }
                    }
                    else
                    {
                        RenderChatbotArea(chatbot, textbox, submitBtn, stopBtn);
                        RenderFooter();
                    }
                }
                finally
                {
                    column.Exit();
                }

                SetupEvents();
            }
            finally
            {
                this.Exit();
            }

            if (BlockContext.GetRenderContext() != null)
            {
                this.Render();
            }
        }



        private void RenderChatbotArea(
            Chatbot chatbot,
            Component textbox,
            object submitBtn,
            object stopBtn
        )
        {
            SubmitBtn = submitBtn ?? true;
            StopBtn = stopBtn ?? false;

            if (chatbot != null)
            {
                Chatbot = (Chatbot)Gradio.Net.Utils.Utils.GetComponentInstance(chatbot, render: true);
                if (Chatbot.Examples != null && ExamplesMessages != null && ExamplesMessages.Count > 0)
                {
                }
                Chatbot.Examples = ExamplesMessages;
            }
            else
            {
                Chatbot = new Chatbot(
                    label: new I18nData("chat_interface.chatbot").ToString(),
                    scale: 1,
                    height: FillHeight ? 400 : null,
                    autoscroll: Autoscroll,
                    examples: ExamplesMessages
                );
            }

            var group = new Group();
            group.Enter();
            try
            {
                var row = new Row();
                row.Enter();
                try
                {
                    if (textbox != null)
                    {
                        if (textbox is Textbox tb)
                        {
                            tb.ShowLabel = false;
                            Textbox = (Component)Gradio.Net.Utils.Utils.GetComponentInstance(tb, render: true);
                        }
                        else if (textbox is MultimodalTextbox mtb)
                        {
                            mtb.ShowLabel = false;
                            Textbox = (Component)Gradio.Net.Utils.Utils.GetComponentInstance(mtb, render: true);
                        }
                        else
                        {
                            throw new Exception($"Expected a Textbox or MultimodalTextbox component, but got {textbox.GetType().Name}");
                        }
                    }
                    else
                    {
                        if (Multimodal)
                        {
                            Textbox = new MultimodalTextbox(
                                showLabel: false,
                                label: "",
                                placeholder: new I18nData("chat_interface.message_placeholder").ToString(),
                                scale: 7,
                                autofocus: Autofocus,
                                submitBtn: SubmitBtn,
                                stopBtn: StopBtn
                            );
                        }
                        else
                        {
                            Textbox = new Textbox(
                                showLabel: false,
                                label: "",
                                placeholder: new I18nData("chat_interface.message_placeholder").ToString(),
                                scale: 7,
                                autofocus: Autofocus,
                                submitBtn: SubmitBtn,
                                stopBtn: StopBtn
                            );
                        }
                    }
                }
                finally
                {
                    row.Exit();
                }
            }
            finally
            {
                group.Exit();
            }

            _originalStopBtn = stopBtn;
            if (Textbox is Textbox tb1) tb1.StopBtn = false;
            if (Textbox is MultimodalTextbox mtb1) mtb1.StopBtn = false;

            _fakeApiBtn = new Button("Fake API", visible: false);
            _apiResponse = new JsonComponent(label: "Response", visible: false, minWidth: 160);

            _chatbotState = new Components.State(Chatbot.Value ?? new List<object>());
            _chatbotValue = new Components.State(Chatbot.Value ?? new List<object>());
        }

        private void RenderFooter()
        {
            if (Examples != null && ExamplesMessages != null && ExamplesMessages.Count > 0)
            {
                var samples = ExamplesMessages
                    .Select(msg => new List<object> { (object)msg.Text })
                    .ToList();

                var examplesDataset = new Components.Dataset(
                    components: new List<Component> { Textbox },
                    samples: samples,
                    type: "tuple",
                    visible: false
                );
            }

            var anyUnrenderedInputs = AdditionalInputs.Any(input => !input.IsRendered);
            if (AdditionalInputs.Count > 0 && anyUnrenderedInputs)
            {
                foreach (var inputComponent in AdditionalInputs)
                {
                    if (!inputComponent.IsRendered)
                    {
                        inputComponent.Render();
                    }
                }
            }
        }

        private List<ExampleMessage> SetupExampleMessages(
            object examples,
            object exampleLabels,
            object exampleIcons
        )
        {
            var examplesMessages = new List<ExampleMessage>();
            if (examples != null)
            {
                var examplesList = Gradio.Net.Utils.Utils.NoneOrSingletonToList(examples);
                var labelsList = Gradio.Net.Utils.Utils.NoneOrSingletonToList(exampleLabels);
                var iconsList = Gradio.Net.Utils.Utils.NoneOrSingletonToList(exampleIcons);

                for (int i = 0; i < examplesList.Count; i++)
                {
                    var example = examplesList[i];
                    if (example is List<object> exampleList && exampleList.Count > 0)
                    {
                        example = exampleList[0];
                    }

                    var exampleMessage = new ExampleMessage();
                    if (example is string strExample)
                    {
                        exampleMessage.Text = strExample;
                    }
                    else if (example is Dictionary<string, object> dictExample)
                    {
                        exampleMessage.Text = dictExample.ContainsKey("text") ? dictExample["text"] as string : "";
                        exampleMessage.Files = dictExample.ContainsKey("files") ? dictExample["files"] as List<object> : new List<object>();
                    }

                    if (labelsList.Count > i)
                    {
                        exampleMessage.DisplayText = labelsList[i] as string;
                    }

                    if (Multimodal)
                    {
                        if (exampleMessage.Files == null || exampleMessage.Files.Count == 0)
                        {
                            if (iconsList.Count > i)
                            {
                                exampleMessage.Icon = iconsList[i];
                            }
                            else
                            {
                                exampleMessage.Icon = new Dictionary<string, object>
                                {
                                    { "path", "" },
                                    { "url", null },
                                    { "orig_name", null },
                                    { "mime_type", "text" },
                                    { "meta", new Dictionary<string, object> { { "_type", "gradio.FileData" } } }
                                };
                            }
                        }
                    }
                    else if (iconsList.Count > i)
                    {
                        exampleMessage.Icon = iconsList[i];
                    }

                    examplesMessages.Add(exampleMessage);
                }
            }
            return examplesMessages;
        }

        private void SetupEvents()
        {
            var blocksConfig = Core.Context.GetBlocksContext();
            if (blocksConfig == null)
            {
                throw new InvalidOperationException("Cannot setup events outside of a Blocks context");
            }

            Chatbot._retryable = true;
            Chatbot._undoable = true;

            Delegate submitFn = _isGenerator
                ? (Delegate)(Func<object, object, IAsyncEnumerable<object>>)((msg, hist) => StreamFn(msg, hist))
                : (Delegate)(Func<object, object, Task<object>>)((msg, hist) => SubmitFn(msg, hist));

            var userSubmitTarget = new Events.EventListenerMethod(Textbox, "submit");
            var (_, clearAndSaveIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod> { userSubmitTarget },
                fn: new Func<string, (object, string)>(ClearAndSaveTextbox),
                inputs: new object[] { Textbox },
                outputs: new object[] { Textbox, _savedInput },
                apiVisibility: "undocumented",
                queue: Validator != null,
                validator: Validator as Delegate
            );

            var (_, appendIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object, object, object>((msg, hist) => AppendMessageToHistory(msg, hist, "user")),
                inputs: new object[] { _savedInput, Chatbot },
                outputs: new object[] { Chatbot },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: clearAndSaveIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var (_, submitIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: submitFn,
                inputs: new object[] { _savedInput, _chatbotState }.Concat(AdditionalInputs).ToArray(),
                outputs: new object[] { _nullComponent, Chatbot }.Concat(AdditionalOutputs).ToArray(),
                apiVisibility: "undocumented",
                concurrencyLimit: ConcurrencyLimit,
                showProgress: ShowProgress,
                triggerAfter: appendIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var (_, syncIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object, (object, object)>(x => (x, x)),
                inputs: new object[] { Chatbot },
                outputs: new object[] { _chatbotState, _chatbotValue },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: submitIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var (_, clearTextboxIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object>(() => new Dictionary<string, object> { { "__type__", "update" }, { "value", null }, { "interactive", true } }),
                inputs: null,
                outputs: new object[] { Textbox },
                apiVisibility: "undocumented",
                triggerAfter: syncIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object, object, object, (object, object)>(PersistConversation),
                inputs: new object[] { _conversationId, _chatbotState, _savedConversations },
                outputs: new object[] { _conversationId, _savedConversations },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: clearTextboxIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var fakeApiTarget = new Events.EventListenerMethod(_fakeApiBtn, "click");
            blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod> { fakeApiTarget },
                fn: submitFn,
                inputs: new object[] { Textbox, _chatbotState }.Concat(AdditionalInputs).ToArray(),
                outputs: new object[] { _apiResponse, _chatbotState }.Concat(AdditionalOutputs).ToArray(),
                apiName: ApiName,
                apiVisibility: ApiVisibility,
                concurrencyLimit: ConcurrencyLimit,
                postprocess: false
            );

            int exampleSubmitIdx = -1;
            if (Examples != null && ExamplesMessages != null && ExamplesMessages.Count > 0 && (CacheExamples == true || RunExamplesOnClick))
            {
                var exampleSelectTarget = new Events.EventListenerMethod(Chatbot, "example_select");
                var (_, exampleClickedIdx) = blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod> { exampleSelectTarget },
                    fn: new Func<Events.SelectData, (object, object)>(ExampleClicked),
                    inputs: null,
                    outputs: new object[] { Chatbot, _savedInput },
                    apiVisibility: "undocumented"
                );

                var (_, exSubmitIdx) = blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod>(),
                    fn: submitFn,
                    inputs: new object[] { _savedInput, _chatbotState }.Concat(AdditionalInputs).ToArray(),
                    outputs: new object[] { _nullComponent, Chatbot }.Concat(AdditionalOutputs).ToArray(),
                    apiVisibility: "undocumented",
                    concurrencyLimit: ConcurrencyLimit,
                    showProgress: ShowProgress,
                    triggerAfter: exampleClickedIdx,
                    triggerOnlyOnSuccess: false,
                    triggerOnlyOnFailure: false
                );
                exampleSubmitIdx = exSubmitIdx;

                blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod>(),
                    fn: new Func<object, (object, object)>(x => (x, x)),
                    inputs: new object[] { Chatbot },
                    outputs: new object[] { _chatbotState, _chatbotValue },
                    apiVisibility: "undocumented",
                    queue: false,
                    triggerAfter: exSubmitIdx,
                    triggerOnlyOnSuccess: false,
                    triggerOnlyOnFailure: false
                );
            }

            var retryTarget = new Events.EventListenerMethod(Chatbot, "retry");
            var (_, popIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod> { retryTarget },
                fn: new Func<object, (object, object)>(PopLastUserMessage),
                inputs: new object[] { _chatbotState },
                outputs: new object[] { _chatbotState, _savedInput },
                apiVisibility: "undocumented",
                queue: false
            );

            var (_, retryAppendIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object, object, object>((msg, hist) => AppendMessageToHistory(msg, hist, "user")),
                inputs: new object[] { _savedInput, _chatbotState },
                outputs: new object[] { Chatbot },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: popIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var (_, retryDisableIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object>(() => new Dictionary<string, object> { { "__type__", "update" }, { "interactive", false }, { "placeholder", "" } }),
                inputs: null,
                outputs: new object[] { Textbox },
                apiVisibility: "undocumented",
                triggerAfter: retryAppendIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var (_, retrySubmitIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: submitFn,
                inputs: new object[] { _savedInput, _chatbotState }.Concat(AdditionalInputs).ToArray(),
                outputs: new object[] { _nullComponent, Chatbot }.Concat(AdditionalOutputs).ToArray(),
                apiVisibility: "undocumented",
                concurrencyLimit: ConcurrencyLimit,
                showProgress: ShowProgress,
                triggerAfter: retryDisableIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var (_, retrySyncIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object, (object, object)>(x => (x, x)),
                inputs: new object[] { Chatbot },
                outputs: new object[] { _chatbotState, _chatbotValue },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: retrySubmitIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var (_, retryEnableIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object>(() => new Dictionary<string, object> { { "__type__", "update" }, { "interactive", true } }),
                inputs: null,
                outputs: new object[] { Textbox },
                apiVisibility: "undocumented",
                triggerAfter: retrySyncIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object, object, object, (object, object)>(PersistConversation),
                inputs: new object[] { _conversationId, _chatbotState, _savedConversations },
                outputs: new object[] { _conversationId, _savedConversations },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: retryEnableIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var eventsToCancel = new List<int> { submitIdx, retrySubmitIdx };
            if (exampleSubmitIdx >= 0) eventsToCancel.Add(exampleSubmitIdx);
            SetupStopEvents(
                eventTriggers: new[] { retryTarget, new Events.EventListenerMethod(Chatbot, "example_select") },
                eventsToCancel: eventsToCancel,
                afterSuccessIndex: clearAndSaveIdx
            );

            var undoTarget = new Events.EventListenerMethod(Chatbot, "undo");
            var (_, undoIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod> { undoTarget },
                fn: new Func<object, (object, object)>(PopLastUserMessage),
                inputs: new object[] { Chatbot },
                outputs: new object[] { Chatbot, Textbox },
                apiVisibility: "undocumented",
                queue: false
            );

            var (_, undoSyncIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object, (object, object)>(x => (x, x)),
                inputs: new object[] { Chatbot },
                outputs: new object[] { _chatbotState, _chatbotValue },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: undoIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object, object, object, (object, object)>(PersistConversation),
                inputs: new object[] { _conversationId, _chatbotState, _savedConversations },
                outputs: new object[] { _conversationId, _savedConversations },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: undoSyncIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var optionSelectTarget = new Events.EventListenerMethod(Chatbot, "option_select");
            var (_, optionSelectIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod> { optionSelectTarget },
                fn: new Func<object, object, (object, object)>(OptionClicked),
                inputs: new object[] { Chatbot },
                outputs: new object[] { Chatbot, _savedInput },
                apiVisibility: "undocumented",
                collectsEventData: true
            );

            var (_, optionSubmitIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: submitFn,
                inputs: new object[] { _savedInput, _chatbotState }.Concat(AdditionalInputs).ToArray(),
                outputs: new object[] { _nullComponent, Chatbot }.Concat(AdditionalOutputs).ToArray(),
                apiVisibility: "undocumented",
                concurrencyLimit: ConcurrencyLimit,
                showProgress: ShowProgress,
                triggerAfter: optionSelectIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var (_, optionSyncIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object, (object, object)>(x => (x, x)),
                inputs: new object[] { Chatbot },
                outputs: new object[] { _chatbotState, _chatbotValue },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: optionSubmitIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object, object, object, (object, object)>(PersistConversation),
                inputs: new object[] { _conversationId, _chatbotState, _savedConversations },
                outputs: new object[] { _conversationId, _savedConversations },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: optionSyncIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var clearTarget = new Events.EventListenerMethod(Chatbot, "clear");
            var (_, clearSyncIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod> { clearTarget },
                fn: new Func<object, (object, object)>(x => (x, x)),
                inputs: new object[] { Chatbot },
                outputs: new object[] { _chatbotState, _chatbotValue },
                apiVisibility: "undocumented",
                queue: false
            );

            blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object, object, (object, object)>(RemoveConversation),
                inputs: new object[] { _conversationId, _savedConversations },
                outputs: new object[] { _conversationId, _savedConversations },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: clearSyncIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );

            var chatbotValueChangeTarget = new Events.EventListenerMethod(_chatbotValue, "change");
            var (_, chatbotValueChangeIdx) = blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod> { chatbotValueChangeTarget },
                fn: new Func<object, object>(x => x),
                inputs: new object[] { _chatbotValue },
                outputs: new object[] { Chatbot },
                apiVisibility: "undocumented",
                triggerMode: "always_last"
            );

            blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object, (object, object)>(x => (x, x)),
                inputs: new object[] { Chatbot },
                outputs: new object[] { _chatbotState, _chatbotValue },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: chatbotValueChangeIdx,
                triggerOnlyOnSuccess: false,
                triggerOnlyOnFailure: false
            );
        }

        private void SetupStopEvents(
            IEnumerable<Events.EventListenerMethod> eventTriggers,
            List<int> eventsToCancel,
            int afterSuccessIndex)
        {
            var blocksConfig = Core.Context.GetBlocksContext();
            if (blocksConfig == null) return;

            blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod>(),
                fn: new Func<object>(() => new Dictionary<string, object> { { "__type__", "update" }, { "submit_btn", false }, { "stop_btn", _originalStopBtn } }),
                inputs: null,
                outputs: new object[] { Textbox },
                apiVisibility: "undocumented",
                queue: false,
                triggerAfter: afterSuccessIndex,
                triggerOnlyOnSuccess: true,
                triggerOnlyOnFailure: false
            );

            foreach (var eventTrigger in eventTriggers ?? Enumerable.Empty<Events.EventListenerMethod>())
            {
                blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod> { eventTrigger },
                    fn: new Func<object>(() => new Dictionary<string, object> { { "__type__", "update" }, { "submit_btn", false }, { "stop_btn", _originalStopBtn } }),
                    inputs: null,
                    outputs: new object[] { Textbox },
                    apiVisibility: "undocumented",
                    queue: false
                );
            }

            foreach (var eventIndex in eventsToCancel)
            {
                blocksConfig.SetEventTrigger(
                    targets: new List<Events.EventListenerMethod>(),
                    fn: new Func<object>(() => new Dictionary<string, object> { { "__type__", "update" }, { "submit_btn", SubmitBtn }, { "stop_btn", false } }),
                    inputs: null,
                    outputs: new object[] { Textbox },
                    apiVisibility: "undocumented",
                    queue: false,
                    triggerAfter: eventIndex,
                    triggerOnlyOnSuccess: false,
                    triggerOnlyOnFailure: false
                );
            }

            var stopTarget = new Events.EventListenerMethod(Textbox, "stop");
            blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod> { stopTarget },
                fn: null,
                inputs: null,
                outputs: null,
                apiVisibility: "private"
            );
            blocksConfig.SetEventTrigger(
                targets: new List<Events.EventListenerMethod> { stopTarget },
                fn: null,
                inputs: null,
                outputs: null,
                cancels: eventsToCancel,
                apiVisibility: "private",
                isCancelFunction: true
            );
        }

        private static bool CheckIfAsyncFunction(object func)
        {
            if (func == null) return false;
            var methodInfo = func.GetType().GetMethod("Invoke");
            return methodInfo != null && methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }

        private static bool CheckIfGeneratorFunction(object func)
        {
            if (func == null) return false;
            var methodInfo = func.GetType().GetMethod("Invoke");
            if (methodInfo == null) return false;
            var returnType = methodInfo.ReturnType;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
                return true;

            if (returnType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)))
                return true;

            if (returnType != typeof(string) && returnType != typeof(byte[]))
            {
                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return true;
                if (returnType == typeof(System.Collections.IEnumerable))
                    return true;
                if (returnType.GetInterfaces().Any(i => i == typeof(System.Collections.IEnumerable) ||
                    (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))))
                    return true;
            }

            return false;
        }

        private void RenderHeader()
        {
            if (_chatTitle != null)
            {
                new Markdown($"<h1 style='text-align: center; margin-bottom: 1rem'>{_chatTitle}</h1>");
            }
            if (!string.IsNullOrEmpty(_chatDescription))
            {
                new Markdown(_chatDescription);
            }
        }

        private void RenderHistoryArea()
        {
            var column = new Column();
            column.Enter();
            try
            {
                _newChatButton = new Button(
                    "New Chat",
                    variant: "primary",
                    size: "md",
                    icon: Gradio.Net.Utils.Utils.GetIconPath("plus.svg")
                );
            }
            finally
            {
                column.Exit();
            }
        }

        private (object, object) PersistConversation(object conversationId, object chatbotState, object savedConversations)
        {
            if (!SaveHistory) return (conversationId, savedConversations);

            var conversation = chatbotState as List<Dictionary<string, object>> ?? new List<Dictionary<string, object>>();
            var conversations = savedConversations as List<List<Dictionary<string, object>>> ?? new List<List<Dictionary<string, object>>>();

            var serializedConversation = SerializeComponents(conversation);

            int? index = null;
            if (conversationId != null && int.TryParse(conversationId.ToString(), out int convId))
            {
                index = convId;
            }

            if (index.HasValue && index.Value < conversations.Count)
            {
                conversations[index.Value] = serializedConversation;
            }
            else
            {
                conversations.Insert(0, serializedConversation);
                index = 0;
            }

            return (index, conversations);
        }

        private (object, object) RemoveConversation(object conversationId, object savedConversations)
        {
            if (conversationId == null)
            {
                return (null, savedConversations);
            }

            var conversations = savedConversations as List<List<Dictionary<string, object>>> ?? new List<List<Dictionary<string, object>>>();

            if (int.TryParse(conversationId.ToString(), out int index) && index >= 0 && index < conversations.Count)
            {
                conversations.RemoveAt(index);
            }

            return (null, conversations);
        }

        private static List<Dictionary<string, object>> SerializeComponents(List<Dictionary<string, object>> conversation)
        {
            static object? Inner(object? obj)
            {
                if (obj == null)
                {
                    return null;
                }

                if (obj is Component component)
                {
                    return component.Value;
                }

                if (obj is IDictionary<string, object> dict)
                {
                    var result = new Dictionary<string, object>();
                    foreach (var kvp in dict)
                    {
                        result[kvp.Key] = Inner(kvp.Value) ?? null;
                    }
                    return result;
                }

                if (obj is System.Collections.IDictionary anyDict)
                {
                    var result = new Dictionary<string, object>();
                    foreach (System.Collections.DictionaryEntry entry in anyDict)
                    {
                        var key = entry.Key?.ToString() ?? string.Empty;
                        result[key] = Inner(entry.Value) ?? null;
                    }
                    return result;
                }

                if (obj is System.Collections.IEnumerable list && obj is not string)
                {
                    var result = new List<object?>();
                    foreach (var item in list)
                    {
                        result.Add(Inner(item));
                    }
                    return result;
                }

                return obj;
            }

            var normalized = Inner(conversation);
            if (normalized is List<object?> listObj)
            {
                var result = new List<Dictionary<string, object>>();
                foreach (var item in listObj)
                {
                    if (item is Dictionary<string, object> d)
                    {
                        result.Add(d);
                    }
                    else if (item is IDictionary<string, object> id)
                    {
                        result.Add(new Dictionary<string, object>(id));
                    }
                }
                return result;
            }

            return conversation;
        }

        private async Task<object> SubmitFn(object message, object history, params object[] args)
        {
            if (_fn == null) return new { response = (object)null, history = history };

            object response = null;
            object historyList = history;

            try
            {
                var inputs = new List<object> { message, history };
                inputs.AddRange(args);

                if (_isAsync)
                {
                    var task = (Task)_fn.GetType().GetMethod("Invoke")?.Invoke(_fn, inputs.ToArray());
                    await task.ConfigureAwait(false);
                    var resultProperty = task.GetType().GetProperty("Result");
                    response = resultProperty?.GetValue(task);
                }
                else
                {
                    response = await Task.Run(() =>
                    {
                        return _fn.GetType().GetMethod("Invoke")?.Invoke(_fn, inputs.ToArray());
                    });
                }

                object[] additionalOutputs = null;
                if (AdditionalOutputs != null && AdditionalOutputs.Count > 0)
                {
                    if (response is object[] responseArray && responseArray.Length > 1)
                    {
                        response = responseArray[0];
                        additionalOutputs = responseArray.Skip(1).ToArray();
                    }
                }

                historyList = AppendMessageToHistory(message, historyList, "user");
                historyList = AppendMessageToHistory(response, historyList, "assistant");

                if (additionalOutputs != null && additionalOutputs.Length > 0)
                {
                    var result = new List<object> { response, historyList };
                    result.AddRange(additionalOutputs);
                    return result.ToArray();
                }

                return (response, historyList);
            }
            catch (Exception)
            {
                return ((object)null, history);
            }
        }

        private async IAsyncEnumerable<object> StreamFn(object message, object history, params object[] args)
        {
            if (_fn == null)
            {
                yield return ((object)null, history);
                yield break;
            }

            object generatorResult = null;
            var inputs = new List<object> { message, history };
            inputs.AddRange(args);

            try
            {
                if (_isAsync)
                {
                    var task = (Task)_fn.GetType().GetMethod("Invoke")?.Invoke(_fn, inputs.ToArray());
                    await task.ConfigureAwait(false);
                    var resultProperty = task.GetType().GetProperty("Result");
                    generatorResult = resultProperty?.GetValue(task);
                }
                else
                {
                    generatorResult = await Task.Run(() =>
                    {
                        return _fn.GetType().GetMethod("Invoke")?.Invoke(_fn, inputs.ToArray());
                    });
                }
            }
            catch (Exception)
            {
                generatorResult = null;
            }

            if (generatorResult == null)
            {
                yield return ((object)null, history);
                yield break;
            }

            var historyList = AppendMessageToHistory(message, history, "user");
            object[] additionalOutputs = null;

            if (generatorResult is System.Collections.IEnumerable enumerable)
            {
                foreach (var response in enumerable)
                {
                    object currentResponse = response;
                    if (AdditionalOutputs != null && AdditionalOutputs.Count > 0)
                    {
                        if (response is object[] responseArray && responseArray.Length > 1)
                        {
                            currentResponse = responseArray[0];
                            additionalOutputs = responseArray.Skip(1).ToArray();
                        }
                    }

                    var updatedHistory = AppendMessageToHistory(currentResponse, historyList, "assistant");
                    if (additionalOutputs != null && additionalOutputs.Length > 0)
                    {
                        var result = new List<object> { currentResponse, updatedHistory };
                        result.AddRange(additionalOutputs);
                        yield return result.ToArray();
                    }
                    else
                    {
                        yield return (currentResponse, updatedHistory);
                    }

                    await Task.Delay(10);
                }
            }
            else if (generatorResult is IAsyncEnumerable<object> asyncEnumerable)
            {
                await foreach (var response in asyncEnumerable)
                {
                    object currentResponse = response;
                    if (AdditionalOutputs != null && AdditionalOutputs.Count > 0)
                    {
                        if (response is object[] responseArray && responseArray.Length > 1)
                        {
                            currentResponse = responseArray[0];
                            additionalOutputs = responseArray.Skip(1).ToArray();
                        }
                    }

                    var updatedHistory = AppendMessageToHistory(currentResponse, historyList, "assistant");
                    if (additionalOutputs != null && additionalOutputs.Length > 0)
                    {
                        var result = new List<object> { currentResponse, updatedHistory };
                        result.AddRange(additionalOutputs);
                        yield return result.ToArray();
                    }
                    else
                    {
                        yield return (currentResponse, updatedHistory);
                    }
                }
            }
            else
            {
                var updatedHistory = AppendMessageToHistory(generatorResult, historyList, "assistant");
                yield return (generatorResult, updatedHistory);
            }
        }

        private (object, object) PopLastUserMessage(object history)
        {
            var historyList = new List<(string role, object rawItem)>();
            var isMessageList = history is List<Message>;

            if (history is List<Message> msgList)
            {
                foreach (var m in msgList)
                    historyList.Add((m.Role, m));
            }
            else if (history is System.Collections.IEnumerable enumHist && history is not string)
            {
                foreach (var item in enumHist)
                {
                    string role = null;
                    if (item is Message m2) role = m2.Role;
                    else if (item is Dictionary<string, object> d && d.ContainsKey("role")) role = d["role"]?.ToString();
                    historyList.Add((role, item));
                }
            }

            if (historyList.Count == 0)
            {
                return (history, Multimodal
                    ? (object)new Dictionary<string, object> { { "text", "" }, { "files", new List<object>() } }
                    : "");
            }

            int i = historyList.Count - 1;
            while (i >= 0 && historyList[i].role == "assistant") i--;
            while (i >= 0 && historyList[i].role == "user") i--;

            string lastUserMessage = "";
            var files = new List<object>();
            for (int j = i + 1; j < historyList.Count; j++)
            {
                var (role, rawItem) = historyList[j];
                if (role != "user") continue;

                if (rawItem is Message msg)
                {
                    foreach (var contentItem in msg.Content ?? new List<object>())
                    {
                        if (contentItem is TextMessage tm) lastUserMessage += tm.Text;
                        else if (contentItem is FileMessage fm) files.Add(fm.File);
                    }
                }
                else if (rawItem is Dictionary<string, object> d && d.ContainsKey("content"))
                {
                    var content = d["content"];
                    if (content is List<object> contentList)
                    {
                        foreach (var c in contentList)
                        {
                            if (c is Dictionary<string, object> cd)
                            {
                                if (cd.TryGetValue("type", out var t) && t?.ToString() == "text" && cd.ContainsKey("text"))
                                    lastUserMessage += cd["text"]?.ToString();
                                else if (cd.TryGetValue("type", out var t2) && t2?.ToString() == "file" && cd.ContainsKey("file"))
                                    files.Add(cd["file"]);
                            }
                            else if (c is TextMessage tm2) lastUserMessage += tm2.Text;
                            else if (c is string s) lastUserMessage += s;
                        }
                    }
                    else if (content is string cs) lastUserMessage += cs;
                }
            }

            object truncated;
            if (isMessageList)
                truncated = historyList.Take(i + 1).Select(x => x.rawItem).Cast<Message>().ToList();
            else
            {
                var truncatedDicts = new List<Dictionary<string, object>>();
                for (int j = 0; j <= i; j++)
                    if (historyList[j].rawItem is Dictionary<string, object> d2) truncatedDicts.Add(d2);
                truncated = truncatedDicts;
            }

            object returnMessage = Multimodal
                ? (object)new Dictionary<string, object> { { "text", lastUserMessage }, { "files", files } }
                : lastUserMessage;

            return (truncated, returnMessage);
        }

        private object AppendMessageToHistory(object message, object history, string role)
        {
            var messageDicts = MessageToDict(message, role);
            var historyList = new List<Dictionary<string, object>>();

            if (history is List<Dictionary<string, object>> histList)
            {
                foreach (var item in histList)
                {
                    historyList.Add(new Dictionary<string, object>(item));
                }
            }
            else if (history is System.Collections.IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (item is Dictionary<string, object> dict)
                    {
                        historyList.Add(new Dictionary<string, object>(dict));
                    }
                }
            }

            historyList.AddRange(messageDicts);

            return historyList;
        }

        private List<Dictionary<string, object>> MessageToDict(object message, string role)
        {
            var messageDicts = new List<Dictionary<string, object>>();

            var messageList = new List<object>();
            if (message is System.Collections.IList list)
            {
                foreach (var item in list)
                {
                    messageList.Add(item);
                }
            }
            else
            {
                messageList.Add(message);
            }

            foreach (var msg in messageList)
            {
                if (msg == null) continue;

                if (msg is Dictionary<string, object> dict)
                {
                    if (dict.ContainsKey("content"))
                    {
                        dict["role"] = role;
                        messageDicts.Add(new Dictionary<string, object>(dict));
                    }
                    else if (dict.ContainsKey("text") || dict.ContainsKey("files"))
                    {
                        var multimodalMessage = new Dictionary<string, object>
                        {
                            { "role", role },
                            { "content", new List<object>() }
                        };

                        var content = (List<object>)multimodalMessage["content"];

                        if (dict.ContainsKey("files") && dict["files"] is System.Collections.IEnumerable files)
                        {
                            foreach (var file in files)
                            {
                                var path = file;
                                if (file is Dictionary<string, object> fileDict && fileDict.ContainsKey("path"))
                                    path = fileDict["path"];
                                content.Add(new Dictionary<string, object> { { "path", path } });
                            }
                        }

                        if (dict.ContainsKey("text") && !string.IsNullOrEmpty(dict["text"]?.ToString()))
                            content.Add(dict["text"]);

                        messageDicts.Add(multimodalMessage);
                    }
                    else
                    {
                        messageDicts.Add(new Dictionary<string, object>
                        {
                            { "role", role },
                            { "content", msg }
                        });
                    }
                }
                else if (msg is string || msg.GetType().IsClass)
                {
                    messageDicts.Add(new Dictionary<string, object>
                    {
                        { "role", role },
                        { "content", msg }
                    });
                }
            }

            return messageDicts;
        }

        private (object, string) ClearAndSaveTextbox(object message)
        {
            var textboxUpdate = new Dictionary<string, object>
            {
                { "__type__", "update" },
                { "value", "" },
                { "interactive", false },
                { "placeholder", "" }
            };

            return (textboxUpdate, message?.ToString() ?? "");
        }

        private (object, object) OptionClicked(object history, object option)
        {
            var historyList = history as List<Dictionary<string, object>> ?? new List<Dictionary<string, object>>();

            object optionValue = null;
            if (option != null)
            {
                var optionType = option.GetType();
                var valueProp = optionType.GetProperty("value") ?? optionType.GetProperty("Value");
                if (valueProp != null)
                {
                    optionValue = valueProp.GetValue(option);
                }
            }

            historyList.Add(new Dictionary<string, object>
            {
                { "role", "user" },
                { "content", optionValue ?? "" }
            });

            return (historyList, optionValue);
        }

        private (object, object) ExampleClicked(Events.SelectData example)
        {
            if (example == null)
            {
                return (new List<Dictionary<string, object>>(), (object)null);
            }

            object exampleValue = example.Value;

            object message = exampleValue;
            if (Multimodal && exampleValue is Dictionary<string, object> multiDict)
            {
                if (multiDict.ContainsKey("files") && multiDict["files"] is System.Collections.IEnumerable files)
                {
                    var filePaths = new List<string>();
                    foreach (var file in files)
                    {
                        if (file is Dictionary<string, object> fileDict && fileDict.ContainsKey("path"))
                            filePaths.Add(fileDict["path"].ToString());
                    }
                    multiDict["files"] = filePaths;
                }
                message = multiDict;
            }
            else if (!Multimodal && exampleValue is Dictionary<string, object> textDict && textDict.ContainsKey("text"))
            {
                message = textDict["text"];
            }

            var history = AppendMessageToHistory(message, new List<Dictionary<string, object>>(), "user");
            return (history, message);
        }
    }
}
