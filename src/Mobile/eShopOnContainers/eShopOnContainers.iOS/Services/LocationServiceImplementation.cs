using CoreLocation;
using eShopOnContainers.Core.Models.Location;
using eShopOnContainers.Core.Models.Permissions;
using eShopOnContainers.Core.Services.Location;
using eShopOnContainers.Core.Services.Permissions;
using eShopOnContainers.iOS.Services;
using Foundation;
using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(LocationServiceImplementation))]
namespace eShopOnContainers.iOS.Services
{
    public class LocationServiceImplementation : ILocationServiceImplementation
    {
        IPermissionsService _permissionsService;

        public LocationServiceImplementation()
        {
            DesiredAccuracy = 100;
            _permissionsService = new PermissionsService();
        }

        #region Internal Implementation

        async Task<bool> CheckPermissions(Permission permission)
        {
            var status = await _permissionsService.CheckPermissionStatusAsync(permission);
            if (status != PermissionStatus.Granted)
            {
                Console.WriteLine("Currently do not have Location permissions, requesting permissions");

                var request = await _permissionsService.RequestPermissionsAsync(permission);
                if (request[permission] != PermissionStatus.Granted)
                {
                    Console.WriteLine("Location permission denied, can not get positions async.");
                    return false;
                }
            }
            return true;
        }

        CLLocationManager GetManager()
        {
            CLLocationManager manager = null;
            new NSObject().InvokeOnMainThread(() => manager = new CLLocationManager());
            return manager;
        }

        #endregion

        #region ILocationServiceImplementation

        public double DesiredAccuracy { get; set; }
        public bool IsGeolocationAvailable => true;
        public bool IsGeolocationEnabled
        {
            get
            {
                var status = CLLocationManager.Status;
                return CLLocationManager.LocationServicesEnabled;
            }
        }

        public async Task<Position> GetPositionAsync(TimeSpan? timeout, CancellationToken? cancelToken = null)
        {
            var permission = Permission.LocationWhenInUse;
            var hasPermission = await CheckPermissions(permission);
            if (!hasPermission)
                throw new GeolocationException(GeolocationError.Unauthorized);

            var timeoutMilliseconds = timeout.HasValue ? (int)timeout.Value.TotalMilliseconds : Timeout.Infinite;
            if (timeoutMilliseconds <= 0 && timeoutMilliseconds != Timeout.Infinite)
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be positive or Timeout.Infinite");
            if (!cancelToken.HasValue)
                cancelToken = CancellationToken.None;

            TaskCompletionSource<Position> tcs;

            var manager = GetManager();
            manager.DesiredAccuracy = DesiredAccuracy;

            // Always prevent location update pausing since we're only listening for a single update
            if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))
                manager.PausesLocationUpdatesAutomatically = false;

            tcs = new TaskCompletionSource<Position>(manager);
            var singleListener = new GeolocationSingleUpdateDelegate(manager, DesiredAccuracy, timeoutMilliseconds, cancelToken.Value);
            manager.Delegate = singleListener;
            manager.StartUpdatingLocation();

            return await singleListener.Task;
        }

        #endregion
    }
}
