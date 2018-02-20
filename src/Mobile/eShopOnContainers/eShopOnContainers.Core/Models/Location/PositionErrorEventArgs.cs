using System;

namespace eShopOnContainers.Core.Models.Location
{
    public class PositionErrorEventArgs : EventArgs
    {
        public GeolocationError Error { get; private set; }

        public PositionErrorEventArgs(GeolocationError error)
        {
            Error = error;
        }
    }
}
