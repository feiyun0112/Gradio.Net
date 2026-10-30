using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A model that goes over the wire as a document: the base of the types whose shape is the Python
/// version's and whose field names are the protocol.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>GradioModel</c> is a pydantic model with two things added — a way to
/// read one back (<c>from_json</c>) and a way to copy the files it names into a directory
/// (<c>copy_to_dir</c>). Here it is the first of those three things: <see cref="ToJson"/> is what
/// the protocol carries, and it is declared once so that "this type is a document" is a statement
/// rather than a habit.
/// </para>
/// <para>
/// The other two are not on the base, and deliberately. Reading a model back is a static method
/// with the model's own field list, so a base could only declare it as a static abstract member —
/// which would make every implementer pay for a call nothing makes. Copying a model's files is
/// <see cref="MediaFiles"/>'s job, because it needs the application's cache folder and the model
/// does not know one exists.
/// </para>
/// <para>
/// What derives from this is what the framework sends and receives as a model:
/// <see cref="FileData"/> today. The documents that are only ever produced — the config, the API
/// description, the queue's messages — are <see cref="JsonObject"/>s, because nothing reads them
/// back and a type per document would be a type per the Python version dictionary to keep in step.
/// </para>
/// </remarks>
public abstract class GradioModel
{
    /// <summary>The model as the protocol carries it.</summary>
    /// <returns>
    /// The document, with the Python version's field names, in the Python version's order, and with
    /// the nulls written rather than omitted where the Python version writes them.
    /// </returns>
    public abstract JsonObject ToJson();
}
