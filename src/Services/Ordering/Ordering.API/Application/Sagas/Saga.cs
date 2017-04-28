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

        protected TEntity FindSagaById(int id, DbContext context = null)
        {
            var ctx = context ?? _dbContext;
            return ctx.Set<TEntity>().Where(x => x.Id == id).SingleOrDefault();
        }

        protected async Task<bool> SaveChangesAsync(DbContext context = null)
        {
            var ctx = context ?? _dbContext;
            var result = await ctx.SaveChangesAsync();
            return result > 0;
        }
    }
}
