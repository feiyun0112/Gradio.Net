using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Gradio.Net.Core;

public class Request
{
    public HttpRequest HttpRequest { get; set; }
    public string Username { get; set; }
    public string SessionHash { get; set; }
    public Dictionary<string, object> Kwargs { get; set; }

    public Request(HttpRequest request = null, string username = null, string sessionHash = null, Dictionary<string, object> kwargs = null)
    {
        HttpRequest = request;
        Username = username;
        SessionHash = sessionHash;
        Kwargs = kwargs ?? new Dictionary<string, object>();
    }

    public T GetAttribute<T>(string name)
    {
        if (HttpRequest != null)
        {
            var propertyInfo = typeof(HttpRequest).GetProperty(name);
            if (propertyInfo != null)
            {
                var value = propertyInfo.GetValue(HttpRequest);
                if (value is T typedValue)
                {
                    return typedValue;
                }
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        if (Kwargs.TryGetValue(name, out var kwargsValue))
        {
            if (kwargsValue is T typedValue)
            {
                return typedValue;
            }
            return (T)Convert.ChangeType(kwargsValue, typeof(T));
        }

        throw new InvalidOperationException($"Request object has no attribute '{name}'");
    }

    public Dictionary<string, string> Headers
    {
        get
        {
            if (HttpRequest != null)
            {
                return HttpRequest.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            }
            return Kwargs.TryGetValue("headers", out var headers) ? (Dictionary<string, string>)headers : new Dictionary<string, string>();
        }
    }

    public Dictionary<string, string> QueryParams
    {
        get
        {
            if (HttpRequest != null)
            {
                return HttpRequest.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
            }
            return Kwargs.TryGetValue("query_params", out var queryParams) ? (Dictionary<string, string>)queryParams : new Dictionary<string, string>();
        }
    }

    public Dictionary<string, string> Cookies
    {
        get
        {
            if (HttpRequest != null)
            {
                return HttpRequest.Cookies.ToDictionary(c => c.Key, c => c.Value);
            }
            return Kwargs.TryGetValue("cookies", out var cookies) ? (Dictionary<string, string>)cookies : new Dictionary<string, string>();
        }
    }

    public Dictionary<string, string> PathParams
    {
        get
        {
            if (HttpRequest != null)
            {
                return HttpRequest.RouteValues.ToDictionary(r => r.Key, r => r.Value?.ToString() ?? "");
            }
            return Kwargs.TryGetValue("path_params", out var pathParams) ? (Dictionary<string, string>)pathParams : new Dictionary<string, string>();
        }
    }

    public object Client
    {
        get
        {
            if (HttpRequest != null)
            {
                var connectionInfo = HttpRequest.HttpContext.Connection;
                return new { Host = connectionInfo.RemoteIpAddress?.ToString(), Port = connectionInfo.RemotePort };
            }
            return Kwargs.TryGetValue("client", out var client) ? client : null;
        }
    }

    public string Url
    {
        get
        {
            if (HttpRequest != null)
            {
                return HttpRequest.GetDisplayUrl();
            }
            return Kwargs.TryGetValue("url", out var url) ? url.ToString() : "";
        }
    }

    public Dictionary<string, object> ToDict()
    {
        var dict = new Dictionary<string, object>(Kwargs);
        dict["headers"] = Headers;
        dict["query_params"] = QueryParams;
        dict["cookies"] = Cookies;
        dict["path_params"] = PathParams;
        dict["client"] = Client;
        dict["url"] = Url;
        dict["username"] = Username;
        dict["session_hash"] = SessionHash;
        return dict;
    }
}
