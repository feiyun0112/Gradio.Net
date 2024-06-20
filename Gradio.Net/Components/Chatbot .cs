namespace Gradio.Net;

public class Chatbot : Component
{
    internal Chatbot() { }

    internal int? Height { get; set; }
    internal IEnumerable<Dictionary<string, object>> LatexDelimiters { get; set; }
    internal bool? ShowShareButton { get; set; }
    internal bool? RenderMarkdown { get; set; }
    internal bool? ShowCopyButton { get; set; }
    internal Tuple<string, string> AvatarImages { get; set; }
    internal bool? SanitizeHtml { get; set; }
    internal bool? BubbleFullWidth { get; set; }
    internal bool? LineBreaks { get; set; }
    internal bool? Likeable { get; set; }
    internal string Layout { get; set; }
    internal string Placeholder { get; set; }

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
        { nameof(Container), true },
        { nameof(MinWidth), 160 },
        { nameof(Visible), true},
        { nameof(Render), true },
         { nameof(Rtl), false },
         { nameof(ShowCopyButton), false },
          { nameof(SanitizeHtml), true },
           { nameof(RenderMarkdown), true },
           { nameof(BubbleFullWidth), true },
            { nameof(LineBreaks), true },
             { nameof(Likeable), false },
        {nameof(ShowShareButton),  GradioUtils.GetSpace() != null},
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;

    protected override Dictionary<string, object> GetApiInfo()
    {
        return new Dictionary<string, object>() { { "type", "string" } };
    }

    private ChatMessage PreprocessChatMessages(string chatMessage)
    {
        if (chatMessage == null)
        {
            return null;
        }
        else if (chatMessage.Contains("gradio.FileMessage"))
        {
            FileMessage fileMessage = JsonUtils.Deserialize<FileMessage>(chatMessage);
            if (!string.IsNullOrEmpty(fileMessage.AltText))
            {
                return new ChatMessage { FilePath = fileMessage.File.Path, AltText = fileMessage.AltText };
            }
            else
            {
                return new ChatMessage { FilePath = fileMessage.File.Path };
            }
        }


        return new ChatMessage { TextMessage = chatMessage };
    }

    internal override object PreProcess(object data)
    {
        List<ChatbotMessagePair> processedMessages = [];
        if (data == null)
        {
            return processedMessages;
        }
        string? str = data.ToString();

        List<string[]> listMessagePair = JsonUtils.Deserialize<List<string[]>>(str);


        foreach (string[] messagePair in listMessagePair)
        {
            if (messagePair.Length != 2)
            {
                throw new InvalidDataException(
                    $"Expected a list of lists of length 2 or list of tuples of length 2. Received: {messagePair}"
                );
            }
            processedMessages.Add(new ChatbotMessagePair(PreprocessChatMessages(messagePair[0]), PreprocessChatMessages(messagePair[1])));
        }
        return processedMessages;
    }

    private object PostprocessChatMessages(ChatMessage chatMessage)
    {
        if (chatMessage == null)
        {
            return null;
        }
        else if (!string.IsNullOrEmpty(chatMessage.FilePath))
        {
            string filePath = chatMessage.FilePath;

            string mimeType = ClientUtils.GetMimeType(filePath);
            return new FileMessage()
            {
                File = new FileData() { Path = filePath, MimeType = mimeType },
                AltText = chatMessage.AltText,
            };
        }
        else
        {
            return chatMessage.TextMessage;
        }
    }

    internal override object PostProcess(string rootUrl, object data)
    {
        if (data == null)
        {
            return new List<object[]>();
        }

        IList<ChatbotMessagePair>? value = data as IList<ChatbotMessagePair>;

        List<object[]> processedMessages = [];
        foreach (ChatbotMessagePair messagePair in value)
        {
            processedMessages.Add(
                [
                    PostprocessChatMessages(messagePair.HumanMessage),
                    PostprocessChatMessages(messagePair.AiMessage),
                ]
            );
        }
        return processedMessages;
    }

    public static IList<ChatbotMessagePair> Payload(object obj)
    {
        if (obj == null)
        {
            return null;
        }

        if (obj is IList<ChatbotMessagePair> list)
        {
            return list;
        }

        throw new ArgumentException($"Payload Type expect IList<ChatbotMessagePair> actual {obj.GetType()}");
    }
}
