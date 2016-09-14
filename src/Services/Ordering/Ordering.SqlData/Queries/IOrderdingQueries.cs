using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.SqlData.Queries
{
    //The OrderingQueries Contracts/Interfaces could be moved to a third assembly
    //We're not putting this contract in the Domain layer assembly because
    //queries/joins are just Application's needs and should not be limited 
    //to the Domain Model restrictions (Aggregates and Repositories restrictions).
    //
    //In this case we're using the same EF Context but another good approach
    //is also to simply use SQL sentences for the queries with any Micro-ORM (like Dapper) or even just ADO.NET
    //
    //The point is that Queries are IDEMPOTENT and don't need to commit to DDD Domain restrictions 
    //so could be implemented in a completely orthogonal way in regards the Domain Layer (à la CQRS)

    public interface IOrderdingQueries
    {
        Task<dynamic> GetAllOrdersIncludingValueObjectsAndChildEntities();
        Task<dynamic> GetOrderById(Guid orderId);
    }
}
