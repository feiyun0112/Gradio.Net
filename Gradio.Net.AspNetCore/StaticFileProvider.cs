using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net.AspNetCore
{
    internal class StaticFileProvider : IFileProvider
    {
        private readonly GradioApp _gradioApp;

        internal StaticFileProvider(GradioApp gradioApp)
        {
            _gradioApp = gradioApp;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return _gradioApp.GetFileInfo(subpath, typeof(StaticFileProvider));
        }
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return null;
        }

        public IChangeToken Watch(string filter)
        {
            return null;
        }
    }
}
