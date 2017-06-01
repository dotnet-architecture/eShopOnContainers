using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Microsoft.eShopOnContainers.Services.Locations.API.Model
{
    public class Locations
    {
        public int Id { get; private set; }
        public string Code { get; private set; }
        public int? ParentId { get; private set; }
        public string Description { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public (Locations location, bool isSuccess) GetUserMostSpecificLocation(double userLatitude, double userLongitude) 
            => CheckUserMostSpecificLocation(userLatitude, userLongitude);

        public Locations()
        {
            ChildLocations = new List<Locations>();
        }

        public Locations(string code, string description, double latitude, double longitude, List<FrontierPoints> polygon = null) : this()
        {
            Code = code;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
            Polygon = polygon ?? new List<FrontierPoints>();
        }

        public virtual List<FrontierPoints> Polygon { get; set; }

        [ForeignKey("ParentId")]
        public virtual List<Locations> ChildLocations { get; set; }

        private (Locations location, bool isSuccess) CheckUserMostSpecificLocation(double userLatitude, double userLongitude, Locations location = null)
        {
            Locations result = this;
            var childLocations = location != null ? location.ChildLocations : ChildLocations;

            // Check if user is in location's area, if not then returns false
            if (!CheckIsPointInPolygon(userLatitude, userLongitude))
            {
                return (this, false);
            }

            foreach (var childLocation in childLocations)
            {
                result = childLocation;

                if (childLocation.ChildLocations.Count == 0){ break; }

                CheckUserMostSpecificLocation(userLatitude, userLongitude, childLocation);
            }
            return (result, true);
        }

        private bool CheckIsPointInPolygon(double lat, double lon)
        {
            if(Polygon.Count == 0) { return false; };
            double minX = Polygon[0].Latitude;
            double maxX = Polygon[0].Latitude;
            double minY = Polygon[0].Longitude;
            double maxY = Polygon[0].Longitude;
            for (int i = 1; i < Polygon.Count; i++)
            {
                FrontierPoints q = Polygon[i];
                minX = Math.Min(q.Latitude, minX);
                maxX = Math.Max(q.Latitude, maxX);
                minY = Math.Min(q.Longitude, minY);
                maxY = Math.Max(q.Longitude, maxY);
            }

            if (lat < minX || lat > maxX || lon < minY || lon > maxY)
            {
                return false;
            }

            bool inside = false;
            for (int i = 0, j = Polygon.Count - 1; i < Polygon.Count; j = i++)
            {
                if ((Polygon[i].Longitude > lon) != (Polygon[j].Latitude > lat) &&
                     lat < (Polygon[j].Longitude - Polygon[i].Latitude) * (lon - Polygon[i].Longitude) / (Polygon[j].Longitude - Polygon[i].Longitude) + Polygon[i].Latitude)
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}
