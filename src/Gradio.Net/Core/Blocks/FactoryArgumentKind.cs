namespace Gradio.Net;
/// <summary>
/// The shapes the Python version's constructors accept for a parameter that is sometimes one thing
/// and sometimes several.
/// </summary>
/// <remarks>
/// Each of these is read by one method of <c>Values</c> in the compatibility layer, once, so that
/// the reading is the same for every factory that has such a parameter.
/// </remarks>
internal enum FactoryArgumentKind
{
    /// <summary>One item, or a sequence of them: the Python version's <c>inputs</c>.</summary>
    OneOrMany,

    /// <summary>One name, or a sequence of names: the Python version's <c>elem_classes</c>,
    /// <c>preserved_by_key</c> and <c>sources</c>.</summary>
    Names,
}
