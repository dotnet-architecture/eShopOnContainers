namespace Microsoft.eShopOnContainers.Services.Locations.API.Model
{
    using global::Locations.API.Model.Core;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver.GeoJsonObjectModel;
    using System.Collections.Generic;

    public class Locations
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int LocationId { get; set; }
        public string Code { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string Parent_Id { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public LocationPoint Location { get; private set; }
        public LocationPolygon Polygon { get; private set; }

        // Temporal commented in previewVersion7 of netcore and 2.9.0-beta2 of Mongo packages, review in next versions
        // public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; private set; }
        // public GeoJsonPolygon<GeoJson2DGeographicCoordinates> Polygon { get; private set; }
        public void SetLocation(double lon, double lat) => SetPosition(lon, lat);
        public void SetArea(List<GeoJson2DGeographicCoordinates> coordinatesList) => SetPolygon(coordinatesList);

        private void SetPosition(double lon, double lat)
        {
            Latitude = lat;
            Longitude = lon;
            Location = new LocationPoint(lon, lat);
        }

        private void SetPolygon(List<GeoJson2DGeographicCoordinates> coordinatesList)
        {
            Polygon = new LocationPolygon(coordinatesList);
        }
    }
}
