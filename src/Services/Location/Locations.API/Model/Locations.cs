namespace Microsoft.eShopOnContainers.Services.Locations.API.Model
{
    using MongoDB.Bson;
    using MongoDB.Driver.GeoJsonObjectModel;
    using System.Collections.Generic;

    public class Locations
    {
        public ObjectId Id { get; set; }
        public string Code { get; set; }
        public ObjectId Parent_Id { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }
        public GeoJsonPolygon<GeoJson2DGeographicCoordinates> Polygon { get; set; }
        public void SetLocation(double lon, double lat) => SetPosition(lon, lat);

        private void SetPosition(double lon, double lat)
        {
            Latitude = lat;
            Longitude = lon;
            Location = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                new GeoJson2DGeographicCoordinates(lon, lat));
        }
    }
}
