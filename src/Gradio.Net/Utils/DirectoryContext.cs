namespace Gradio.Net.Utils;

public class DirectoryContext : IDisposable
{
    private readonly string _originalDirectory;

    public DirectoryContext(string path)
    {
        _originalDirectory = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(path);
    }

    public void Dispose()
    {
        Directory.SetCurrentDirectory(_originalDirectory);
    }
}
