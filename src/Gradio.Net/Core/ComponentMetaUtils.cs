using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using Gradio.Net.Components;
using Gradio.Net.Events;

namespace Gradio.Net.Core;

public static class ComponentMetaUtils
{
    private const string InterfaceTemplate = @"{{contents}}
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Gradio.Net.Blocks;
    using Gradio.Net.Components;
    using Gradio.Net.Events;

    {% for event in events %}
    public Dependency {{event.EventName}}(Func<object[], object> fn = null,
        IEnumerable<Block> inputs = null,
        IEnumerable<Block> outputs = null,
        string apiName = null,
        bool scrollToOutput = false,
        string showProgress = ""full"",
        IEnumerable<Component> showProgressOn = null,
        bool? queue = null,
        bool batch = false,
        int maxBatchSize = 4,
        bool preprocess = true,
        bool postprocess = true,
        object cancels = null,
        object every = null,
        string triggerMode = null,
        string js = null,
        object concurrencyLimit = ""default"",
        string concurrencyId = null,
        string apiVisibility = ""public"",
        object key = null,
        object apiDescription = null,
        Func<object[], object> validator = null
        {% for arg in event.EventSpecificArgs %},
        {{arg.Type}} {{arg.Name}}
        {% endfor %})
    {
        throw new NotImplementedException();
    }
    {% endfor %}
";

    /// <summary>///</summary>
    public static string CreatePyi(string classCode, List<EventListener> events)
    {
        var template = InterfaceTemplate;
        var eventTemplate = events.Select(e => e).ToList();

        var eventMethods = new StringBuilder();
        foreach (var @event in eventTemplate)
        {
            var eventArgs = string.Join(",\n        ", @event.EventSpecificArgs.Select(arg => $"{arg.Type} {arg.Name}"));
            var argsSection = @event.EventSpecificArgs.Any() ? ",\n        " + eventArgs : "";

            eventMethods.Append($@"
    public Dependency {@event.EventName}(Func<object[], object> fn = null,
        IEnumerable<Block> inputs = null,
        IEnumerable<Block> outputs = null,
        string apiName = null,
        bool scrollToOutput = false,
        string showProgress = ""full"",
        IEnumerable<Component> showProgressOn = null,
        bool? queue = null,
        bool batch = false,
        int maxBatchSize = 4,
        bool preprocess = true,
        bool postprocess = true,
        object cancels = null,
        object every = null,
        string triggerMode = null,
        string js = null,
        object concurrencyLimit = ""default"",
        string concurrencyId = null,
        string apiVisibility = ""public"",
        object key = null,
        object apiDescription = null,
        Func<object[], object> validator = null{argsSection})
    {{
        throw new NotImplementedException();
    }}
");
        }

        var content = template
            .Replace("{{contents}}", classCode)
            .Replace("{% for event in events %}", "")
            .Replace("{% endfor %}", eventMethods.ToString())
            .Replace("{% for arg in event.EventSpecificArgs %}", "")
            .Replace("{% endfor %}", "");

        return content;
    }

    /// <summary>///</summary>
    public static (string, int) ExtractClassSourceCode(string code, string className)
    {
        var classMatch = Regex.Match(code, $@"class+{Regex.Escape(className)}*[{{:]");
        if (!classMatch.Success)
        {
            return (null, 0);
        }

        int startLine = code.Substring(0, classMatch.Index).Split('\n').Length;

        var lines = code.Split('\n');
        var classLines = new List<string>();
        bool inClass = false;
        int braceCount = 0;

        foreach (var line in lines)
        {
            if (line.Contains($"class {className}"))
            {
                inClass = true;
                braceCount += line.Count(c => c == '{') - line.Count(c => c == '}');
                classLines.Add(line);
            }
            else if (inClass)
            {
                braceCount += line.Count(c => c == '{') - line.Count(c => c == '}');
                classLines.Add(line);
                if (braceCount <= 0)
                {
                    break;
                }
            }
        }

        return (string.Join("\n", classLines), startLine);
    }

    /// <summary>///</summary>
    public static void CreateOrModifyPyi(Type componentType, string className, List<EventListener> events)
    {
        try
        {
            var sourceFile = new FileInfo(componentType.Assembly.Location);
            var sourceCode = File.ReadAllText(sourceFile.FullName, Encoding.UTF8);

            var (currentImpl, lineno) = ExtractClassSourceCode(sourceCode, className);

            if (string.IsNullOrEmpty(currentImpl))
            {
                throw new ArgumentException("Couldn't find class source code");
            }

            var newInterface = CreatePyi(currentImpl, events);

            var pyiFile = new FileInfo(Path.ChangeExtension(sourceFile.FullName, ".cs"));
            if (!pyiFile.Exists)
            {
                File.WriteAllText(pyiFile.FullName, newInterface, Encoding.UTF8);
            }
            else
            {
                var currentContent = File.ReadAllText(pyiFile.FullName, Encoding.UTF8);
                var (currentInterface, _) = ExtractClassSourceCode(currentContent, className);

                if (string.IsNullOrEmpty(currentInterface))
                {
                    File.AppendAllText(pyiFile.FullName, "\n" + newInterface, Encoding.UTF8);
                }
                else
                {
                    var updatedContent = currentContent.Replace(currentInterface, newInterface.Trim());
                    if (currentContent != updatedContent)
                    {
                        File.WriteAllText(pyiFile.FullName, updatedContent, Encoding.UTF8);
                    }
                }
            }
        }
        catch (Exception)
        {
        }
    }

    /// <summary>///</summary>
    public static (bool inEventListener, bool isRender) GetLocalContexts()
    {
        return (
            LocalContext.InEventListener,
            LocalContext.CurrentRenderable != null
        );
    }

    /// <summary>///</summary>
    public static bool Updateable<T>(T component, Dictionary<string, object> constructorArgs)
        where T : Component
    {
        var constructorArgsField = component.GetType().GetField("_constructor_args", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        bool initializedBefore = constructorArgsField != null && constructorArgsField.GetValue(component) != null;

        if (!initializedBefore)
        {
            if (constructorArgsField != null)
            {
                constructorArgsField.SetValue(component, new List<Dictionary<string, object>>());
            }
        }

        if (constructorArgsField != null)
        {
            var argsList = constructorArgsField.GetValue(component) as List<Dictionary<string, object>>;
            if (argsList != null)
            {
                argsList.Add(constructorArgs);
            }
        }

        var (inEventListener, isRender) = GetLocalContexts();

        if (inEventListener && initializedBefore && !isRender)
        {
            return false;
        }

        return true;
    }

    /// <summary>///</summary>
    public static void AddEventListenersToComponent(Type componentType, List<EventListener> events)
    {
        foreach (var @event in events)
        {
            var methodName = @event.EventName;
            var methodInfo = componentType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            if (methodInfo == null)
            {
                AddEventMethodToType(componentType, @event);
            }
        }
    }

    /// <summary>///</summary>
    private static void AddEventMethodToType(Type type, EventListener eventListener)
    {

    }
}

/// <summary>///</summary>
