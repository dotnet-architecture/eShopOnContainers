using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.SqlData.SeedWork
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : AggregateRoot  //1:1 relationship between Repository and AggregateRoot
    {
        protected readonly DbContext _unitOfWork;

        //DbContext injected thru DI from ASP.NET Core bootstrap
        public Repository(DbContext unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            _unitOfWork = unitOfWork;
        }

        public DbContext UnitOfWork
        {
            get
            {
                return _unitOfWork;
            }
        }

        public virtual async Task<int> Add(TEntity item)
        {
            if (item == null)
                return 0;

            _unitOfWork.Set<TEntity>().Add(item); // add new item in this set

            return await _unitOfWork.SaveChangesAsync();
        }

        public virtual async Task<int> Remove(TEntity item)
        {
            if (item == null)
                return 0;

            //attach item if not exist
            _unitOfWork.Set<TEntity>().Attach(item);

            //set as "removed"
            _unitOfWork.Set<TEntity>().Remove(item);

            return await _unitOfWork.SaveChangesAsync();
        }

        public virtual async Task<int> Update(TEntity item)
        {
            if (item == null)
                return 0;

            _unitOfWork.Set<TEntity>().Update(item);

            return await _unitOfWork.SaveChangesAsync();
        }

        public virtual async Task<TEntity> Get(Guid id)
        {
            if (id != Guid.Empty)
                return await _unitOfWork.Set<TEntity>().FirstOrDefaultAsync(o => o.Id == id);
            else
                return null;
        }

        public void Dispose()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }

    }
}
