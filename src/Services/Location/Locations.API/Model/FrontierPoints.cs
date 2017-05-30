namespace Microsoft.eShopOnContainers.Services.Locations.API.Model
{
    public class FrontierPoints
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Locations Location { get; set; }
    }
}