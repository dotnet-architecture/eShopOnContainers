namespace Microsoft.eShopOnContainers.Services.Locations.API.Model
{
    public class FrontierPoints
    {
        public int Id { get; set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public FrontierPoints()
        {
        }

        public FrontierPoints(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Locations Location { get; private set; }
    }
}