namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Repositories;

public class BuyerRepository
    : IBuyerRepository
{
    private readonly OrderingContext _context;
    public IUnitOfWork UnitOfWork
    {
        get
        {
            return _context;
        }
    }

    public BuyerRepository(OrderingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Buyer Add(Buyer buyer)
    {
        if (buyer.IsTransient())
        {
            return _context.Buyers
                .Add(buyer)
                .Entity;
        }
        else
        {
            return buyer;
        }
    }

    public Buyer Update(Buyer buyer)
    {
        return _context.Buyers
                .Update(buyer)
                .Entity;
    }

    public async Task<Buyer> FindAsync(string identity)
    {
        var buyer = await _context.Buyers
            .Include(b => b.PaymentMethods)
            .Where(b => b.IdentityGuid == identity)
            .SingleOrDefaultAsync();

        return buyer;
    }

    public async Task<Buyer> FindByIdAsync(string id)
    {
        var buyer = await _context.Buyers
            .Include(b => b.PaymentMethods)
            .Where(b => b.Id == int.Parse(id))
            .SingleOrDefaultAsync();

        return buyer;
    }
}
