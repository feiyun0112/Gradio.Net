using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Gradio.Net.Data;

public class ListFiles : GradioRootModel<List<FileData>>, IEnumerable<FileData>
{
    public FileData this[int index] => Root[index];

    public IEnumerator<FileData> GetEnumerator() => Root.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
