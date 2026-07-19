using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Gradio.Net.Data;

internal static class TraverseHelper
{
    public static object Traverse(object obj, Func<object, object> func, Func<object, bool> predicate)
    {
        if (predicate(obj))
        {
            return func(obj);
        }
        else if (obj is Dictionary<string, object> dict)
        {
            return dict.ToDictionary(
                kvp => kvp.Key,
                kvp => Traverse(kvp.Value, func, predicate)
            );
        }
        else if (obj is List<object> list)
        {
            return list.Select(item => Traverse(item, func, predicate)).ToList();
        }
        else if (obj is object[] array)
        {
            return array.Select(item => Traverse(item, func, predicate)).ToArray();
        }
        else
        {
            return obj;
        }
    }

    public static string GenerateRandomHex(int length)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[length / 2];
            rng.GetBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
