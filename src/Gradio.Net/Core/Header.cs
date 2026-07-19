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

public class Header
{
    public string Value { get; }

    public Header(string value)
    {
        Value = value;
    }

    public static implicit operator string(Header header) => header.Value;

    public static implicit operator Header(string value) => new Header(value);

    public override string ToString() => Value;
}
