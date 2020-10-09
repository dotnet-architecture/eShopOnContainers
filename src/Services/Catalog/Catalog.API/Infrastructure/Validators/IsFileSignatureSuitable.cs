using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Validators
{
    public class IsFileSignatureSuitable : ISpecification<IFormFile>
    {
        public bool IsSatisfiedBy(IFormFile file)
        {
            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                foreach (KeyValuePair<string, List<byte[]>> key in _fileSignature)
                {
                    var headerBytes = reader.ReadBytes(key.Value.Max(m => m.Length));
                    reader.BaseStream.Position=0;
                    if(key.Value.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature))) return true;
                }
            }
            return false;
        }

        private static readonly Dictionary<string, List<byte[]>> _fileSignature = new Dictionary<string, List<byte[]>>
        {
            { ".jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                }
            },

            { ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                }
            },

            { ".png", new List<byte[]>
                {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                }
            }
        };
    }
}