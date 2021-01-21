using eShopOnContainers.Core.Models.Location;
using System;

namespace eShopOnContainers.Droid.Extensions
{
    public static class LocationExtensions
    {
        static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        static int TwoMinutes = 120000;

        internal static Position ToPosition(this Android.Locations.Location location)
        {
            var p = new Position();
            if (location.HasAccuracy)
                p.Accuracy = location.Accuracy;
            if (location.HasAltitude)
                p.Altitude = location.Altitude;
            if (location.HasBearing)
                p.Heading = location.Bearing;
            if (location.HasSpeed)
                p.Speed = location.Speed;

            p.Longitude = location.Longitude;
            p.Latitude = location.Latitude;
            p.Timestamp = location.GetTimestamp();
            return p;
        }

        internal static DateTimeOffset GetTimestamp(this Android.Locations.Location location)
        {
            try
            {
                return new DateTimeOffset(Epoch.AddMilliseconds(location.Time));
            }
            catch (Exception)
            {
                return new DateTimeOffset(Epoch);
            }
        }

        internal static bool IsBetterLocation(this Android.Locations.Location location, Android.Locations.Location bestLocation)
        {

            if (bestLocation == null)
                return true;

            var timeDelta = location.Time - bestLocation.Time;
            var isSignificantlyNewer = timeDelta > TwoMinutes;
            var isSignificantlyOlder = timeDelta < -TwoMinutes;
            var isNewer = timeDelta > 0;

            if (isSignificantlyNewer)
                return true;

            if (isSignificantlyOlder)
                return false;

            var accuracyDelta = (int)(location.Accuracy - bestLocation.Accuracy);
            var isLessAccurate = accuracyDelta > 0;
            var isMoreAccurate = accuracyDelta < 0;
            var isSignificantlyLessAccurage = accuracyDelta > 200;
            var isFromSameProvider = IsSameProvider(location.Provider, bestLocation.Provider);

            if (isMoreAccurate)
                return true;

            if (isNewer && !isLessAccurate)
                return true;

            if (isNewer && !isSignificantlyLessAccurage && isFromSameProvider)
                return true;

            return false;
        }

        internal static bool IsSameProvider(string provider1, string provider2)
        {
            if (provider1 == null)
                return provider2 == null;

            return provider1.Equals(provider2);
        }
    }
}
