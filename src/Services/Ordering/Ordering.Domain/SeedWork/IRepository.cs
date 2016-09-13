using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork
{
    /// <summary>
    /// Base interface to implement a "Repository Pattern", for
    /// more information about this pattern see http://martinfowler.com/eaaCatalog/repository.html
    /// or http://blogs.msdn.com/adonet/archive/2009/06/16/using-repository-and-unit-of-work-patterns-with-entity-framework-4-0.aspx
    /// </summary>
    /// <remarks>
    /// Indeed, DbSet is already a generic repository and for Data-Driven apps 
    /// you might not need custom Repositories. 
    /// But using this interface allows us to ensure PI (Persistence Ignorance) principle
    /// from the Domain and Application code
    /// </remarks>
    /// <typeparam name="TEntity">Type of entity for this repository </typeparam>
    public interface IRepository<TEntity> : IDisposable
        where TEntity : AggregateRoot  //1:1 relationship between Repository and AggregateRoot
    {
        Task<int> Add(TEntity item);
        Task<int> Remove(TEntity item);
        Task<int> Update(TEntity item);
        Task<TEntity> Get(Guid id);
    }
}
