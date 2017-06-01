using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Locations.API.Model
{
    public class UserLocation
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public UserLocation()
        {
        }

        public UserLocation(int userId) : this()
        {
            UserId = userId;
        }

        public Locations Location { get; set; }
    }
}
