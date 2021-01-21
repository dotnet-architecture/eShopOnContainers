using System;

namespace eShopOnContainers.Core.Models.Location
{
    public class GeolocationException : Exception
    {
        public GeolocationError Error { get; private set; }

        public GeolocationException(GeolocationError error)
            : base("A geolocation error occured: " + error)
        {
            if (!Enum.IsDefined(typeof(GeolocationError), error))
                throw new ArgumentException("error is not a valid GelocationError member", "error");

            Error = error;
        }

        public GeolocationException(GeolocationError error, Exception innerException)
            : base("A geolocation error occured: " + error, innerException)
        {
            if (!Enum.IsDefined(typeof(GeolocationError), error))
                throw new ArgumentException("error is not a valid GelocationError member", "error");

            Error = error;
        }
    }
}
