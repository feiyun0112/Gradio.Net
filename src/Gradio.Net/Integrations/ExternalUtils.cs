using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Gradio.Net.Core.Exceptions;


namespace Gradio.Net;

public class ExternalUtils
{
    private static readonly HttpClient Http = new HttpClient();

    /// <summary>///</summary>
    public static Tuple<string, List<string>> GetModelInfo(string modelName, string token = null)
    {

        var req = new HttpRequestMessage(HttpMethod.Get, $"https://huggingface.co/api/models/{modelName}");
        if (!string.IsNullOrWhiteSpace(token))
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        using var resp = Http.Send(req);
        resp.EnsureSuccessStatusCode();

        var json = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        using var doc = JsonDocument.Parse(json);

        string pipeline = string.Empty;
        var tags = new List<string>();

        if (doc.RootElement.TryGetProperty("pipeline_tag", out var p) && p.ValueKind == JsonValueKind.String)
        {
            pipeline = p.GetString() ?? string.Empty;
        }

        if (doc.RootElement.TryGetProperty("tags", out var t) && t.ValueKind == JsonValueKind.Array)
        {
            foreach (var tag in t.EnumerateArray())
            {
                if (tag.ValueKind == JsonValueKind.String)
                {
                    tags.Add(tag.GetString() ?? string.Empty);
                }
            }
        }

        return new Tuple<string, List<string>>(pipeline, tags);
    }

    /// <summary>///</summary>
    public static Dictionary<string, List<double>> GetTabularExamples(string modelName)
    {
        var readmeResp = Http.GetAsync($"https://huggingface.co/{modelName}/resolve/main/README.md").GetAwaiter().GetResult();
        if (!readmeResp.IsSuccessStatusCode)
        {
            throw new Exception($"Cannot load examples from README for {modelName}");
        }

        var readme = readmeResp.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        // Best-effort parse for widget.structuredData in front matter.
        // We avoid a YAML dependency and support common JSON-like layouts used on HF cards.
        var structuredDataMatch = Regex.Match(
            readme,
            @"structuredData\s*:\s*(\{[\s\S]*?\})\s*(\n\w|\n---|$)",
            RegexOptions.Multiline
        );

        if (!structuredDataMatch.Success)
        {
            throw new Exception(
                $"No example data found in README.md of {modelName} - Cannot build gradio demo."
            );
        }

        var raw = structuredDataMatch.Groups[1].Value.Trim();
        if (!raw.StartsWith("{"))
        {
            throw new Exception(
                $"No example data found in README.md of {modelName} - Cannot build gradio demo."
            );
        }

        // Normalize single quotes to double quotes when present in card metadata snippets.
        if (raw.Contains('\''))
        {
            raw = raw.Replace("'", "\"");
        }

        using var doc = JsonDocument.Parse(raw);
        var output = new Dictionary<string, List<double>>();
        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            var values = new List<double>();
            if (prop.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in prop.Value.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Number)
                    {
                        values.Add(item.GetDouble());
                    }
                    else if (item.ValueKind == JsonValueKind.String)
                    {
                        var s = item.GetString();
                        if (string.Equals(s, "NaN", StringComparison.OrdinalIgnoreCase))
                        {
                            values.Add(double.NaN);
                        }
                        else if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                        {
                            values.Add(d);
                        }
                        else
                        {
                            values.Add(double.NaN);
                        }
                    }
                    else
                    {
                        values.Add(double.NaN);
                    }
                }
            }
            output[prop.Name] = values;
        }

        return output;
    }

    /// <summary>///</summary>
    public static Tuple<List<string>, List<List<double>>> ColsToRows(Dictionary<string, List<object>> exampleData)
    {
        var headers = exampleData.Keys.ToList();
        int nRows = exampleData.Values.Max(col => col?.Count ?? 0);
        var data = new List<List<double>>();

        for (int rowIndex = 0; rowIndex < nRows; rowIndex++)
        {
            var rowData = new List<double>();
            foreach (var header in headers)
            {
                var col = exampleData[header];
                if (col == null || rowIndex >= col.Count)
                {
                    rowData.Add(double.NaN);
                }
                else
                {
                    if (col[rowIndex] is double d)
                    {
                        rowData.Add(d);
                    }
                    else if (double.TryParse(col[rowIndex].ToString(), out double parsed))
                    {
                        rowData.Add(parsed);
                    }
                    else
                    {
                        rowData.Add(double.NaN);
                    }
                }
            }
            data.Add(rowData);
        }

        return new Tuple<List<string>, List<List<double>>>(headers, data);
    }

    /// <summary>///</summary>
    public static Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> RowsToCols(Dictionary<string, object> incomingData)
    {
        var dataColumnWise = new Dictionary<string, List<string>>();

        if (incomingData.TryGetValue("headers", out object headersObj) &&
            incomingData.TryGetValue("data", out object dataObj))
        {
            var headers = headersObj as List<string>;
            var data = dataObj as List<List<object>>;

            if (headers != null && data != null)
            {
                for (int i = 0; i < headers.Count; i++)
                {
                    var header = headers[i];
                    dataColumnWise[header] = data.Select(row =>
                        row.Count > i ? row[i].ToString() : "").ToList();
                }
            }
        }

        return new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>
        {
            { "inputs", new Dictionary<string, Dictionary<string, List<string>>>
                { { "data", dataColumnWise } }
            }
        };
    }

    /// <summary>///</summary>
    public static Dictionary<string, double> PostprocessLabel(List<ImageClassificationOutputElement> scores)
    {
        var result = new Dictionary<string, double>();
        foreach (var score in scores)
        {
            result[score.Label] = score.Score;
        }
        return result;
    }

    /// <summary>///</summary>
    public static Dictionary<string, double> PostprocessMaskTokens(List<Dictionary<string, object>> scores)
    {
        var result = new Dictionary<string, double>();
        foreach (var score in scores)
        {
            if (score.TryGetValue("token_str", out object tokenStrObj) &&
                score.TryGetValue("score", out object scoreObj))
            {
                string tokenStr = tokenStrObj.ToString();
                double scoreValue = scoreObj is double d ? d : double.Parse(scoreObj.ToString());
                result[tokenStr] = scoreValue;
            }
        }
        return result;
    }

    /// <summary>///</summary>
    public static Tuple<string, Dictionary<string, double>> PostprocessQuestionAnswering(Dictionary<string, object> answer)
    {
        if (answer.TryGetValue("answer", out object answerObj) &&
            answer.TryGetValue("score", out object scoreObj))
        {
            string answerStr = answerObj.ToString();
            double scoreValue = scoreObj is double d ? d : double.Parse(scoreObj.ToString());
            return new Tuple<string, Dictionary<string, double>>(
                answerStr,
                new Dictionary<string, double> { { answerStr, scoreValue } }
            );
        }
        return new Tuple<string, Dictionary<string, double>>("", new Dictionary<string, double>());
    }

    /// <summary>///</summary>
    public static Dictionary<string, double> PostprocessVisualQuestionAnswering(List<Dictionary<string, object>> scores)
    {
        var result = new Dictionary<string, double>();
        foreach (var score in scores)
        {
            if (score.TryGetValue("answer", out object answerObj) &&
                score.TryGetValue("score", out object scoreObj))
            {
                string answerStr = answerObj.ToString();
                double scoreValue = scoreObj is double d ? d : double.Parse(scoreObj.ToString());
                result[answerStr] = scoreValue;
            }
        }
        return result;
    }

    /// <summary>///</summary>
    public static string EncodeToBase64(HttpResponseMessage response)
    {
        var bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
        var base64 = Convert.ToBase64String(bytes);

        // Python parity: if returned blob already embeds data prefix, keep it.
        if (base64.Contains(";base64,"))
        {
            return base64;
        }

        string contentType = response.Content.Headers.ContentType?.MediaType;

        if (string.Equals(contentType, "application/json", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var text = Encoding.UTF8.GetString(bytes);
                using var doc = JsonDocument.Parse(text);
                if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
                {
                    var first = doc.RootElement[0];
                    if (first.TryGetProperty("content-type", out var ct) && ct.ValueKind == JsonValueKind.String)
                    {
                        contentType = ct.GetString();
                    }
                    if (first.TryGetProperty("blob", out var blob) && blob.ValueKind == JsonValueKind.String)
                    {
                        base64 = blob.GetString() ?? base64;
                    }
                }
            }
            catch
            {
                // fall back to response headers/content bytes
            }
        }

        contentType ??= "application/octet-stream";
        return $"data:{contentType};base64,{base64}";
    }

    /// <summary>///</summary>
    public static List<Tuple<string, string>> FormatNerList(string inputString, List<Dictionary<string, object>> nerGroups)
    {
        var output = new List<Tuple<string, string>>();
        if (nerGroups.Count == 0)
        {
            output.Add(new Tuple<string, string>(inputString, null));
            return output;
        }

        int end = 0;
        int prevEnd = 0;

        foreach (var group in nerGroups)
        {
            if (group.TryGetValue("entity_group", out object entityObj) &&
                group.TryGetValue("start", out object startObj) &&
                group.TryGetValue("end", out object endObj))
            {
                string entity = entityObj.ToString();
                int start = Convert.ToInt32(startObj);
                end = Convert.ToInt32(endObj);

                output.Add(new Tuple<string, string>(inputString.Substring(prevEnd, start - prevEnd), null));
                output.Add(new Tuple<string, string>(inputString.Substring(start, end - start), entity));
                prevEnd = end;
            }
        }

        output.Add(new Tuple<string, string>(inputString.Substring(end), null));
        return output;
    }

    /// <summary>///</summary>
    public static Tuple<string, List<string>, List<string>> ChatbotPreprocess(string text, Dictionary<string, object> state)
    {
        if (state == null || !state.ContainsKey("conversation"))
        {
            return new Tuple<string, List<string>, List<string>>(text, new List<string>(), new List<string>());
        }

        var conversation = state["conversation"] as Dictionary<string, object>;
        var generatedResponses = new List<string>();
        var pastUserInputs = new List<string>();

        if (conversation != null)
        {
            if (conversation.TryGetValue("generated_responses", out object genResponsesObj))
            {
                generatedResponses = (genResponsesObj as List<object>).Select(r => r.ToString()).ToList();
            }

            if (conversation.TryGetValue("past_user_inputs", out object pastInputsObj))
            {
                pastUserInputs = (pastInputsObj as List<object>).Select(i => i.ToString()).ToList();
            }
        }

        return new Tuple<string, List<string>, List<string>>(text, generatedResponses, pastUserInputs);
    }

    /// <summary>///</summary>
    public static Tuple<List<Tuple<string, string>>, Dictionary<string, object>> ChatbotPostprocess(Dictionary<string, object> response)
    {
        var chatbotHistory = new List<Tuple<string, string>>();

        if (response.TryGetValue("conversation", out object conversationObj))
        {
            var conversation = conversationObj as Dictionary<string, object>;
            if (conversation != null)
            {
                if (conversation.TryGetValue("past_user_inputs", out object pastInputsObj) &&
                    conversation.TryGetValue("generated_responses", out object genResponsesObj))
                {
                    var pastInputs = (pastInputsObj as List<object>).Select(i => i.ToString()).ToList();
                    var genResponses = (genResponsesObj as List<object>).Select(r => r.ToString()).ToList();

                    int minLength = Math.Min(pastInputs.Count, genResponses.Count);
                    for (int i = 0; i < minLength; i++)
                    {
                        chatbotHistory.Add(new Tuple<string, string>(pastInputs[i], genResponses[i]));
                    }
                }
            }
        }

        return new Tuple<List<Tuple<string, string>>, Dictionary<string, object>>(chatbotHistory, response);
    }

    /// <summary>///</summary>
    public static string MethodBox(string method)
    {
        var colorMap = new Dictionary<string, string>
        {
            { "GET", "#61affe" },
            { "POST", "#49cc90" },
            { "PUT", "#fca130" },
            { "DELETE", "#f93e3e" },
            { "PATCH", "#50e3c2" }
        };

        string color = colorMap.TryGetValue(method.ToUpper(), out string c) ? c : "#999";

        return $"<span style='display:inline-block;min-width:48px;padding:2px 10px;border-radius:4px;background:{color};color:white;font-weight:bold;font-family:monospace;margin-right:8px;text-align:center;border:2px solid {color};box-shadow:0 1px 2px rgba(0,0,0,0.08);'>{method.ToUpper()}</span>";
    }

    /// <summary>///</summary>
    public static void HandleHfError(Exception e)
    {
        string errorMessage = e.ToString();
        if (errorMessage.Contains("429"))
        {
            throw new TooManyRequestsError();
        }
        else if (errorMessage.Contains("401") || errorMessage.Contains("You must provide an api_key"))
        {
            throw new Error("Unauthorized, please make sure you are signed in.");
        }
        else
        {
            throw new Error(errorMessage);
        }
    }

    /// <summary>///</summary>
    public static Dictionary<string, object> ResolveSchemaRef(Dictionary<string, object> schema, Dictionary<string, object> spec)
    {
        if (schema.TryGetValue("$ref", out object refObj))
        {
            string refPath = refObj.ToString();
            if (refPath.StartsWith("#/components/schemas/"))
            {
                string schemaName = refPath.Split('/').Last();
                if (spec.TryGetValue("components", out object componentsObj))
                {
                    var components = componentsObj as Dictionary<string, object>;
                    if (components != null && components.TryGetValue("schemas", out object schemasObj))
                    {
                        var schemas = schemasObj as Dictionary<string, object>;
                        if (schemas != null && schemas.TryGetValue(schemaName, out object schemaObj))
                        {
                            return schemaObj as Dictionary<string, object>;
                        }
                    }
                }
            }
            else if (refPath.StartsWith("#/"))
            {
                string[] pathParts = refPath.Substring(2).Split('/');
                object current = spec;
                foreach (var part in pathParts)
                {
                    if (current is Dictionary<string, object> dict && dict.TryGetValue(part, out object next))
                    {
                        current = next;
                    }
                    else
                    {
                        break;
                    }
                }
                return current as Dictionary<string, object>;
            }
        }
        return schema;
    }
}
