using System.Text.RegularExpressions;
using System.Collections;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace Gradio.Net.Core;

public class TemplateEngine
{
    private static readonly Regex _variableRegex = new Regex(@"\{\{\s*(.+?)\s*\}\}", RegexOptions.Compiled);

    public string Render(string template, Dictionary<string, object> context)
    {
        return _variableRegex.Replace(template, match =>
        {
            var expression = match.Groups[1].Value.Trim();
            return EvaluateExpression(expression, context, match);
        });
    }

    private string EvaluateExpression(string expression, Dictionary<string, object> context, Match originalMatch)
    {
        try
        {
            // Handle expressions with filters: {{ variable | filter }}
            if (expression.Contains('|'))
            {
                return EvaluateFilterExpression(expression, context);
            }

            // Handle expressions with or operator: {{ variable or default }}
            if (expression.Contains(" or "))
            {
                return EvaluateOrExpression(expression, context);
            }

            // Handle simple variable access: {{ variable }}
            if (!expression.Contains('.') && !expression.Contains('(') && !expression.Contains('['))
            {
                if (context.TryGetValue(expression, out var value))
                {
                    return Convert.ToString(value) ?? string.Empty;
                }
                return originalMatch.Groups[0].Value;
            }

            // Handle complex expressions with method calls, dot notation, or dictionary access
            return EvaluateComplexExpression(expression, context);
        }
        catch (Exception)
        {
            // If evaluation fails, return the original expression
            return originalMatch.Groups[0].Value;
        }
    }

    private string EvaluateOrExpression(string expression, Dictionary<string, object> context)
    {
        var parts = expression.Split(new[] { " or " }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return string.Empty;
        }

        string leftExpr = parts[0].Trim();
        string rightExpr = parts[1].Trim();

        // Evaluate left side
        object? leftValue = EvaluateComplexExpressionObject(leftExpr, context);

        // If left value is null, undefined, empty string, etc., use right side
        if (leftValue == null || (leftValue is string str && string.IsNullOrEmpty(str)))
        {
            // Check if right side is a string literal
            if ((rightExpr.StartsWith('"') && rightExpr.EndsWith('"')) ||
                (rightExpr.StartsWith("'") && rightExpr.EndsWith("'")))
            {
                return rightExpr.Substring(1, rightExpr.Length - 2);
            }
            // Otherwise evaluate as expression
            return EvaluateComplexExpression(rightExpr, context);
        }

        return Convert.ToString(leftValue) ?? string.Empty;
    }

    private string EvaluateFilterExpression(string expression, Dictionary<string, object> context)
    {
        var parts = expression.Split('|', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return string.Empty;
        }

        string valueExpr = parts[0].Trim();
        string filterExpr = parts[1].Trim();

        // Evaluate the value expression
        object? value = EvaluateComplexExpressionObject(valueExpr, context);

        // Apply the filter
        switch (filterExpr.ToLower())
        {
            case "toorjson":
                var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                return json
                    .Replace("<", "\\u003c")
                    .Replace(">", "\\u003e")
                    .Replace("&", "\\u0026")
                    .Replace("'", "\\u0027");
            default:
                // Unknown filter, return original value
                return Convert.ToString(value) ?? string.Empty;
        }
    }

    private object? EvaluateComplexExpressionObject(string expression, Dictionary<string, object> context)
    {
        // Start with the context as the initial value
        object? current = context;
        int index = 0;

        while (index < expression.Length)
        {
            // Skip whitespace
            while (index < expression.Length && char.IsWhiteSpace(expression[index]))
                index++;

            if (index >= expression.Length)
                break;

            // Check if it's a method call
            if (index + 1 < expression.Length && expression.Substring(index).Contains("("))
            {
                // Find the end of the method call
                int methodEndIndex = FindMatchingParenthesis(expression, index);
                if (methodEndIndex == -1)
                    break;

                // Extract the method call expression
                string methodCall = expression.Substring(index, methodEndIndex - index + 1);
                current = EvaluateMethodCall(methodCall, context);
                if (current == null)
                    return null;

                index = methodEndIndex + 1;
            }
            // Check if it's a property access with dot notation
            else if (expression[index] == '.')
            {
                index++;
                // Skip whitespace after dot
                while (index < expression.Length && char.IsWhiteSpace(expression[index]))
                    index++;

                // Find the next property name or method call
                int propertyEndIndex = index;
                while (propertyEndIndex < expression.Length &&
                       (char.IsLetterOrDigit(expression[propertyEndIndex]) ||
                        expression[propertyEndIndex] == '_'))
                {
                    propertyEndIndex++;
                }

                string propertyName = expression.Substring(index, propertyEndIndex - index).Trim();
                if (propertyEndIndex < expression.Length && expression[propertyEndIndex] == '(')
                {
                    int methodEndIndex = FindMatchingParenthesis(expression, propertyEndIndex);
                    if (methodEndIndex == -1)
                        return null;

                    string argsString = expression.Substring(propertyEndIndex + 1, methodEndIndex - propertyEndIndex - 1);
                    var args = ParseArguments(argsString, context);
                    current = HandleMethodCall(current, propertyName, args, context);
                    if (current == null)
                        return null;

                    index = methodEndIndex + 1;
                }
                else
                {
                    current = EvaluateExpressionPart(current, propertyName, context);
                    if (current == null)
                        return null;

                    index = propertyEndIndex;
                }
            }
            // Check if it's dictionary access with square brackets
            else if (expression[index] == '[')
            {
                index++;
                // Skip whitespace after opening bracket
                while (index < expression.Length && char.IsWhiteSpace(expression[index]))
                    index++;

                // Find the end of the key, handling quotes
                char quoteChar = '\0';
                if (expression[index] == '"' || expression[index] == '\'')
                {
                    quoteChar = expression[index];
                    index++;
                }

                int keyEndIndex = index;
                while (keyEndIndex < expression.Length)
                {
                    if (quoteChar != '\0')
                    {
                        // If we're inside quotes, look for the closing quote
                        if (expression[keyEndIndex] == quoteChar)
                        {
                            break;
                        }
                    }
                    else
                    {
                        // If we're not inside quotes, look for the closing bracket
                        if (expression[keyEndIndex] == ']')
                        {
                            break;
                        }
                    }
                    keyEndIndex++;
                }

                if (keyEndIndex >= expression.Length)
                    break;

                // Extract the key
                string key = expression.Substring(index, keyEndIndex - index).Trim();

                // Skip closing quote if present
                if (quoteChar != '\0')
                {
                    keyEndIndex++;
                }

                // Skip closing bracket
                if (keyEndIndex < expression.Length && expression[keyEndIndex] == ']')
                {
                    keyEndIndex++;
                }

                // Access the dictionary with the key
                current = EvaluateDictionaryAccess(current, key);
                if (current == null)
                    return null;

                index = keyEndIndex;
            }
            // Simple property access without dot (first part)
            else
            {
                // Find the end of the property name or method call
                int propertyEndIndex = index;
                while (propertyEndIndex < expression.Length &&
                       (char.IsLetterOrDigit(expression[propertyEndIndex]) ||
                        expression[propertyEndIndex] == '_'))
                {
                    propertyEndIndex++;
                }

                string propertyName = expression.Substring(index, propertyEndIndex - index).Trim();
                current = EvaluateExpressionPart(current, propertyName, context);
                if (current == null)
                    return null;

                index = propertyEndIndex;
            }
        }

        return current;
    }

    private string EvaluateComplexExpression(string expression, Dictionary<string, object> context)
    {
        object? result = EvaluateComplexExpressionObject(expression, context);
        return Convert.ToString(result) ?? string.Empty;
    }

    private int FindMatchingParenthesis(string expression, int startIndex)
    {
        int openCount = 0;
        bool inString = false;
        char stringChar = ' ';

        for (int i = startIndex; i < expression.Length; i++)
        {
            char c = expression[i];

            // Handle string literals to ignore parentheses inside strings
            if (c == '"' || c == '\'')
            {
                if (!inString)
                {
                    inString = true;
                    stringChar = c;
                }
                else if (c == stringChar)
                {
                    inString = false;
                }
            }
            else if (!inString)
            {
                if (c == '(')
                {
                    openCount++;
                }
                else if (c == ')')
                {
                    openCount--;
                    if (openCount == 0)
                    {
                        return i;
                    }
                }
            }
        }

        return -1; // No matching parenthesis found
    }

    private object? EvaluateDictionaryAccess(object? obj, string key)
    {
        if (obj is Dictionary<string, object> dict)
        {
            return dict.TryGetValue(key, out var value) ? value : null;
        }
        if (obj is IDictionary dictionary)
        {
            return dictionary.Contains(key) ? dictionary[key] : null;
        }
        return null;
    }

    private (string part, int nextIndex) GetNextExpressionPart(string expression, int startIndex)
    {
        // Skip whitespace
        while (startIndex < expression.Length && char.IsWhiteSpace(expression[startIndex]))
            startIndex++;

        if (startIndex >= expression.Length)
            return (string.Empty, startIndex);

        // Check for method call
        if (expression[startIndex] == '(')
        {
            // This is a closing parenthesis, return empty to end evaluation
            return (string.Empty, expression.Length);
        }
        else if (expression[startIndex] == ')')
        {
            // This is a closing parenthesis, return empty to end evaluation
            return (string.Empty, startIndex + 1);
        }
        else if (expression[startIndex] == ',')
        {
            // This is an argument separator, return empty to end evaluation
            return (string.Empty, startIndex + 1);
        }

        // Find the end of the current part
        int endIndex = startIndex;
        while (endIndex < expression.Length)
        {
            char c = expression[endIndex];
            if (c == '.' || c == '(')
            {
                break;
            }
            endIndex++;
        }

        string part = expression.Substring(startIndex, endIndex - startIndex).Trim();
        return (part, endIndex);
    }

    private object? EvaluateExpressionPart(object? current, string part, Dictionary<string, object> context)
    {
        if (current == null)
            return null;

        // Handle dictionary access
        if (current is Dictionary<string, object> dict)
        {
            return dict.TryGetValue(part, out var value) ? value : null;
        }
        if (current is IDictionary dictionary)
        {
            return dictionary.Contains(part) ? dictionary[part] : null;
        }

        // Handle object property access
        var property = current.GetType().GetProperty(part);
        if (property != null)
        {
            return property.GetValue(current);
        }

        return null;
    }

    private object? EvaluateMethodCall(string expression, Dictionary<string, object> context)
    {
        // Find the method name and arguments
        var methodMatch = Regex.Match(expression, @"^([a-zA-Z0-9_.]+)\s*\((.*)\)");
        if (!methodMatch.Success)
            return null;

        var objectPath = methodMatch.Groups[1].Value;
        var argsString = methodMatch.Groups[2].Value;

        var lastDot = objectPath.LastIndexOf('.');
        string targetPath;
        string methodName;

        if (lastDot >= 0)
        {
            targetPath = objectPath.Substring(0, lastDot);
            methodName = objectPath.Substring(lastDot + 1);
        }
        else
        {
            targetPath = objectPath;
            methodName = objectPath;
        }

        // Evaluate the object path to get the actual object, not a string
        object? obj = EvaluateComplexExpressionObject(targetPath, context);
        if (obj == null && context.TryGetValue(targetPath, out var ctxObj))
        {
            obj = ctxObj;
        }
        if (obj == null)
            return null;

        // Parse arguments
        var args = ParseArguments(argsString, context);

        // Handle method call
        return HandleMethodCall(obj, methodName, args, context);
    }

    private List<object> ParseArguments(string argsString, Dictionary<string, object> context)
    {
        var args = new List<object>();
        if (string.IsNullOrWhiteSpace(argsString))
            return args;

        // Split arguments by commas, respecting parentheses and quotes
        var argParts = SplitArguments(argsString);

        foreach (var argPart in argParts)
        {
            string arg = argPart.Trim();
            if (string.IsNullOrEmpty(arg))
                continue;

            // Handle empty dictionary literal
            if (arg == "{}")
            {
                args.Add(new Dictionary<string, object>());
            }
            // Handle string literals
            else if ((arg.StartsWith('"') && arg.EndsWith('"')) ||
                     (arg.StartsWith("'") && arg.EndsWith("'")))
            {
                // Remove quotes and add as string
                args.Add(arg.Substring(1, arg.Length - 2));
            }
            // Handle numbers
            else if (double.TryParse(arg, out var number))
            {
                args.Add(number);
            }
            // Handle nested expressions {{ ... }}
            else if (arg.StartsWith("{{") && arg.EndsWith("}}"))
            {
                var nestedExpr = arg.Substring(2, arg.Length - 4).Trim();
                var nestedValue = EvaluateComplexExpression(nestedExpr, context);
                args.Add(nestedValue);
            }
            // Handle identifiers
            else
            {
                if (context.TryGetValue(arg, out var value))
                {
                    args.Add(value);
                }
                else
                {
                    args.Add(arg);
                }
            }
        }

        return args;
    }

    private List<string> SplitArguments(string argsString)
    {
        var result = new List<string>();
        int startIndex = 0;
        int openParentheses = 0;
        bool inString = false;
        char stringChar = ' ';

        for (int i = 0; i < argsString.Length; i++)
        {
            char c = argsString[i];

            // Handle string literals
            if (c == '"' || c == '\'')
            {
                if (!inString)
                {
                    inString = true;
                    stringChar = c;
                }
                else if (c == stringChar)
                {
                    inString = false;
                }
            }
            else if (!inString)
            {
                if (c == '(')
                {
                    openParentheses++;
                }
                else if (c == ')')
                {
                    openParentheses--;
                }
                else if (c == ',' && openParentheses == 0)
                {
                    // Found a comma that's not inside parentheses or strings
                    result.Add(argsString.Substring(startIndex, i - startIndex));
                    startIndex = i + 1;
                }
            }
        }

        // Add the last argument
        result.Add(argsString.Substring(startIndex));
        return result;
    }

    private object? HandleMethodCall(object obj, string methodName, List<object> args, Dictionary<string, object> context)
    {
        switch (methodName.ToLower())
        {
            case "get":
                return HandleGetMethod(obj, args, context);
            default:
                return null;
        }
    }

    private object? HandleGetMethod(object obj, List<object> args, Dictionary<string, object> context)
    {
        if (args.Count == 0)
            return null;

        var key = Convert.ToString(args[0]) ?? string.Empty;
        object? defaultValue = args.Count > 1 ? args[1] : null;

        // Handle dictionary get()
        if (obj is Dictionary<string, object> dict)
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }
            return defaultValue;
        }
        if (obj is IDictionary dictionary)
        {
            if (dictionary.Contains(key))
            {
                return dictionary[key];
            }
            return defaultValue;
        }

        // Handle nested get() calls with default values that are dictionaries
        if (defaultValue is string defaultStr && defaultStr == "{}")
        {
            return new Dictionary<string, object>();
        }

        return defaultValue;
    }
}
