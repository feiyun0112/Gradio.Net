using System.Collections.Concurrent;
using System.Diagnostics;
using System.Web;
using Gradio.Net.Core;
using Gradio.Net.Utils;


namespace Gradio.Net;

public static class QueueingUtils
{
    public static Tuple<bool, List<Dictionary<string, object>>> ProcessValidationResponse(object validationResponse, BlockFunction fn = null)
    {
        var validationData = new List<Dictionary<string, object>>();
        var paramNames = new List<string>();

        if (fn != null && fn.Fn != null)
        {
            var sig = fn.Fn.GetType().GetMethod("Invoke").GetParameters();
            paramNames = sig.Select(p => p.Name).ToList();
        }

        if (validationResponse is List<object> validationResponseList)
        {
            for (int i = 0; i < validationResponseList.Count; i++)
            {
                var data = validationResponseList[i];
                if (data is Dictionary<string, object> dataDict && dataDict.ContainsKey("__type__") && dataDict["__type__"] == "validate")
                {
                    var paramName = i < paramNames.Count ? paramNames[i] : $"parameter_{i}";
                    var dataWithName = new Dictionary<string, object>(dataDict);
                    dataWithName["parameter_name"] = paramName;
                    validationData.Add(dataWithName);
                }
                else
                {
                    validationData.Add(new Dictionary<string, object> { { "is_valid", true }, { "message", "" } });
                }
            }
        }
        else if (validationResponse is Dictionary<string, object> validationResponseDict && validationResponseDict.ContainsKey("is_valid") && !(bool)validationResponseDict["is_valid"])
        {
            validationData.Add(validationResponseDict);
        }
        else
        {
            validationData.Add(new Dictionary<string, object> { { "is_valid", true }, { "message", "" } });
        }

        var allValid = validationData.All(x => x.ContainsKey("is_valid") && (bool)x["is_valid"]);
        return Tuple.Create(allValid, validationData);
    }
}
