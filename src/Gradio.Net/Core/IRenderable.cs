namespace Gradio.Net.Core;

public interface IRenderable
{
    int Id { get; }

    object Apply(params object[] args);
}
