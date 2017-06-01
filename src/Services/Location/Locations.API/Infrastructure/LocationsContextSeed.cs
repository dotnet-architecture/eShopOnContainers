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
            var ww = new Locations("WW", "WorldWide", -1, -1);
            ww.ChildLocations.Add(GetUSLocations());
            return ww;
        }

        static Locations GetUSLocations()
        {
            var us = new Locations("US", "United States", 41.650455, -101.357386, GetUSPoligon());
            us.ChildLocations.Add(GetWashingtonLocations());
            return us;
        }

        static Locations GetWashingtonLocations()
        {
            var wht = new Locations("WHT", "Washington", 47.223652, -119.542781, GetWashingtonPoligon());
            wht.ChildLocations.Add(GetSeattleLocations());
            wht.ChildLocations.Add(GetRedmondLocations());
            return wht;
        }
   
        static Locations GetSeattleLocations()
        {
            var bcn = new Locations("SEAT", "Seattle", 47.603111, -122.330747, GetSeattlePoligon());
            bcn.ChildLocations.Add(new Locations("SEAT-PioneerSqr", "Seattle Pioneer Square Shop", 47.602053, -122.333884, GetSeattlePioneerSqrPoligon()));
            return bcn;
        }

        static Locations GetRedmondLocations()
        {
            var bcn = new Locations("REDM", "Redmond", 47.674961, -122.122887, GetRedmondPoligon());
            bcn.ChildLocations.Add(new Locations("REDM-DWNTWP", "Redmond Downtown Central Park Shop", 47.674433, -122.125006, GetRedmondDowntownParkPoligon()));
            return bcn;
        }
   
        static List<FrontierPoints> GetUSPoligon()
        {
            var poligon = new List<FrontierPoints>();
            poligon.Add(new FrontierPoints(48.7985, -62.88205));
            poligon.Add(new FrontierPoints(48.76513, -129.3132));
            poligon.Add(new FrontierPoints(30.12256, -120.9496));
            poligon.Add(new FrontierPoints(30.87114, -111.3944));
            poligon.Add(new FrontierPoints(24.24979, -78.11975));
            return poligon;
        }

        static List<FrontierPoints> GetWashingtonPoligon()
        {
            var poligon = new List<FrontierPoints>();
            poligon.Add(new FrontierPoints(48.8943,  -124.68633));
            poligon.Add(new FrontierPoints(45.66613, -124.32962));
            poligon.Add(new FrontierPoints(45.93384, -116.73824));
            poligon.Add(new FrontierPoints(49.04282, -116.96912));
            return poligon;
        }

        static List<FrontierPoints> GetSeattlePoligon()
        {
            var poligon = new List<FrontierPoints>();
            poligon.Add(new FrontierPoints(47.82929, -122.36238));
            poligon.Add(new FrontierPoints(47.6337, -122.42091));
            poligon.Add(new FrontierPoints(47.45224, -122.37371));
            poligon.Add(new FrontierPoints(47.50259, -122.20788));
            poligon.Add(new FrontierPoints(47.73644, -122.26934));
            return poligon;
        }

        static List<FrontierPoints> GetRedmondPoligon()
        {
            var poligon = new List<FrontierPoints>();
            poligon.Add(new FrontierPoints(47.73148, -122.15432));
            poligon.Add(new FrontierPoints(47.72559, -122.17673));
            poligon.Add(new FrontierPoints(47.67851, -122.16904));
            poligon.Add(new FrontierPoints(47.65036, -122.16136));
            poligon.Add(new FrontierPoints(47.62746, -122.15604));
            poligon.Add(new FrontierPoints(47.63463, -122.01562));
            poligon.Add(new FrontierPoints(47.74244, -122.04961));
            return poligon;
        }

        static List<FrontierPoints> GetSeattlePioneerSqrPoligon()
        {
            var poligon = new List<FrontierPoints>();
            poligon.Add(new FrontierPoints(47.60338, -122.3327));
            poligon.Add(new FrontierPoints(47.60192, -122.33665));
            poligon.Add(new FrontierPoints(47.60096, -122.33456));
            poligon.Add(new FrontierPoints(47.60136, -122.33186));            
            return poligon;
        }

        static List<FrontierPoints> GetRedmondDowntownParkPoligon()
        {
            var poligon = new List<FrontierPoints>();
            poligon.Add(new FrontierPoints(47.67595, -122.12467));
            poligon.Add(new FrontierPoints(47.67449, -122.12862));
            poligon.Add(new FrontierPoints(47.67353, -122.12653));
            poligon.Add(new FrontierPoints(47.67368, -122.12197));
            return poligon;
        }
    }
}
