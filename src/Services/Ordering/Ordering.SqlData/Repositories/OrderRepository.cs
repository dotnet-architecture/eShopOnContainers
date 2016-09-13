using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.eShopOnContainers.Services.Ordering.SqlData.SeedWork;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.RepositoryContracts;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.SqlData.Repositories
{
    //1:1 relationship between Repository and Aggregate (i.e. OrderRepository and Order)
    public class OrderRepository
    : Repository<Order>, IOrderRepository
    {
        public OrderRepository(OrderingDbContext unitOfWork)
            : base(unitOfWork) { }

        //TBD - To define Specific Actions Not In Base Repository class

        public async Task<int> Remove(Guid orderId)
        {
            if (orderId == null)
                return 0;

            Order orderToDelete = await this.Get(orderId);


            //attach item if not exist
            _unitOfWork.Attach(orderToDelete);

            //set as "removed"
            _unitOfWork.Remove(orderToDelete);

            return await _unitOfWork.SaveChangesAsync();
        }
    }

}
