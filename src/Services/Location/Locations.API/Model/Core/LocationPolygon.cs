using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Locations.API.Model.Core
{
    public class LocationPolygon
    {
        public LocationPolygon()
        {
        }

        public LocationPolygon(List<GeoJson2DGeographicCoordinates> coordinatesList)
        {
            var coordinatesMapped = coordinatesList.Select(x => new List<double>() { x.Longitude, x.Latitude }).ToList();
            this.coordinates.Add(coordinatesMapped);
        }

        public string type { get; private set; } = "Polygon";

        public List<List<List<double>>> coordinates { get; private set; } = new List<List<List<double>>>();
    }
}
