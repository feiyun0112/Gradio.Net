using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Gradio.Net.Data;

public abstract class GradioBaseModel
{
    public abstract GradioDataModel CopyToDir(string dir);

    public abstract GradioDataModel FromJson(object data);
}

