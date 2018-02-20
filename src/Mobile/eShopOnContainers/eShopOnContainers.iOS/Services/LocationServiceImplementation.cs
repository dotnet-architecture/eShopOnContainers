using eShopOnContainers.iOS.Services;
using eShopOnContainers.Core.Services.Location;
using CoreLocation;
using eShopOnContainers.Core.Models.Location;
using Foundation;
using System;
using System.Threading.Tasks;
using System.Threading;
using UIKit;
using eShopOnContainers.Core.Models.Permissions;
using eShopOnContainers.Core.Services.Permissions;

[assembly: Xamarin.Forms.Dependency(typeof(LocationServiceImplementation))]
namespace eShopOnContainers.iOS.Services
{
    public class LocationServiceImplementation : ILocationServiceImplementation
    {
        bool _deferringUpdates;
        readonly CLLocationManager _manager;
        Position _lastPosition;

        public event EventHandler<PositionErrorEventArgs> PositionError;
        public event EventHandler<PositionEventArgs> PositionChanged;
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

        public bool SupportsHeading => CLLocationManager.HeadingAvailable;

        public LocationServiceImplementation()
        {
            DesiredAccuracy = 100;
            //_manager = GetManager();
            //_manager.AuthorizationChanged += OnAuthorizationChanged;
            //_manager.Failed += OnFailed;
            //_manager.UpdatedLocation += OnUpdatedLocation;
            //_manager.UpdatedHeading += OnUpdatedHeading;
            //_manager.DeferredUpdatesFinished += OnDeferredUpdatesFinished;
        }

        void OnDeferredUpdatesFinished(object sender, NSErrorEventArgs e) => _deferringUpdates = false;

        #region Internal Implementation

        async Task<bool> CheckPermissions(Permission permission)
        {
            IPermissionsService permissionsService = new PermissionsService();
            var status = await permissionsService.CheckPermissionStatusAsync(permission);
            if (status != PermissionStatus.Granted)
            {
                Console.WriteLine("Currently do not have Location permissions, requesting permissions");

                var request = await permissionsService.RequestPermissionsAsync(permission);
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

        public async Task<Position> GetPositionAsync(TimeSpan? timeout, CancellationToken? cancelToken = null, bool includeHeading = false)
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

            // Permit background updates if background location mode is enabled
            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                var backgroundModes = NSBundle.MainBundle.InfoDictionary[(NSString)"UIBackgroundModes"] as NSArray;
                manager.AllowsBackgroundLocationUpdates = backgroundModes != null && (backgroundModes.Contains((NSString)"Location") || backgroundModes.Contains((NSString)"location"));
            }

            // Always prevent location update pausing since we're only listening for a single update
            if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))
                manager.PausesLocationUpdatesAutomatically = false;

            tcs = new TaskCompletionSource<Position>(manager);
            var singleListener = new GeolocationSingleUpdateDelegate(manager, DesiredAccuracy, includeHeading, timeoutMilliseconds, cancelToken.Value);
            manager.Delegate = singleListener;
            manager.StartUpdatingLocation();

            if (includeHeading && SupportsHeading)
                manager.StartUpdatingHeading();

            return await singleListener.Task;

            //tcs = new TaskCompletionSource<Position>();
            //if (_lastPosition == null)
            //{
            //    if (cancelToken != CancellationToken.None)
            //        cancelToken.Value.Register(() => tcs.TrySetCanceled());

            //    EventHandler<PositionErrorEventArgs> gotError = null;
            //    gotError = (s, e) =>
            //    {
            //        tcs.TrySetException(new GeolocationException(e.Error));
            //        PositionError -= gotError;
            //    };
            //    PositionError += gotError;

            //    EventHandler<PositionEventArgs> gotPosition = null;
            //    gotPosition = (s, e) =>
            //    {
            //        tcs.TrySetResult(e.Position);
            //        PositionChanged += gotPosition;
            //    };
            //    PositionChanged += gotPosition;
            //}
            //else
            //    tcs.SetResult(_lastPosition);

            //return await tcs.Task;
        }

        #endregion
    }
}
