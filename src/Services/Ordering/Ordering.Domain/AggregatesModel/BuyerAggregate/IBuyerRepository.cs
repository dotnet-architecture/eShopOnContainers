namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;

//This is just the RepositoryContracts or Interface defined at the Domain Layer
//as requisite for the Buyer Aggregate

public interface IBuyerRepository : IRepository<Buyer>
{
    Buyer Add(Buyer buyer);
    Buyer Update(Buyer buyer);
    Task<Buyer> FindAsync(string BuyerIdentityGuid);
    Task<Buyer> FindByIdAsync(string id);
}

