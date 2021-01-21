namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Exceptions
{
    using System;

    /// <summary>
    /// Exception type for app exceptions
    /// </summary>
    public class LocationDomainException : Exception
    {
        public LocationDomainException()
        { }

        public LocationDomainException(string message)
            : base(message)
        { }

        public LocationDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
