using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webhooks.API.Services
{
    public interface IGrantUrlTesterService
    {
        Task<bool> TestGrantUrl(string urlHook, string url, string token);
    }
}
