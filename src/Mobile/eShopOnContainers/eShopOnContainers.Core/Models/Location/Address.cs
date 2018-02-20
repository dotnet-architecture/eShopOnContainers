using System;

namespace eShopOnContainers.Core.Models.Location
{
    public class Address
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string FeatureName { get; set; }
        public string PostalCode { get; set; }
        public string SubLocality { get; set; }
        public string Thoroughfare { get; set; }
        public string SubThoroughfare { get; set; }
        public string Locality { get; set; }
        public string AdminArea { get; set; }
        public string SubAdminArea { get; set; }

        public Address(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            CountryCode = address.CountryCode;
            CountryName = address.CountryName;
            Latitude = address.Latitude;
            Longitude = address.Longitude;
            FeatureName = address.FeatureName;
            PostalCode = address.PostalCode;
            SubLocality = address.SubLocality;
            Thoroughfare = address.Thoroughfare;
            SubThoroughfare = address.SubThoroughfare;
            SubAdminArea = address.SubAdminArea;
            AdminArea = address.AdminArea;
        }
    }
}
