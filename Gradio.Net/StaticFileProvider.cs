using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.Primitives;
using System.Reflection;
using System.Text;

namespace Gradio.Net
{
    internal class StaticFileProvider : IFileProvider
    {
        private static readonly char[] _pathSeparators = new[]
            {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};
        private readonly string _rootPath;
        private readonly Assembly? _assembly;
        private readonly DateTimeOffset _lastModified;
        private readonly string? _baseNamespace;

        internal StaticFileProvider(string rootPath) { 
            this._rootPath = rootPath;
            this._assembly = Assembly.GetAssembly(typeof(StaticFileProvider));
            this._lastModified= DateTimeOffset.UtcNow;
            this._baseNamespace = typeof(StaticFileProvider).Namespace;
        }

        public IFileInfo GetFileInfo(string subpath)
        {            
            subpath =_rootPath+"/"+ subpath.TrimStart(_pathSeparators);

            if (Path.IsPathRooted(subpath))
            {
                return new NotFoundFileInfo(subpath);
            }

            if (string.IsNullOrEmpty(subpath))
            {
                return new NotFoundFileInfo(subpath);
            }

            var builder = new StringBuilder(_baseNamespace.Length + subpath.Length);
            builder.Append(_baseNamespace);

            if (subpath.StartsWith("/", StringComparison.Ordinal))
            {
                subpath = subpath.Substring(1, subpath.Length - 1);
            }

            builder.Append("."+subpath.Replace("/","."));

            var resourcePath = builder.ToString();
            
            var name = Path.GetFileName(subpath);
            if (_assembly.GetManifestResourceInfo(resourcePath) == null)
            {
                return new NotFoundFileInfo(name);
            }

            return new EmbeddedResourceFileInfo(_assembly, resourcePath, name, _lastModified); ;
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
