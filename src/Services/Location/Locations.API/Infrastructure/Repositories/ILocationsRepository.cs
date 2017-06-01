namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Repositories
{
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILocationsRepository : IRepository
    {
        UserLocation Add(UserLocation order);

        void Update(UserLocation order);

        Task<UserLocation> GetAsync(int userId);

        Task<List<Locations>> GetNearestLocationListAsync(double lat, double lon);
    }
}
