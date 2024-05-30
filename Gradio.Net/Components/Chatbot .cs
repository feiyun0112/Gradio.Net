using Gradio.Net.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using Gradio.Net.Models;
using System.IO;

namespace Gradio.Net
{
    public class Chatbot : Component
    {
        internal int? Height { get;  set; }
        internal IEnumerable<Dictionary<string, object>> LatexDelimiters { get;  set; }
        internal bool ShowShareButton { get;  set; }
        internal bool RenderMarkdown { get;  set; }
        internal bool ShowCopyButton { get;  set; }
        internal Tuple<string, string> AvatarImages { get;  set; }
        internal bool SanitizeHtml { get;  set; }
        internal bool BubbleFullWidth { get;  set; }
        internal bool LineBreaks { get;  set; }
        internal bool Likeable { get;  set; }
        internal string Layout { get;  set; }
        internal string Placeholder { get;  set; }

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
                var fileMessage = JsonUtils.Deserialize<FileMessage>(chatMessage);
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
            List<ChatbotMessagePair> processedMessages = new List<ChatbotMessagePair>();
            if (data == null)
            {
                return processedMessages;
            }
            var str = data.ToString();

            var listMessagePair=  JsonUtils.Deserialize<List<string[]>>(str);

            
            foreach  (var messagePair in listMessagePair)
            {
                if (messagePair.Length != 2)
                {
                    throw new InvalidDataException(
                        $"Expected a list of lists of length 2 or list of tuples of length 2. Received: {messagePair}"
                    );
                }
                processedMessages.Add(new ChatbotMessagePair ( PreprocessChatMessages(messagePair[0]) , PreprocessChatMessages(messagePair[1]))) ;
            }
            return processedMessages;
        }

        private object PostprocessChatMessages(ChatMessage chatMessage)
        {
            if (chatMessage == null)
            {
                return null;
            }
            else if (!string.IsNullOrEmpty( chatMessage.FilePath))
            {
                var filePath = chatMessage.FilePath;

                var mimeType = ClientUtils.GetMimeType(filePath);
                return new FileMessage()
                {
                    File = new FileData() { Path = filePath, MimeType = mimeType },
                    AltText = chatMessage.AltText,
                };
            }
            else if(!string.IsNullOrEmpty(chatMessage.TextMessage))
            {
                return chatMessage.TextMessage;
            }
            else
            {
                throw new ArgumentException($"Invalid message for Chatbot component: {chatMessage}");
            }
        }

        internal override object PostProcess(string rootUrl, object data)
        {
            if (data == null) {
                return new List<object[]>() ;
            }

            var value = data as IList<ChatbotMessagePair>;

            var processedMessages =  new List<object[]>();
            foreach (var messagePair in value)
            {
                processedMessages.Add(
                    [
                        PostprocessChatMessages(messagePair.HumanMessage),
                        PostprocessChatMessages(messagePair.AiMessage),
                    ]
                );
             }
            return processedMessages ;
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
}
