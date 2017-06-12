namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;
    using MongoDB.Driver.GeoJsonObjectModel;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class LocationsContextSeed
    {
        private static LocationsContext ctx;
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory)
        {
            var config = applicationBuilder
                .ApplicationServices.GetRequiredService<IOptions<LocationSettings>>();

            ctx = new LocationsContext(config);

            if (!ctx.Locations.Database.GetCollection<Locations>(nameof(Locations)).AsQueryable().Any())
            {
                await SetIndexes();
                await SetUSLocations();
            }
        }

        static async Task SetUSLocations()
        {
            var us = new Locations()
            {
                Code = "US",
                Description = "United States"
            };
            us.SetLocation(-101.357386, 41.650455);
            us.SetArea(GetUSPoligon());
            await ctx.Locations.InsertOneAsync(us);
            await SetWashingtonLocations(us.Id);
        }

        static async Task SetWashingtonLocations(string parentId)
        {
            var wht = new Locations()
            {
                Parent_Id = parentId,
                Code = "WHT",
                Description = "Washington"
            };
            wht.SetLocation(-119.542781, 47.223652);
            wht.SetArea(GetWashingtonPoligon());
            await ctx.Locations.InsertOneAsync(wht);
            await SetSeattleLocations(wht.Id);
            await SetRedmondLocations(wht.Id);
        }

        static async Task SetSeattleLocations(string parentId)
        {
            var stl = new Locations()
            {
                Parent_Id = parentId,
                Code = "SEAT",
                Description = "Seattle"
            };
            stl.SetArea(GetSeattlePoligon());
            stl.SetLocation(-122.330747, 47.603111);
            await ctx.Locations.InsertOneAsync(stl);
        }

        static async Task SetRedmondLocations(string parentId)
        {
            var rdm = new Locations()
            {
                Parent_Id = parentId,
                Code = "REDM",
                Description = "Redmond"
            };
            rdm.SetLocation(-122.122887, 47.674961);
            rdm.SetArea(GetRedmondPoligon());
            await ctx.Locations.InsertOneAsync(rdm);
        }

        static async Task SetIndexes()
        {
            // Set location indexes
            var builder = Builders<Locations>.IndexKeys;
            var keys = builder.Geo2DSphere(prop => prop.Location);
            await ctx.Locations.Indexes.CreateOneAsync(keys);
        }

        static List<GeoJson2DGeographicCoordinates> GetUSPoligon()
        {
            return new List<GeoJson2DGeographicCoordinates>()
                     {
                        new GeoJson2DGeographicCoordinates(-62.88205, 48.7985),
                        new GeoJson2DGeographicCoordinates(-129.3132, 48.76513),
                        new GeoJson2DGeographicCoordinates(-120.9496, 30.12256),
                        new GeoJson2DGeographicCoordinates(-111.3944, 30.87114),
                        new GeoJson2DGeographicCoordinates(-78.11975, 24.24979),
                        new GeoJson2DGeographicCoordinates(-62.88205, 48.7985)
                     };
        }

        static List<GeoJson2DGeographicCoordinates> GetSeattlePoligon()
        {
            return new List<GeoJson2DGeographicCoordinates>()
                     {
                        new GeoJson2DGeographicCoordinates(-122.36238,47.82929),
                        new GeoJson2DGeographicCoordinates(-122.42091,47.6337),
                        new GeoJson2DGeographicCoordinates(-122.37371,47.45224),
                        new GeoJson2DGeographicCoordinates(-122.20788,47.50259),
                        new GeoJson2DGeographicCoordinates(-122.26934,47.73644),
                        new GeoJson2DGeographicCoordinates(-122.36238,47.82929)
                     };
        }

        static List<GeoJson2DGeographicCoordinates> GetRedmondPoligon()
        {
            return new List<GeoJson2DGeographicCoordinates>()
                     {
                        new GeoJson2DGeographicCoordinates(-122.15432, 47.73148),
                        new GeoJson2DGeographicCoordinates(-122.17673, 47.72559),
                        new GeoJson2DGeographicCoordinates(-122.16904, 47.67851),
                        new GeoJson2DGeographicCoordinates(-122.16136, 47.65036),
                        new GeoJson2DGeographicCoordinates(-122.15604, 47.62746),
                        new GeoJson2DGeographicCoordinates(-122.01562, 47.63463),
                        new GeoJson2DGeographicCoordinates(-122.04961, 47.74244),
                        new GeoJson2DGeographicCoordinates(-122.15432, 47.73148)
                     };
        }

        static List<GeoJson2DGeographicCoordinates> GetWashingtonPoligon()
        {
            return new List<GeoJson2DGeographicCoordinates>()
                     {
                        new GeoJson2DGeographicCoordinates(-124.68633, 48.8943),
                        new GeoJson2DGeographicCoordinates(-124.32962, 45.66613),
                        new GeoJson2DGeographicCoordinates(-116.73824, 45.93384),
                        new GeoJson2DGeographicCoordinates(-116.96912, 49.04282),
                        new GeoJson2DGeographicCoordinates(-124.68633, 48.8943)
                     };
        }
    }
}
