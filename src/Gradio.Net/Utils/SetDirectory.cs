
namespace Gradio.Net.Utils;

public class SetDirectory : System.IDisposable
{
    private readonly string _originalDirectory;

    public SetDirectory(string path)
    {
        _originalDirectory = System.IO.Directory.GetCurrentDirectory();
        System.IO.Directory.SetCurrentDirectory(path);
    }

    public void Dispose()
    {
        System.IO.Directory.SetCurrentDirectory(_originalDirectory);
    }
}
