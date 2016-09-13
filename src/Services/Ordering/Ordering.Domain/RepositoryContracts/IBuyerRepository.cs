using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.RepositoryContracts
{
    public interface IBuyerRepository : IRepository<Order>
    {
        //TBD - To define Specific Actions Not In Base Repo
    }
}
