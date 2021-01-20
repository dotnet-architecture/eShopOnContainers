using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Locations.API.Model.Core
{
    public class LocationPoint
    {
        public LocationPoint()
        {
        }

        public LocationPoint(double longitude, double latitude)
        {
            this.coordinates.Add(longitude);
            this.coordinates.Add(latitude);
        }

        public string type { get; private set; } = "Point";

        public List<double> coordinates { get; private set; } = new List<double>();
    }
}
