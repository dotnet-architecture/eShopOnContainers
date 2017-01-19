namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Repositories
{
    using Domain.SeedWork;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.RepositoryContracts;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class BuyerRepository
        : IBuyerRepository
    {
        private readonly OrderingContext _context;

        public BuyerRepository(OrderingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public Buyer Add(Buyer buyer)
        {
            return _context.Buyers
                .Add(buyer)
                .Entity;
        }

        public async Task<Buyer> FindAsync(string BuyerIdentityGuid)
        {
            var buyer = await _context.Buyers
                .Include(b => b.Payments)
                .Where(b => b.FullName == BuyerIdentityGuid)
                .SingleOrDefaultAsync();

            return buyer;
        }
    }
}
