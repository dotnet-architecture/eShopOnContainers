using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Repositories
{
    public class RequestManager : IRequestManager
    {
        private readonly OrderingContext _context;
        public RequestManager(OrderingContext ctx)
        {
            _context = ctx;
        }


        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context.FindAsync<ClientRequest>(id);
            return request != null;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id)
        {

            var exists = await ExistAsync(id);
            var request = exists ? 
                throw new Exception($"Request with {id} already exists") : 
                new ClientRequest()
                {
                    Id = id,
                    Name = typeof(T).Name,
                    Time = DateTime.UtcNow
                };

            _context.Add(request);
            await _context.SaveChangesAsync();
        }

    }
}
