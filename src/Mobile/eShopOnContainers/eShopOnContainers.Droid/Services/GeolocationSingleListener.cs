using Android.Locations;
using Android.OS;
using Android.Runtime;
using eShopOnContainers.Core.Models.Location;
using eShopOnContainers.Droid.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace eShopOnContainers.Droid.Services
{
    public class GeolocationSingleListener : Java.Lang.Object, ILocationListener
    {
        readonly object _locationSync = new object();
        readonly Action _finishedCallback;
        readonly float _desiredAccuracy;
        readonly Timer _timer;
        readonly TaskCompletionSource<Position> _tcs = new TaskCompletionSource<Position>();
        HashSet<string> _activeProviders = new HashSet<string>();
        Android.Locations.Location _bestLocation;

        public Task<Position> Task => _tcs.Task;

        public GeolocationSingleListener(LocationManager manager, float desiredAccuracy, int timeout, IEnumerable<string> activeProviders, Action finishedCallback)
        {
            _desiredAccuracy = desiredAccuracy;
            _finishedCallback = finishedCallback;
            _activeProviders = new HashSet<string>(activeProviders);

            foreach (var provider in activeProviders)
            {
                var location = manager.GetLastKnownLocation(provider);
                if (location != null && location.IsBetterLocation(_bestLocation))
                    _bestLocation = location;
            }

            if (timeout != Timeout.Infinite)
                _timer = new Timer(TimesUp, null, timeout, 0);
        }

        public void Cancel() => _tcs.TrySetCanceled();

        public void OnLocationChanged(Android.Locations.Location location)
        {
            if (location.Accuracy <= _desiredAccuracy)
            {
                Finish(location);
                return;
            }

            lock (_locationSync)
            {
                if (location.IsBetterLocation(_bestLocation))
                    _bestLocation = location;
            }
        }

        public void OnProviderDisabled(string provider)
        {
            lock (_activeProviders)
            {
                if (_activeProviders.Remove(provider) && _activeProviders.Count == 0)
                    _tcs.TrySetException(new GeolocationException(GeolocationError.PositionUnavailable));
            }
        }

        public void OnProviderEnabled(string provider)
        {
            lock (_activeProviders)
                _activeProviders.Add(provider);
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            switch (status)
            {
                case Availability.Available:
                    OnProviderEnabled(provider);
                    break;

                case Availability.OutOfService:
                    OnProviderDisabled(provider);
                    break;
            }
        }

        void TimesUp(object state)
        {
            lock (_locationSync)
            {
                if (_bestLocation == null)
                {
                    if (_tcs.TrySetCanceled())
                        _finishedCallback?.Invoke();
                }
                else
                {
                    Finish(_bestLocation);
                }
            }
        }

        void Finish(Android.Locations.Location location)
        {
            _finishedCallback?.Invoke();
            _tcs.TrySetResult(location.ToPosition());
        }
    }
}
