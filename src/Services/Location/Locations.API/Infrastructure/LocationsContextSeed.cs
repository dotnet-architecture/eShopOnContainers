namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MongoDB.Bson;
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
            await ctx.Locations.InsertOneAsync(us);        
            await SetWashingtonLocations(us.Id);
        }

        static async Task SetWashingtonLocations(ObjectId parentId)
        {
            var wht = new Locations()
            {
                Parent_Id = parentId,
                Code = "WHT",
                Description = "Washington"
            };
            wht.SetLocation(-119.542781, 47.223652);
            await ctx.Locations.InsertOneAsync(wht);
            await SetSeattleLocations(wht.Id);
            await SetRedmondLocations(wht.Id);
        }

        static async Task SetSeattleLocations(ObjectId parentId)
        {
            var stl = new Locations()
            {
                Parent_Id = parentId,
                Code = "SEAT",
                Description = "Seattle",
                Polygon = GetSeattlePoligon()
            };
            stl.SetLocation(-122.330747, 47.603111);
            await ctx.Locations.InsertOneAsync(stl);
        }

        static async Task SetRedmondLocations(ObjectId parentId)
        {
            var rdm = new Locations()
            {
                Parent_Id = parentId,
                Code = "REDM",
                Description = "Redmond",
                Polygon = GetRedmondPoligon()
            };
            rdm.SetLocation(-122.122887, 47.674961);
            await ctx.Locations.InsertOneAsync(rdm);
        }

        static async Task SetIndexes()
        {
            // Set location indexes
            var builder = Builders<Locations>.IndexKeys;
            var keys = builder.Geo2DSphere(prop => prop.Location);
            await ctx.Locations.Indexes.CreateOneAsync(keys);
        }        

        static GeoJsonPolygon<GeoJson2DGeographicCoordinates> GetSeattlePoligon()
        {
            return new GeoJsonPolygon<GeoJson2DGeographicCoordinates>(new GeoJsonPolygonCoordinates<GeoJson2DGeographicCoordinates>(
                 new GeoJsonLinearRingCoordinates<GeoJson2DGeographicCoordinates>(
                     new List<GeoJson2DGeographicCoordinates>()
                     {
                        new GeoJson2DGeographicCoordinates(-122.36238,47.82929),
                        new GeoJson2DGeographicCoordinates(-122.42091,47.6337),
                        new GeoJson2DGeographicCoordinates(-122.37371,47.45224),
                        new GeoJson2DGeographicCoordinates(-122.20788,47.50259),
                        new GeoJson2DGeographicCoordinates(-122.26934,47.73644),
                        new GeoJson2DGeographicCoordinates(-122.36238,47.82929)
                     })));
        }

        static GeoJsonPolygon<GeoJson2DGeographicCoordinates> GetRedmondPoligon()
        {
            return new GeoJsonPolygon<GeoJson2DGeographicCoordinates>(new GeoJsonPolygonCoordinates<GeoJson2DGeographicCoordinates>(
                 new GeoJsonLinearRingCoordinates<GeoJson2DGeographicCoordinates>(
                     new List<GeoJson2DGeographicCoordinates>()
                     {
                        new GeoJson2DGeographicCoordinates(47.73148, -122.15432),
                        new GeoJson2DGeographicCoordinates(47.72559, -122.17673),
                        new GeoJson2DGeographicCoordinates(47.67851, -122.16904),
                        new GeoJson2DGeographicCoordinates(47.65036, -122.16136),
                        new GeoJson2DGeographicCoordinates(47.62746, -122.15604),
                        new GeoJson2DGeographicCoordinates(47.63463, -122.01562),
                        new GeoJson2DGeographicCoordinates(47.74244, -122.04961),
                        new GeoJson2DGeographicCoordinates(47.73148, -122.15432),
                     })));
        }       
    }
}
