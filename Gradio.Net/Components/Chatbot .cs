using Gradio.Net.ChatBot;
using Gradio.Net.Enums;

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
    internal string Type { get; set; }

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
        { nameof(LatexDelimiters), JsonUtils.Deserialize<IEnumerable<Dictionary<string,object>>>("""
            [
                {
                    "left": "$$",
                    "right": "$$",
                    "display": true
                }
            ]
            """) },
        { nameof(Type), "messages" },
        { nameof(Container), true },
        { nameof(MinWidth), 160 },
        { nameof(Height), 400 },
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
            ChatBot.FileMessage fileMessage = JsonUtils.Deserialize<ChatBot.FileMessage>(chatMessage);
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

        List<Message> listMessage = JsonUtils.Deserialize<List<Message>>(str);

        Message previousMessage = null;
        foreach (var message in listMessage)
        {
            if (previousMessage == null)
            {
                previousMessage = message;
            }
            else
            {
                if (previousMessage.Role == "user" && message.Role == "assistant")
                {
                    processedMessages.Add(new ChatbotMessagePair(PreprocessChatMessages(previousMessage.Content.ToString()), PreprocessChatMessages(message.Content.ToString())));
                    previousMessage = null;
                }
                else if (previousMessage.Role == "user")
                {
                    processedMessages.Add(new ChatbotMessagePair(PreprocessChatMessages(previousMessage.Content.ToString()), null));
                    previousMessage = message;
                }
                else
                {
                    processedMessages.Add(new ChatbotMessagePair(null, PreprocessChatMessages(previousMessage.Content.ToString())));
                    previousMessage = message;
                }
            }

        }

        if (previousMessage != null)
        {
            if (previousMessage.Role == "user")
            {
                processedMessages.Add(new ChatbotMessagePair(PreprocessChatMessages(previousMessage.Content.ToString()), null));
            }
            else
            {
                processedMessages.Add(new ChatbotMessagePair(null, PreprocessChatMessages(previousMessage.Content.ToString())));
            }
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
            return new ChatBot.FileMessage()
            {
                File = new FileData() { Path = filePath, MimeType = mimeType },
                AltText = chatMessage.AltText,
            };
        }
        else
        {
            return new ChatBot.Message()
            {
                Role = chatMessage.Role,
                Content = chatMessage.TextMessage,
                Metadata = new ChatBot.Metadata() { },
            };
        }
    }

    internal override object PostProcess(string rootUrl, object data)
    {
        if (data == null)
        {
            return new List<object[]>();
        }

        IList<ChatbotMessagePair>? value = data as IList<ChatbotMessagePair>;

        List<object> processedMessages = [];
        foreach (ChatbotMessagePair messagePair in value)
        {
            processedMessages.AddRange(
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
