namespace Gradio.Net;

using System.Text;
using System.Text.RegularExpressions;

internal static class StringExtensions
{
    internal static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        StringBuilder result = new StringBuilder();
        bool isFirstWord = true;

        foreach (char c in input)
        {
            if (c == '_')
            {
                isFirstWord = true;
            }
            else
            {
                if (isFirstWord)
                {
                    if (c >= '0' && c <= '9')
                    {
                        result.Append('_');
                        result.Append(char.ToUpper(c));
                    }
                    else
                    {
                        result.Append(char.ToUpper(c));
                    }

                    isFirstWord = false;
                }
                else
                {
                    result.Append(c);
                }
            }
        }

        return result.ToString();
    }

    internal static string ToSnakeCase(this string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (text.Length < 2)
        {
            return text.ToLowerInvariant();
        }

        StringBuilder sb = new();
        sb.Append(char.ToLowerInvariant(text[0]));

        for (int i = 1; i < text.Length; ++i)
        {
            char c = text[i];

            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
