using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace Gradio.Net.Core;

public static class BrotliMode
{
    public const int Generic = 0;
    public const int Text = 1;
    public const int Font = 2;
}

