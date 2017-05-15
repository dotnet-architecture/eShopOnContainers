using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.Sagas
{
    public abstract class Saga<TEntity> where TEntity : Entity
    {
        private readonly DbContext _dbContext;

        public Saga(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public abstract TEntity FindSagaById(int id);

        public abstract Task<bool> SaveChangesAsync();
        //{
        //    var ctx = context ?? _dbContext;
        //    var result = await ctx.SaveChangesAsync();
        //    return result > 0;
        //}
    }
}
