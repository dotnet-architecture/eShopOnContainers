using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Text;

namespace OcelotApiGw.Providers
{
    public class InMemoryFile : IFileInfo
    {
        private readonly byte[] _data;

        public InMemoryFile(string data) => _data = Encoding.UTF8.GetBytes(data);

        public Stream CreateReadStream() => new MemoryStream(_data);

        public bool Exists { get; } = true;

        public long Length => _data.Length;

        public string PhysicalPath { get; } = string.Empty;

        public string Name { get; } = string.Empty;

        public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;

        public bool IsDirectory { get; } = false;
    }
}
