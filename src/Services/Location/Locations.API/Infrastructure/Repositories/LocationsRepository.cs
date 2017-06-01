namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class LocationsRepository
        : ILocationsRepository
    {
        private readonly LocationsContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public LocationsRepository(LocationsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public UserLocation Add(UserLocation location)
        {
            return _context.UserLocation.Add(location).Entity;

        }

        public async Task<UserLocation> GetAsync(int userId)
        {
            return await _context.UserLocation
                .Where(ul => ul.UserId == userId)
                .SingleOrDefaultAsync();
        }       

        public async Task<List<Locations>> GetNearestLocationListAsync(double lat, double lon)
        {
            var query = $"SELECT TOP(100) location.* " +
                $"FROM[dbo].[Locations] AS location " +
                $"ORDER BY [dbo].[GetDistanceFromLocation](location.Latitude, location.Longitude, " +
                $"{lat.ToString(CultureInfo.InvariantCulture)}, " +
                $"{lon.ToString(CultureInfo.InvariantCulture)})";

            return await _context.Locations.FromSql(query)
                .Include(f => f.Polygon)
                .ToListAsync();
        }

        public void Update(UserLocation location)
        {
            _context.Entry(location).State = EntityState.Modified;
        }
    }
}
