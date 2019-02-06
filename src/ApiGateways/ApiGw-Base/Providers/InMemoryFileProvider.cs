using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace OcelotApiGw.Providers
{
    public class InMemoryFileProvider : IFileProvider
    {
        private readonly IFileInfo _fileInfo;

        public InMemoryFileProvider(string data) => _fileInfo = new InMemoryFile(data);

        public IFileInfo GetFileInfo(string subpath) => _fileInfo;

        public IDirectoryContents GetDirectoryContents(string subpath) => null;

        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }
}
