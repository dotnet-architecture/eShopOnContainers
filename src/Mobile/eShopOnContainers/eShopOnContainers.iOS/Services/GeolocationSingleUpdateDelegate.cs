using CoreLocation;
using eShopOnContainers.Core.Models.Location;
using Foundation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace eShopOnContainers.iOS.Services
{
    internal class GeolocationSingleUpdateDelegate : CLLocationManagerDelegate
    {
        bool _haveLocation;
        readonly Position _position = new Position();
        readonly double _desiredAccuracy;
        readonly TaskCompletionSource<Position> _tcs;
        readonly CLLocationManager _manager;

        public Task<Position> Task => _tcs?.Task;

        public GeolocationSingleUpdateDelegate(CLLocationManager manager, double desiredAccuracy, int timeout, CancellationToken cancelToken)
        {
            _manager = manager;
            _tcs = new TaskCompletionSource<Position>(manager);
            _desiredAccuracy = desiredAccuracy;

            if (timeout != Timeout.Infinite)
            {
                Timer t = null;
                t = new Timer(s =>
                {
                    if (_haveLocation)
                        _tcs.TrySetResult(new Position(_position));
                    else
                        _tcs.TrySetCanceled();

                    StopListening();
                    t.Dispose();
                }, null, timeout, 0);
            }

            cancelToken.Register(() =>
            {
                StopListening();
                _tcs.TrySetCanceled();
            });
        }

        public override void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status)
        {
            // If user has services disabled, throw an exception for consistency.
            if (status == CLAuthorizationStatus.Denied || status == CLAuthorizationStatus.Restricted)
            {
                StopListening();
                _tcs.TrySetException(new GeolocationException(GeolocationError.Unauthorized));
            }
        }

        public override void Failed(CLLocationManager manager, NSError error)
        {
            switch ((CLError)(int)error.Code)
            {
                case CLError.Network:
                    StopListening();
                    _tcs.SetException(new GeolocationException(GeolocationError.PositionUnavailable));
                    break;
                case CLError.LocationUnknown:
                    StopListening();
                    _tcs.TrySetException(new GeolocationException(GeolocationError.PositionUnavailable));
                    break;
            }
        }

        public override void UpdatedLocation(CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
        {
            if (newLocation.HorizontalAccuracy < 0)
                return;

            if (_haveLocation && newLocation.HorizontalAccuracy > _position.Accuracy)
                return;

            _position.Accuracy = newLocation.HorizontalAccuracy;
            _position.Altitude = newLocation.Altitude;
            _position.AltitudeAccuracy = newLocation.VerticalAccuracy;
            _position.Latitude = newLocation.Coordinate.Latitude;
            _position.Longitude = newLocation.Coordinate.Longitude;
            _position.Speed = newLocation.Speed;

            try
            {
                _position.Timestamp = new DateTimeOffset((DateTime)newLocation.Timestamp);
            }
            catch (Exception)
            {
                _position.Timestamp = DateTimeOffset.UtcNow;
            }
            _haveLocation = true;

            if (_position.Accuracy <= _desiredAccuracy)
            {
                _tcs.TrySetResult(new Position(_position));
                StopListening();
            }
        }

        private void StopListening()
        {
            _manager.StopUpdatingLocation();
        }
    }
}
