namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.eShopOnContainers.Services.Locations.API.Model;
    using System.Text;

    public class LocationsContextSeed
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory)
        {
            var context = (LocationsContext)applicationBuilder
                .ApplicationServices.GetService(typeof(LocationsContext));

            context.Database.Migrate();

            if (!context.Locations.Any())
            {
                context.Locations.AddRange(
                    GetPreconfiguredLocations());

                await context.SaveChangesAsync();
            }
        }

        static Locations GetPreconfiguredLocations()
        {
            var ww = new Locations() { Code = "WW", Description = "WorldWide", Latitude = -1, Longitude = -1 };
            ww.ChildLocations.Add(GetUSLocations());
            return ww;
        }

        static Locations GetUSLocations()
        {
            var us = new Locations() { Code = "US", Description = "United States", Latitude = 41.650455, Longitude = -101.357386, Polygon = GetUSPoligon() };
            us.ChildLocations.Add(GetWashingtonLocations());
            return us;
        }

        static Locations GetWashingtonLocations()
        {
            var wht = new Locations() { Code = "WHT", Description = "Washington", Latitude = 47.223652, Longitude = -119.542781, Polygon = GetWashingtonPoligon() };
            wht.ChildLocations.Add(GetSeattleLocations());
            wht.ChildLocations.Add(GetRedmondLocations());
            return wht;
        }
   
        static Locations GetSeattleLocations()
        {
            var bcn = new Locations() { Code = "SEAT", Description = "Seattle", Latitude = 47.603111, Longitude = -122.330747, Polygon = GetSeattlePoligon() };
            bcn.ChildLocations.Add(new Locations() { Code = "SEAT-PioneerSqr", Description = "Seattle Pioneer Square Shop" , Latitude = 47.602053, Longitude= -122.333884, Polygon = GetSeattlePioneerSqrPoligon() });
            return bcn;
        }

        static Locations GetRedmondLocations()
        {
            var bcn = new Locations() { Code = "REDM", Description = "Redmond", Latitude = 47.674961, Longitude = -122.122887, Polygon = GetRedmondPoligon() };
            bcn.ChildLocations.Add(new Locations() { Code = "REDM-DWNTWP", Description = "Redmond Downtown Central Park Shop", Latitude = 47.674433, Longitude = -122.125006, Polygon = GetRedmondDowntownParkPoligon() });
            return bcn;
        }
   
        static List<FrontierPoints> GetUSPoligon()
        {
            var poligon = new List<FrontierPoints>();
            poligon.Add(new FrontierPoints() { Latitude = 48.7985, Longitude = -62.88205 });
            poligon.Add(new FrontierPoints() { Latitude = 48.76513, Longitude = -129.31329 });
            poligon.Add(new FrontierPoints() { Latitude = 30.12256, Longitude = -120.9496 });
            poligon.Add(new FrontierPoints() { Latitude = 30.87114, Longitude = -111.39442 });
            poligon.Add(new FrontierPoints() { Latitude = 24.24979, Longitude = -78.11975 });
            return poligon;
        }

        static List<FrontierPoints> GetWashingtonPoligon()
        {
            var poligon = new List<FrontierPoints>();
            poligon.Add(new FrontierPoints() { Latitude = 48.8943, Longitude = -124.68633 });
            poligon.Add(new FrontierPoints() { Latitude = 45.66613, Longitude = -124.32962 });
            poligon.Add(new FrontierPoints() { Latitude = 45.93384, Longitude = -116.73824 });
            poligon.Add(new FrontierPoints() { Latitude = 49.04282, Longitude = -116.96912 });
            return poligon;
        }

        static List<FrontierPoints> GetSeattlePoligon()
        {
            var poligon = new List<FrontierPoints>();
            poligon.Add(new FrontierPoints() { Latitude = 47.82929, Longitude = -122.36238 });
            poligon.Add(new FrontierPoints() { Latitude = 47.6337, Longitude = -122.42091 });
            poligon.Add(new FrontierPoints() { Latitude = 47.45224, Longitude = -122.37371 });
            poligon.Add(new FrontierPoints() { Latitude = 47.50259, Longitude = -122.20788 });
            poligon.Add(new FrontierPoints() { Latitude = 47.73644, Longitude = -122.26934 });
            return poligon;
        }

        static List<FrontierPoints> GetRedmondPoligon()
        {
            var poligon = new List<FrontierPoints>();
            poligon.Add(new FrontierPoints() { Latitude = 47.73148, Longitude = -122.15432 });
            poligon.Add(new FrontierPoints() { Latitude = 47.72559, Longitude = -122.17673 });
            poligon.Add(new FrontierPoints() { Latitude = 47.67851, Longitude = -122.16904 });
            poligon.Add(new FrontierPoints() { Latitude = 47.65036, Longitude = -122.16136 });
            poligon.Add(new FrontierPoints() { Latitude = 47.62746, Longitude = -122.15604 });
            poligon.Add(new FrontierPoints() { Latitude = 47.63463, Longitude = -122.01562 });
            poligon.Add(new FrontierPoints() { Latitude = 47.74244, Longitude = -122.04961 });
            return poligon;
        }

        static List<FrontierPoints> GetSeattlePioneerSqrPoligon()
        {
            var poligon = new List<FrontierPoints>();
            poligon.Add(new FrontierPoints() { Latitude = 47.60338, Longitude = -122.3327 });
            poligon.Add(new FrontierPoints() { Latitude = 47.60192, Longitude = -122.33665 });
            poligon.Add(new FrontierPoints() { Latitude = 47.60096, Longitude = -122.33456 });
            poligon.Add(new FrontierPoints() { Latitude = 47.60136, Longitude = -122.33186 });            
            return poligon;
        }

        static List<FrontierPoints> GetRedmondDowntownParkPoligon()
        {
            var poligon = new List<FrontierPoints>();
            poligon.Add(new FrontierPoints() { Latitude = 47.67595, Longitude = -122.12467 });
            poligon.Add(new FrontierPoints() { Latitude = 47.67449, Longitude = -122.12862 });
            poligon.Add(new FrontierPoints() { Latitude = 47.67353, Longitude = -122.12653 });
            poligon.Add(new FrontierPoints() { Latitude = 47.67368, Longitude = -122.12197 });
            return poligon;
        }
    }
}
