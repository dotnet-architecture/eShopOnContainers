using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Microsoft.eShopOnContainers.Services.Locations.API.Model
{
    public class Locations
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int? ParentId { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsPointInPolygon(double lat, double lon) => CheckIsPointInPolygon(lat, lon);

        public Locations()
        {
            ChildLocations = new List<Locations>();
        }

        public virtual List<FrontierPoints> Polygon { get; set; }

        [ForeignKey("ParentId")]
        public virtual List<Locations> ChildLocations { get; set; }


        private bool CheckIsPointInPolygon(double lat, double lon)
        {
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
