using System.Reflection;
using System.Text.RegularExpressions;
using System.Text.Json;
using Gradio.Net.Data;

namespace Gradio.Net.Components
{
    public class Chatbot : Component
    {
        public bool _undoable { get; set; }
        public bool _retryable { get; set; }
        public bool likeable { get; set; }
        public bool Autoscroll { get; set; }
        public object Height { get; set; }
        public bool Resizable { get; set; }
        public object MaxHeight { get; set; }
        public object MinHeight { get; set; }
        public string Editable { get; set; }
        public bool Rtl { get; set; }
        public bool GroupConsecutiveMessages { get; set; }
        public List<Dictionary<string, object>> LatexDelimiters { get; set; }
        public List<string> Buttons { get; set; }
        public bool RenderMarkdown { get; set; }
        public string Watermark { get; set; }
        public bool SanitizeHtml { get; set; }
        public bool LineBreaks { get; set; }
        public string Layout { get; set; }
        public bool AllowFileDownloads { get; set; }
        public List<string> FeedbackOptions { get; set; }
        public List<string> FeedbackValue { get; set; }
        public object Placeholder { get; set; }
        public List<ExampleMessage> Examples { get; set; }
        public object AllowTags { get; set; }
        public List<Tuple<string, string>> ReasoningTags { get; set; }
        public bool LikeUserMessage { get; set; }
        public List<object> AvatarImages { get; set; }

        public Chatbot(
            object value = null,
            string label = null,
            object every = null,
            object inputs = null,
            bool? showLabel = null,
            bool container = true,
            int? scale = null,
            int minWidth = 160,
            object visible = null,
            string elemId = null,
            object elemClasses = null,
            bool autoscroll = true,
            bool render = true,
            object key = null,
            object preservedByKey = null,
            object height = null,
            bool resizable = false,
            object maxHeight = null,
            object minHeight = null,
            string editable = null,
            List<Dictionary<string, object>> latexDelimiters = null,
            bool rtl = false,
            List<string> buttons = null,
            string watermark = null,
            Tuple<object, object> avatarImages = null,
            bool sanitizeHtml = true,
            bool renderMarkdown = true,
            object feedbackOptions = null,
            object feedbackValue = null,
            bool lineBreaks = true,
            string layout = null,
            string placeholder = null,
            List<ExampleMessage> examples = null,
            bool allowFileDownloads = true,
            bool groupConsecutiveMessages = true,
            object allowTags = null,
            List<Tuple<string, string>> reasoningTags = null,
            bool likeUserMessage = false
        )
        {
            Value = value ?? new List<object>();
            Label = label;
            ShowLabel = showLabel ?? true;
            Container = container;
            Scale = scale;
            MinWidth = minWidth;
            ElemId = elemId;
            if (elemClasses is string ecs) ElemClasses = new List<string> { ecs };
            else if (elemClasses is List<string> ecl) ElemClasses = ecl;
            if (visible is bool vb) Visible = vb;
            _undoable = false;
            _retryable = false;
            likeable = false;

            Autoscroll = autoscroll;
            Height = height ?? 400;
            Resizable = resizable;
            MaxHeight = maxHeight;
            MinHeight = minHeight;
            Editable = editable;
            Rtl = rtl;
            GroupConsecutiveMessages = groupConsecutiveMessages;

            if (latexDelimiters == null)
            {
                LatexDelimiters = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "left", "$$" },
                        { "right", "$$" },
                        { "display", true }
                    }
                };
            }
            else
            {
                LatexDelimiters = latexDelimiters;
            }

            Buttons = buttons ?? new List<string> { "share", "copy", "copy_all" };
            RenderMarkdown = renderMarkdown;
            Watermark = watermark;
            SanitizeHtml = sanitizeHtml;
            LineBreaks = lineBreaks;
            Layout = layout;
            AllowFileDownloads = allowFileDownloads;

            if (feedbackOptions != null)
            {
                if (feedbackOptions is List<string> feedbackList)
                {
                    FeedbackOptions = feedbackList;
                }
                else if (feedbackOptions is Tuple<string, string> feedbackTuple)
                {
                    FeedbackOptions = new List<string> { feedbackTuple.Item1, feedbackTuple.Item2 };
                }
                else
                {
                    FeedbackOptions = new List<string> { "Like", "Dislike" };
                }
            }
            else
            {
                FeedbackOptions = new List<string> { "Like", "Dislike" };
            }

            if (feedbackValue != null && feedbackValue is List<string> feedbackValueList)
            {
                FeedbackValue = feedbackValueList;
            }

            Placeholder = placeholder;
            Examples = examples;
            AllowTags = allowTags ?? true;
            ReasoningTags = reasoningTags;
            LikeUserMessage = likeUserMessage;

            AvatarImages = new List<object> { null, null };
            if (avatarImages != null)
            {
                AvatarImages = new List<object>
                {
                    ServeStaticFile(avatarImages.Item1),
                    ServeStaticFile(avatarImages.Item2)
                };
            }

            SetupExamples();
        }

        private void SetupExamples()
        {
            if (Examples != null)
            {
                for (int i = 0; i < Examples.Count; i++)
                {
                    var example = Examples[i];
                    if (example.Icon is string iconPath)
                    {
                        example.Icon = ServeStaticFile(iconPath);
                    }

                    var fileInfo = example.Files;
                    if (fileInfo != null)
                    {
                        for (int j = 0; j < fileInfo.Count; j++)
                        {
                            if (fileInfo[j] is string filePath)
                            {
                                var origName = Path.GetFileName(filePath);
                                var fileData = ServeStaticFile(filePath);
                                if (fileData != null)
                                {
                                    fileData["orig_name"] = origName;
                                    fileData["mime_type"] = GetMimeType(origName);
                                    fileInfo[j] = fileData;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            var mimeTypes = new Dictionary<string, string>
            {
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".png", "image/png"},
                {".gif", "image/gif"},
                {".webp", "image/webp"},
                {".mp4", "video/mp4"},
                {".webm", "video/webm"},
                {".mp3", "audio/mpeg"},
                {".wav", "audio/wav"},
                {".pdf", "application/pdf"},
                {".txt", "text/plain"},
                {".json", "application/json"},
                {".html", "text/html"},
                {".css", "text/css"},
                {".js", "text/javascript"}
            };

            return mimeTypes.TryGetValue(extension, out var mimeType) ? mimeType : "application/octet-stream";
        }

        public override Dictionary<string, object> GetConfig(Type? cls = null)
        {
            var config = base.GetConfig(cls);

            if (Examples != null)
            {
                var examplesList = Examples.Select(e =>
                {
                    var dict = new Dictionary<string, object>();
                    if (e.Icon != null) dict["icon"] = e.Icon;
                    if (e.DisplayText != null) dict["display_text"] = e.DisplayText;
                    if (e.Text != null) dict["text"] = e.Text;
                    if (e.Files != null) dict["files"] = e.Files;
                    return (object)dict;
                }).ToList();
                config["examples"] = examplesList;
            }

            return config;
        }

        public static void CheckFormat(List<object> messages)
        {
            var allValid = messages.All(message =>
            {
                if (message is Dictionary<string, object> messageDict)
                {
                    return messageDict.ContainsKey("role") && messageDict.ContainsKey("content");
                }
                else if (message is System.Collections.IDictionary anyDict)
                {
                    bool hasRole = false;
                    bool hasContent = false;

                    foreach (var key in anyDict.Keys)
                    {
                        var k = key?.ToString();
                        if (string.Equals(k, "role", StringComparison.OrdinalIgnoreCase))
                        {
                            hasRole = true;
                        }
                        else if (string.Equals(k, "content", StringComparison.OrdinalIgnoreCase))
                        {
                            hasContent = true;
                        }
                    }

                    return hasRole && hasContent;
                }
                else if (message is ChatMessage || message is Message)
                {
                    return true;
                }
                return false;
            });

            if (!allValid)
            {
                throw new Exception("Data incompatible with messages format. Each message should be a dictionary with 'role' and 'content' keys or a ChatMessage object.");
            }
        }

        private object PreprocessContent(object chatMessage)
        {
            if (chatMessage is string text)
            {
                return new TextMessage { Text = text };
            }
            else if (chatMessage is Dictionary<string, object> dict)
            {
                if (dict.TryGetValue("type", out var typeObj))
                {
                    var type = typeObj?.ToString();
                    if (string.Equals(type, "text", StringComparison.OrdinalIgnoreCase))
                    {
                        dict.TryGetValue("text", out var textObj);
                        return new TextMessage { Text = textObj?.ToString() ?? string.Empty };
                    }

                    if (string.Equals(type, "file", StringComparison.OrdinalIgnoreCase) &&
                        dict.TryGetValue("file", out var fileObj) &&
                        fileObj is Dictionary<string, object> fileDict)
                    {
                        return new FileMessage
                        {
                            File = new FileData
                            {
                                Path = fileDict.TryGetValue("path", out var pathObj) ? pathObj?.ToString() : null,
                                Url = fileDict.TryGetValue("url", out var urlObj) ? urlObj?.ToString() : null,
                                OrigName = fileDict.TryGetValue("orig_name", out var origObj) ? origObj?.ToString() : null,
                                MimeType = fileDict.TryGetValue("mime_type", out var mimeObj) ? mimeObj?.ToString() : null
                            },
                            AltText = dict.TryGetValue("alt_text", out var altObj) ? altObj?.ToString() : null
                        };
                    }
                }

                if (dict.TryGetValue("text", out var rawText))
                {
                    return new TextMessage { Text = rawText?.ToString() ?? string.Empty };
                }
            }

            if (chatMessage is FileMessage fileMessage)
            {
                return fileMessage;
            }
            else if (chatMessage is TextMessage textMessage)
            {
                return textMessage;
            }
            else if (chatMessage is ComponentMessage componentMessage)
            {
                var capitalizedComponent = componentMessage.Component;
                if (capitalizedComponent.Equals("json", StringComparison.OrdinalIgnoreCase) ||
                    capitalizedComponent.Equals("html", StringComparison.OrdinalIgnoreCase))
                {
                    capitalizedComponent = capitalizedComponent.ToUpper();
                }
                else if (capitalizedComponent.Equals("model3d", StringComparison.OrdinalIgnoreCase))
                {
                    capitalizedComponent = "Model3D";
                }
                else
                {
                    capitalizedComponent = char.ToUpper(capitalizedComponent[0]) + capitalizedComponent.Substring(1);
                }

                var component = ImportComponentAndData(capitalizedComponent);
                if (component != null)
                {
                    // Create component instance if type was found
                    try
                    {
                        var instance = Activator.CreateInstance(component);
                        componentMessage.Value = instance;
                    }
                    catch
                    {
                        // Intentionally suppressed: keep original component name if instantiation fails
                    }
                }
                return componentMessage;
            }
            else
            {
                throw new Exception($"Invalid message for Chatbot component: {chatMessage}");
            }
        }

        public static Type ImportComponentAndData(string componentName)
        {
            try
            {
                // Use reflection to find component type in the Components namespace
                var componentType = Type.GetType($"Gradio.Net.Components.{componentName}");
                if (componentType != null)
                {
                    return componentType;
                }

                // Fallback: Search through all types in the assembly
                var assembly = typeof(Component).Assembly;
                componentType = assembly.GetTypes()
                    .FirstOrDefault(t => t.Name.Equals(componentName, StringComparison.OrdinalIgnoreCase)
                                      && t.IsSubclassOf(typeof(Component)));

                if (componentType != null)
                {
                    return componentType;
                }

                // Fallback list for common components
                var components = new List<Type>
                {
                    typeof(Textbox),
                    typeof(Button),
                    typeof(Checkbox),
                    typeof(Slider),
                    typeof(Markdown),
                    typeof(Chatbot)
                };

                return components.FirstOrDefault(c => c.Name.Equals(componentName, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return null;
            }
        }

        public override object Preprocess(object payload)
        {
            if (payload == null)
            {
                return new List<Dictionary<string, object?>>();
            }

            payload = NormalizePayload(payload);

            if (payload is System.Collections.IEnumerable enumerablePayload && payload is not string)
            {
                var rawMessages = enumerablePayload.Cast<object>().ToList();

                if (rawMessages.Count == 2 &&
                    rawMessages[0] is string &&
                    rawMessages[1] is System.Collections.IEnumerable historyEnumerable &&
                    rawMessages[1] is not string)
                {
                    rawMessages = historyEnumerable.Cast<object>().ToList();
                }

                if (rawMessages.Count == 0)
                {
                    return new List<Dictionary<string, object?>>();
                }

                var normalizedRawMessages = new List<object>();
                foreach (var item in rawMessages)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    if (item is System.Collections.IDictionary)
                    {
                        normalizedRawMessages.Add(item);
                        continue;
                    }

                    if (item is System.Collections.IList pair)
                    {
                        if (pair.Count > 0 && pair[0] != null)
                        {
                            normalizedRawMessages.Add(new Dictionary<string, object>
                            {
                                { "role", "user" },
                                { "content", pair[0] }
                            });
                        }

                        if (pair.Count > 1 && pair[1] != null)
                        {
                            normalizedRawMessages.Add(new Dictionary<string, object>
                            {
                                { "role", "assistant" },
                                { "content", pair[1] }
                            });
                        }

                        continue;
                    }

                    if (item is string text)
                    {
                        normalizedRawMessages.Add(new Dictionary<string, object>
                        {
                            { "role", "user" },
                            { "content", text }
                        });
                        continue;
                    }

                    normalizedRawMessages.Add(item);
                }

                rawMessages = normalizedRawMessages;

                CheckFormat(rawMessages);

                var messageDicts = new List<Dictionary<string, object?>>();
                foreach (var message in rawMessages)
                {
                    if (message is not Dictionary<string, object> messageDict)
                    {
                        continue;
                    }

                    var normalizedMessage = new Dictionary<string, object?>
                    {
                        { "role", messageDict.TryGetValue("role", out var roleObj) ? roleObj?.ToString() ?? "assistant" : "assistant" }
                    };

                    var contentItems = new List<object>();
                    if (messageDict.TryGetValue("content", out var contentObj) && contentObj is List<object> contentList)
                    {
                        foreach (var content in contentList)
                        {
                            contentItems.Add(PreprocessContent(content));
                        }
                    }
                    else if (messageDict.TryGetValue("content", out var singleContent) && singleContent != null)
                    {
                        contentItems.Add(PreprocessContent(singleContent));
                    }

                    normalizedMessage["content"] = contentItems;

                    if (messageDict.TryGetValue("metadata", out var metadataObj) && metadataObj != null)
                    {
                        normalizedMessage["metadata"] = metadataObj;
                    }

                    if (messageDict.TryGetValue("options", out var optionsObj) && optionsObj != null)
                    {
                        normalizedMessage["options"] = optionsObj;
                    }

                    messageDicts.Add(normalizedMessage);
                }

                return messageDicts;
            }

            if (payload is ChatbotDataMessages chatbotData)
            {
                var messageDicts = new List<Dictionary<string, object?>>();
                foreach (var message in chatbotData.Root)
                {
                    var messageDict = new Dictionary<string, object?>
                    {
                        { "role", message.Role },
                        { "content", message.Content.Select(PreprocessContent).ToList() }
                    };
                    if (message.Metadata != null)
                    {
                        messageDict["metadata"] = message.Metadata;
                    }
                    if (message.Options != null && message.Options.Count > 0)
                    {
                        messageDict["options"] = message.Options;
                    }
                    messageDicts.Add(messageDict);
                }
                return messageDicts;
            }
            else
            {
                throw new Exception("Data incompatible with the messages format");
            }
        }

        private static object? NormalizePayload(object? value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is JsonElement je)
            {
                return ConvertJsonElement(je);
            }

            if (value is System.Collections.IDictionary dict)
            {
                var normalized = new Dictionary<string, object>();
                foreach (System.Collections.DictionaryEntry entry in dict)
                {
                    var key = entry.Key?.ToString();
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        continue;
                    }

                    normalized[key] = NormalizePayload(entry.Value) ?? string.Empty;
                }

                return normalized;
            }

            if (value is System.Collections.IEnumerable enumerable && value is not string)
            {
                var list = new List<object>();
                foreach (var item in enumerable)
                {
                    list.Add(NormalizePayload(item) ?? string.Empty);
                }

                return list;
            }

            return value;
        }

        private static object? ConvertJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var prop in element.EnumerateObject())
                        {
                            dict[prop.Name] = ConvertJsonElement(prop.Value) ?? string.Empty;
                        }

                        return dict;
                    }
                case JsonValueKind.Array:
                    {
                        var list = new List<object>();
                        foreach (var item in element.EnumerateArray())
                        {
                            list.Add(ConvertJsonElement(item) ?? string.Empty);
                        }

                        return list;
                    }
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var i64)) return i64;
                    if (element.TryGetDouble(out var d)) return d;
                    return element.ToString();
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;
                default:
                    return element.ToString();
            }
        }

        private object PostprocessContent(object chatMessage)
        {
            if (chatMessage is string text)
            {
                return new TextMessage { Text = text.Trim() };
            }
            else if (chatMessage is TextMessage textMessage)
            {
                return textMessage;
            }
            else if (chatMessage is FileMessage || chatMessage is ComponentMessage)
            {
                return chatMessage;
            }
            else if (chatMessage is FileData fileData)
            {
                return new FileMessage { File = fileData };
            }
            else if (chatMessage is Component component)
            {
                // Create a component message with the component type and value
                return new Dictionary<string, object>
                {
                    { "component", component.GetType().Name },
                    { "value", component },
                    { "constructor_args", new Dictionary<string, object>() },
                    { "props", new Dictionary<string, object>() }
                };
            }
            else if (chatMessage is Dictionary<string, object> messageDict)
            {
                if (messageDict.ContainsKey("path"))
                {
                    var filepath = messageDict["path"] as string;
                    return CreateFileMessage(messageDict, filepath);
                }
                else if (messageDict.ContainsKey("file"))
                {
                    var fileDataDict = messageDict["file"] as Dictionary<string, object>;
                    var file = new FileData
                    {
                        Path = fileDataDict.ContainsKey("path") ? fileDataDict["path"] as string : null,
                        Url = fileDataDict.ContainsKey("url") ? fileDataDict["url"] as string : null,
                        Size = fileDataDict.ContainsKey("size") ? Convert.ToInt32(fileDataDict["size"]) : 0,
                        OrigName = fileDataDict.ContainsKey("orig_name") ? fileDataDict["orig_name"] as string : null,
                        MimeType = fileDataDict.ContainsKey("mime_type") ? fileDataDict["mime_type"] as string : null
                    };
                    return new FileMessage { File = file, AltText = messageDict.ContainsKey("alt_text") ? messageDict["alt_text"] as string : null };
                }
                else if (messageDict.ContainsKey("type") && messageDict["type"] as string == "text")
                {
                    return new TextMessage { Text = messageDict["text"] as string };
                }
                else if (messageDict.ContainsKey("type") && messageDict["type"] as string == "component")
                {
                    return new ComponentMessage
                    {
                        Component = messageDict["component"] as string,
                        Value = messageDict["value"],
                        ConstructorArgs = messageDict["constructor_args"] as Dictionary<string, object>,
                        Props = messageDict["props"] as Dictionary<string, object>
                    };
                }
                else if (messageDict.ContainsKey("type") && messageDict["type"] as string == "file")
                {
                    var fileDataDict = messageDict["file"] as Dictionary<string, object>;
                    var file = new FileData
                    {
                        Path = fileDataDict.ContainsKey("path") ? fileDataDict["path"] as string : null,
                        Url = fileDataDict.ContainsKey("url") ? fileDataDict["url"] as string : null,
                        Size = fileDataDict.ContainsKey("size") ? Convert.ToInt32(fileDataDict["size"]) : 0,
                        OrigName = fileDataDict.ContainsKey("orig_name") ? fileDataDict["orig_name"] as string : null,
                        MimeType = fileDataDict.ContainsKey("mime_type") ? fileDataDict["mime_type"] as string : null
                    };
                    return new FileMessage { File = file, AltText = messageDict.ContainsKey("alt_text") ? messageDict["alt_text"] as string : null };
                }
            }
            else if (chatMessage is System.Collections.IDictionary anyDict)
            {
                var dict = new Dictionary<string, object>();
                foreach (System.Collections.DictionaryEntry entry in anyDict)
                {
                    var key = entry.Key?.ToString();
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        dict[key] = entry.Value!;
                    }
                }

                return PostprocessContent(dict);
            }

            throw new Exception($"Invalid message for Chatbot component: {chatMessage}");
        }

        private FileMessage CreateFileMessage(object chatMessage, string filepath)
        {
            var mime_type = GetMimeType(filepath);
            var fileData = new FileData
            {
                Path = filepath,
                MimeType = mime_type
            };

            var altText = GetAltText(chatMessage);
            return new FileMessage { File = fileData, AltText = altText };
        }

        private string GetAltText(object chatMessage)
        {
            if (chatMessage is Dictionary<string, object> messageDict && messageDict.ContainsKey("alt_text"))
            {
                return messageDict["alt_text"] as string;
            }
            else if (chatMessage is Tuple<object, object> tuple && tuple.Item2 is string altText)
            {
                return altText;
            }
            return null;
        }

        private List<Message> PostprocessMessage(object message)
        {
            var messages = new List<Message>();
            string role = "assistant";
            MetadataDict metadata = null;
            List<OptionDict> options = null;
            List<object> contentPostprocessed = new List<object>();

            if (message is Dictionary<string, object> messageDict)
            {
                role = messageDict["role"] as string;
                metadata = messageDict.ContainsKey("metadata") ? messageDict["metadata"] as MetadataDict : null;
                options = messageDict.ContainsKey("options") ? messageDict["options"] as List<OptionDict> : null;

                if (messageDict["content"] is List<object> contentList)
                {
                    foreach (var contentItem in contentList)
                    {
                        var item = PostprocessContent(contentItem);
                        if (item != null)
                        {
                            contentPostprocessed.Add(item);
                        }
                    }
                }
                else
                {
                    var item = PostprocessContent(messageDict["content"]);
                    if (item != null)
                    {
                        contentPostprocessed.Add(item);
                    }
                }
            }
            else if (message is ChatMessage chatMessage)
            {
                role = chatMessage.Role;
                metadata = chatMessage.Metadata;
                options = chatMessage.Options;

                if (chatMessage.Content is List<object> contentList)
                {
                    foreach (var contentItem in contentList)
                    {
                        var item = PostprocessContent(contentItem);
                        if (item != null)
                        {
                            contentPostprocessed.Add(item);
                        }
                    }
                }
                else
                {
                    var item = PostprocessContent(chatMessage.Content);
                    if (item != null)
                    {
                        contentPostprocessed.Add(item);
                    }
                }
            }
            else if (message is Message messageObj)
            {
                return new List<Message> { messageObj };
            }

            if (contentPostprocessed.Count == 0)
            {
                return null;
            }

            if (ReasoningTags != null && ReasoningTags.Count > 0)
            {
                var nonTextContent = contentPostprocessed.Where(item => !(item is TextMessage)).ToList();

                foreach (var contentItem in contentPostprocessed)
                {
                    if (contentItem is TextMessage textMessage)
                    {
                        var segments = ExtractThinkingBlocks(textMessage.Text, ReasoningTags);
                        foreach (var segment in segments)
                        {
                            var segmentText = segment.Item1;
                            var isThinking = segment.Item2;
                            var status = segment.Item3;

                            if (isThinking)
                            {
                                var thinkingMessage = new Message
                                {
                                    Role = role,
                                    Content = new List<object> { new TextMessage { Text = segmentText } },
                                    Metadata = new MetadataDict { Title = "Reasoning", Status = status },
                                    Options = options
                                };
                                messages.Add(thinkingMessage);
                            }
                            else
                            {
                                var proseMessage = new Message
                                {
                                    Role = role,
                                    Content = new List<object> { new TextMessage { Text = segmentText } },
                                    Metadata = metadata,
                                    Options = options
                                };
                                messages.Add(proseMessage);
                            }
                        }
                    }
                }

                if (nonTextContent.Count > 0)
                {
                    var nonTextMessage = new Message
                    {
                        Role = role,
                        Content = nonTextContent,
                        Metadata = metadata,
                        Options = options
                    };
                    messages.Add(nonTextMessage);
                }
            }
            else
            {
                var newMessage = new Message
                {
                    Role = role,
                    Content = contentPostprocessed,
                    Metadata = metadata,
                    Options = options
                };
                messages.Add(newMessage);
            }

            return messages;
        }

        private List<Tuple<string, bool, string>> ExtractThinkingBlocks(string content, List<Tuple<string, string>> tags)
        {
            var segments = new List<Tuple<string, bool, string>>();
            if (string.IsNullOrEmpty(content))
            {
                return segments;
            }

            var patterns = new List<string>();
            foreach (var tag in tags)
            {
                var openTag = Regex.Escape(tag.Item1);
                var closeTag = Regex.Escape(tag.Item2);
                patterns.Add($"({openTag})(.*?)(?:{closeTag}|$)");
            }

            var combinedPattern = string.Join("|", patterns);
            var regex = new Regex(combinedPattern, RegexOptions.Singleline);

            int lastEnd = 0;
            foreach (Match match in regex.Matches(content))
            {
                if (match.Index > lastEnd)
                {
                    var prose = content.Substring(lastEnd, match.Index - lastEnd).Trim();
                    if (!string.IsNullOrEmpty(prose))
                    {
                        segments.Add(new Tuple<string, bool, string>(prose, false, "done"));
                    }
                }

                string thinking = null;
                for (int i = 1; i < match.Groups.Count; i += 2)
                {
                    if (match.Groups[i + 1].Value != null)
                    {
                        thinking = match.Groups[i + 1].Value.Trim();
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(thinking))
                {
                    var pending = !tags.Any(tag => match.Value.EndsWith(tag.Item2));
                    segments.Add(new Tuple<string, bool, string>(thinking, true, pending ? "pending" : "done"));
                }

                lastEnd = match.Index + match.Length;
            }

            if (lastEnd < content.Length)
            {
                var prose = content.Substring(lastEnd).Trim();
                if (!string.IsNullOrEmpty(prose))
                {
                    segments.Add(new Tuple<string, bool, string>(prose, false, "done"));
                }
            }

            return segments;
        }

        public override object Postprocess(object value)
        {
            if (value == null)
            {
                return new List<Message>();
            }

            value = NormalizePayload(value);

            if (value is System.Collections.IEnumerable enumerableValue && value is not string)
            {
                var messagesList = enumerableValue.Cast<object>().ToList();
                CheckFormat(messagesList);
                var processedMessages = new List<Message>();

                foreach (var message in messagesList)
                {
                    var processedMessage = PostprocessMessage(message);
                    if (processedMessage != null)
                    {
                        processedMessages.AddRange(processedMessage);
                    }
                }

                return processedMessages;
            }

            throw new Exception($"Invalid value for Chatbot component: {value}");
        }

        public override object ExamplePayload()
        {
            return new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "role", "user" },
                    { "content", new List<object> { new TextMessage { Text = "Hello!" } } }
                },
                new Dictionary<string, object>
                {
                    { "role", "assistant" },
                    { "content", new List<object> { new TextMessage { Text = "How can I help you?" } } }
                }
            };
        }

        public override object ExampleValue()
        {
            return new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "role", "user" },
                    { "content", new List<object> { new TextMessage { Text = "Hello!" } } }
                },
                new Dictionary<string, object>
                {
                    { "role", "assistant" },
                    { "content", new List<object> { new TextMessage { Text = "How can I help you?" } } }
                }
            };
        }

        public override Dictionary<string, object> ApiInfo()
        {
            return new Dictionary<string, object>
            {
                ["$defs"] = new Dictionary<string, object>
                {
                    ["ComponentMessage"] = new Dictionary<string, object>
                    {
                        ["properties"] = new Dictionary<string, object>
                        {
                            ["component"] = new Dictionary<string, object> { ["title"] = "Component", ["type"] = "string" },
                            ["value"] = new Dictionary<string, object> { ["title"] = "Value" },
                            ["constructor_args"] = new Dictionary<string, object> { ["additionalProperties"] = true, ["title"] = "Constructor Args", ["type"] = "object" },
                            ["props"] = new Dictionary<string, object> { ["additionalProperties"] = true, ["title"] = "Props", ["type"] = "object" },
                            ["type"] = new Dictionary<string, object> { ["const"] = "component", ["default"] = "component", ["title"] = "Type", ["type"] = "string" }
                        },
                        ["required"] = new List<object> { "component", "value", "constructor_args", "props" },
                        ["title"] = "ComponentMessage",
                        ["type"] = "object"
                    },
                    ["FileData"] = new Dictionary<string, object>
                    {
                        ["description"] = "The FileData class is a subclass of the GradioModel class that represents a file object within a Gradio interface. It is used to store file data and metadata when a file is uploaded.\n\nAttributes:\n    path: The server file path where the file is stored.\n    url: The normalized server URL pointing to the file.\n    size: The size of the file in bytes.\n    orig_name: The original filename before upload.\n    mime_type: The MIME type of the file.\n    is_stream: Indicates whether the file is a stream.\n    meta: Additional metadata used internally (should not be changed).",
                        ["properties"] = new Dictionary<string, object>
                        {
                            ["path"] = new Dictionary<string, object> { ["title"] = "Path", ["type"] = "string" },
                            ["url"] = new Dictionary<string, object> { ["anyOf"] = new List<object> { new Dictionary<string, object> { ["type"] = "string" }, new Dictionary<string, object> { ["type"] = "null" } }, ["default"] = (object?)null, ["title"] = "Url" },
                            ["size"] = new Dictionary<string, object> { ["anyOf"] = new List<object> { new Dictionary<string, object> { ["type"] = "integer" }, new Dictionary<string, object> { ["type"] = "null" } }, ["default"] = (object?)null, ["title"] = "Size" },
                            ["orig_name"] = new Dictionary<string, object> { ["anyOf"] = new List<object> { new Dictionary<string, object> { ["type"] = "string" }, new Dictionary<string, object> { ["type"] = "null" } }, ["default"] = (object?)null, ["title"] = "Orig Name" },
                            ["mime_type"] = new Dictionary<string, object> { ["anyOf"] = new List<object> { new Dictionary<string, object> { ["type"] = "string" }, new Dictionary<string, object> { ["type"] = "null" } }, ["default"] = (object?)null, ["title"] = "Mime Type" },
                            ["is_stream"] = new Dictionary<string, object> { ["default"] = false, ["title"] = "Is Stream", ["type"] = "boolean" },
                            ["meta"] = new Dictionary<string, object> { ["$ref"] = "#/$defs/FileDataMeta" }
                        },
                        ["required"] = new List<object> { "path" },
                        ["title"] = "FileData",
                        ["type"] = "object"
                    },
                    ["FileDataMeta"] = new Dictionary<string, object>
                    {
                        ["properties"] = new Dictionary<string, object>
                        {
                            ["_type"] = new Dictionary<string, object> { ["const"] = "gradio.FileData", ["title"] = "Type", ["type"] = "string" }
                        },
                        ["required"] = new List<object> { "_type" },
                        ["title"] = "FileDataMeta",
                        ["type"] = "object"
                    },
                    ["FileMessage"] = new Dictionary<string, object>
                    {
                        ["properties"] = new Dictionary<string, object>
                        {
                            ["file"] = new Dictionary<string, object> { ["$ref"] = "#/$defs/FileData" },
                            ["alt_text"] = new Dictionary<string, object> { ["anyOf"] = new List<object> { new Dictionary<string, object> { ["type"] = "string" }, new Dictionary<string, object> { ["type"] = "null" } }, ["default"] = (object?)null, ["title"] = "Alt Text" },
                            ["type"] = new Dictionary<string, object> { ["const"] = "file", ["default"] = "file", ["title"] = "Type", ["type"] = "string" }
                        },
                        ["required"] = new List<object> { "file" },
                        ["title"] = "FileMessage",
                        ["type"] = "object"
                    },
                    ["Message"] = new Dictionary<string, object>
                    {
                        ["properties"] = new Dictionary<string, object>
                        {
                            ["role"] = new Dictionary<string, object> { ["title"] = "Role", ["type"] = "string" },
                            ["metadata"] = new Dictionary<string, object>
                            {
                                ["anyOf"] = new List<object> { new Dictionary<string, object> { ["$ref"] = "#/$defs/MetadataDict" }, new Dictionary<string, object> { ["type"] = "null" } },
                                ["default"] = (object?)null
                            },
                            ["content"] = new Dictionary<string, object>
                            {
                                ["items"] = new Dictionary<string, object>
                                {
                                    ["anyOf"] = new List<object>
                                    {
                                        new Dictionary<string, object> { ["$ref"] = "#/$defs/TextMessage" },
                                        new Dictionary<string, object> { ["$ref"] = "#/$defs/FileMessage" },
                                        new Dictionary<string, object> { ["$ref"] = "#/$defs/ComponentMessage" }
                                    }
                                },
                                ["title"] = "Content",
                                ["type"] = "array"
                            },
                            ["options"] = new Dictionary<string, object>
                            {
                                ["anyOf"] = new List<object>
                                {
                                    new Dictionary<string, object> { ["items"] = new Dictionary<string, object> { ["$ref"] = "#/$defs/OptionDict" }, ["type"] = "array" },
                                    new Dictionary<string, object> { ["type"] = "null" }
                                },
                                ["default"] = (object?)null,
                                ["title"] = "Options"
                            }
                        },
                        ["required"] = new List<object> { "role", "content" },
                        ["title"] = "Message",
                        ["type"] = "object"
                    },
                    ["MetadataDict"] = new Dictionary<string, object>
                    {
                        ["description"] = "A typed dictionary to represent metadata for a message in the Chatbot component. An\ninstance of this dictionary is used for the `metadata` field in a ChatMessage when\nthe chat message should be displayed as a thought.\nParameters:\n    title: The title of the \"thought\" message. Required if the message is to be displayed as a thought.\n    id: The ID of the message. Only used for nested thoughts. Nested thoughts can be nested by setting the parent_id to the id of the parent thought.\n    parent_id: The ID of the parent message. Only used for nested thoughts.\n    log: A string message to display next to the thought title in a subdued font.\n    duration: The duration of the message in seconds. Appears next to the thought title in a subdued font inside a parentheses.\n    status: if set to `\"pending\"`, a spinner appears next to the thought title and the accordion is initialized open.  If `status` is `\"done\"`, the thought accordion is initialized closed. If `status` is not provided, the thought accordion is initialized open and no spinner is displayed.",
                        ["properties"] = new Dictionary<string, object>
                        {
                            ["title"] = new Dictionary<string, object> { ["title"] = "Title", ["type"] = "string" },
                            ["id"] = new Dictionary<string, object> { ["anyOf"] = new List<object> { new Dictionary<string, object> { ["type"] = "integer" }, new Dictionary<string, object> { ["type"] = "string" } }, ["title"] = "Id" },
                            ["parent_id"] = new Dictionary<string, object> { ["anyOf"] = new List<object> { new Dictionary<string, object> { ["type"] = "integer" }, new Dictionary<string, object> { ["type"] = "string" } }, ["title"] = "Parent Id" },
                            ["log"] = new Dictionary<string, object> { ["title"] = "Log", ["type"] = "string" },
                            ["duration"] = new Dictionary<string, object> { ["title"] = "Duration", ["type"] = "number" },
                            ["status"] = new Dictionary<string, object> { ["enum"] = new List<object> { "pending", "done" }, ["title"] = "Status", ["type"] = "string" }
                        },
                        ["title"] = "MetadataDict",
                        ["type"] = "object"
                    },
                    ["OptionDict"] = new Dictionary<string, object>
                    {
                        ["description"] = "A typed dictionary to represent an option in a ChatMessage. A list of these\ndictionaries is used for the `options` field in a ChatMessage.\nParameters:\n    value: The value to return when the option is selected.\n    label: The text to display in the option, if different from the value.",
                        ["properties"] = new Dictionary<string, object>
                        {
                            ["value"] = new Dictionary<string, object> { ["title"] = "Value", ["type"] = "string" },
                            ["label"] = new Dictionary<string, object> { ["title"] = "Label", ["type"] = "string" }
                        },
                        ["required"] = new List<object> { "value" },
                        ["title"] = "OptionDict",
                        ["type"] = "object"
                    },
                    ["TextMessage"] = new Dictionary<string, object>
                    {
                        ["properties"] = new Dictionary<string, object>
                        {
                            ["text"] = new Dictionary<string, object> { ["title"] = "Text", ["type"] = "string" },
                            ["type"] = new Dictionary<string, object> { ["const"] = "text", ["default"] = "text", ["title"] = "Type", ["type"] = "string" }
                        },
                        ["required"] = new List<object> { "text" },
                        ["title"] = "TextMessage",
                        ["type"] = "object"
                    }
                },
                ["items"] = new Dictionary<string, object> { ["$ref"] = "#/$defs/Message" },
                ["title"] = "ChatbotDataMessages",
                ["type"] = "array",
                ["additional_description"] = (object?)null
            };
        }
    }
}
