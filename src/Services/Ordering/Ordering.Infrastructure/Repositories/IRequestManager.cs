using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Repositories
{
    public interface IRequestManager
    {
        Task<bool> ExistAsync(Guid id);
        Task CreateRequestForCommandAsync<T>(Guid id);
    }
}
