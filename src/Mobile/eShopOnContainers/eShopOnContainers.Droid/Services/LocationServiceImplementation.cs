using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using eShopOnContainers.Core.Models.Location;
using eShopOnContainers.Core.Models.Permissions;
using eShopOnContainers.Core.Services.Location;
using eShopOnContainers.Droid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(LocationServiceImplementation))]
namespace eShopOnContainers.Droid.Services
{
    public class LocationServiceImplementation : ILocationServiceImplementation
    {
        #region Internal Implementation

        LocationManager _locationManager;
        GeolocationSingleListener _singleListener = null;

        string[] Providers => Manager.GetProviders(enabledOnly: false).ToArray();
        string[] IgnoredProviders => new string[] { LocationManager.PassiveProvider, "local_database" };

        public static string[] ProvidersToUse { get; set; } = new string[] { };

        LocationManager Manager
        {
            get
            {
                if (_locationManager == null)
                    _locationManager = (LocationManager)Application.Context.GetSystemService(Context.LocationService);
                return _locationManager;
            }
        }

        public LocationServiceImplementation()
        {
            DesiredAccuracy = 100;
        }

        async Task<bool> CheckPermissionsAsync()
        {
            var status = await PermissionsService.Instance.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                Console.WriteLine("Currently do not have Location permissions, requesting permissions.");

                var request = await PermissionsService.Instance.RequestPermissionsAsync(Permission.Location);
                if (request[Permission.Location] != PermissionStatus.Granted)
                {
                    Console.WriteLine("Location permission denied.");
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region ILocationServiceImplementation

        public double DesiredAccuracy { get; set; }

        public bool IsGeolocationAvailable => Providers.Length > 0;

        public bool IsGeolocationEnabled => Providers.Any(p => !IgnoredProviders.Contains(p) && Manager.IsProviderEnabled(p));

        public async Task<Position> GetPositionAsync(TimeSpan? timeout = null, CancellationToken? cancelToken = null)
        {
            var timeoutMilliseconds = timeout.HasValue ? (int)timeout.Value.TotalMilliseconds : Timeout.Infinite;
            if (timeoutMilliseconds <= 0 && timeoutMilliseconds != Timeout.Infinite)
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be greater than or equal to 0");

            if (!cancelToken.HasValue)
                cancelToken = CancellationToken.None;

            var hasPermission = await CheckPermissionsAsync();
            if (!hasPermission)
                throw new GeolocationException(GeolocationError.Unauthorized);

            var tcs = new TaskCompletionSource<Position>();

            var providers = new List<string>();
            if (ProvidersToUse == null || ProvidersToUse.Length == 0)
                providers.AddRange(Providers);
            else
            {
                foreach (var provider in Providers)
                {
                    if (ProvidersToUse?.Contains(provider) ?? false)
                        continue;
                    providers.Add(provider);
                }
            }

            void SingleListenerFinishCallback()
            {
                if (_singleListener == null)
                    return;

                for (int i = 0; i < providers.Count; ++i)
                    Manager.RemoveUpdates(_singleListener);
            }

            _singleListener = new GeolocationSingleListener(Manager, (float)DesiredAccuracy, timeoutMilliseconds, providers.Where(Manager.IsProviderEnabled), finishedCallback: SingleListenerFinishCallback);
            if (cancelToken != CancellationToken.None)
            {
                cancelToken.Value.Register(() =>
                {
                    _singleListener.Cancel();

                    for (int i = 0; i < providers.Count; ++i)
                        Manager.RemoveUpdates(_singleListener);
                }, true);
            }

            try
            {
                var looper = Looper.MyLooper() ?? Looper.MainLooper;
                int enabled = 0;
                for (var i = 0; i < providers.Count; ++i)
                {
                    if (Manager.IsProviderEnabled(providers[i]))
                        enabled++;

                    Manager.RequestLocationUpdates(providers[i], 0, 0, _singleListener, looper);
                }

                if (enabled == 0)
                {
                    for (int i = 0; i < providers.Count; ++i)
                        Manager.RemoveUpdates(_singleListener);

                    tcs.SetException(new GeolocationException(GeolocationError.PositionUnavailable));
                    return await tcs.Task;
                }
            }
            catch (Java.Lang.SecurityException ex)
            {
                tcs.SetException(new GeolocationException(GeolocationError.Unauthorized, ex));
                return await tcs.Task;
            }
            return await _singleListener.Task;
        }

        #endregion
    }
}
