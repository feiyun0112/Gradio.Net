using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace Gradio.Net;

public class ImageClassificationOutputElement
{
    public string Label { get; set; }
    public double Score { get; set; }
}
