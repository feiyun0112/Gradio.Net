using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A value a block carries that says for itself how it is written to the config.
/// </summary>
/// <remarks>
/// <para>
/// Most props are scalars and lists of strings, which <see cref="PropWriter"/> writes directly. A
/// few are structured — a file's path and metadata, the options of a webcam — and the Python
/// version writes them by serializing a dataclass. This is that seam: the type says how it becomes
/// JSON, and the writer does not have to know about it. One value that is not structured uses it
/// too — <see cref="Visibility"/>, which is written as a boolean or as the word <c>hidden</c> —
/// because the writer would otherwise need a case for each such type as it arrived.
/// </para>
/// <para>
/// It is an interface rather than a case in the writer's switch because the types live in different
/// layers: <see cref="FileData"/> is core data, while the option types belong to the components that
/// use them, and the writer may not reference those.
/// </para>
/// </remarks>
public interface IConfigValue
{
    /// <summary>The JSON the config carries for this value.</summary>
    /// <returns>A new node; the caller may keep or mutate it.</returns>
    JsonNode ToConfigValue();
}
