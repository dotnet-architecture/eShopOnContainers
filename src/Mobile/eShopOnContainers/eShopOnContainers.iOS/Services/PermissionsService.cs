using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Permissions;
using eShopOnContainers.Core.Services.Permissions;
using CoreLocation;
using UIKit;
using Foundation;

namespace eShopOnContainers.iOS.Services
{
    public class PermissionsService : IPermissionsService
    {
        #region Internal Implementation

        PermissionStatus GetLocationPermissionStatus(Permission permission)
        {
            if (!CLLocationManager.LocationServicesEnabled)
                return PermissionStatus.Disabled;

            var status = CLLocationManager.Status;

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                switch (status)
                {
                    case CLAuthorizationStatus.AuthorizedAlways:
                    case CLAuthorizationStatus.AuthorizedWhenInUse:
                        return PermissionStatus.Granted;
                    case CLAuthorizationStatus.Denied:
                        return PermissionStatus.Denied;
                    case CLAuthorizationStatus.Restricted:
                        return PermissionStatus.Restricted;
                    default:
                        return PermissionStatus.Unknown;
                }
            }

            switch (status)
            {
                case CLAuthorizationStatus.Authorized:
                    return PermissionStatus.Granted;
                case CLAuthorizationStatus.Denied:
                    return PermissionStatus.Denied;
                case CLAuthorizationStatus.Restricted:
                    return PermissionStatus.Restricted;
                default:
                    return PermissionStatus.Unknown;
            }
        }

        Task<PermissionStatus> RequestLocationPermissionAsync(Permission permission = Permission.Location)
        {
            if (CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse && permission == Permission.LocationAlways)
            {
                // Don't do anything and request it
            }
            else if (GetLocationPermissionStatus(permission) != PermissionStatus.Unknown)
                return Task.FromResult(GetLocationPermissionStatus(permission));

            if (!UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                return Task.FromResult(PermissionStatus.Unknown);
            }

            EventHandler<CLAuthorizationChangedEventArgs> authCallback = null;
            var tcs = new TaskCompletionSource<PermissionStatus>();
            var locationManager = new CLLocationManager();

            authCallback = (sender, e) =>
            {
                if (e.Status == CLAuthorizationStatus.NotDetermined)
                    return;
                locationManager.AuthorizationChanged -= authCallback;
                tcs.TrySetResult(GetLocationPermissionStatus(permission));
            };
            locationManager.AuthorizationChanged += authCallback;

            var info = NSBundle.MainBundle.InfoDictionary;
            //if (permission == Permission.Location)
            //{
            //    if (info.ContainsKey(new NSString("NSLocationAlwaysUsageDescription")))
            //        locationManager.RequestAlwaysAuthorization();
            //    else if (info.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription")))
            //        locationManager.RequestWhenInUseAuthorization();
            //    else
            //        throw new UnauthorizedAccessException("On iOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates!");
            //}
            //else if (permission == Permission.LocationAlways)
            //{
            //    if (info.ContainsKey(new NSString("NSLocationAlwaysUsageDescription")))
            //        locationManager.RequestAlwaysAuthorization();
            //    else
            //        throw new UnauthorizedAccessException("On iOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates!");
            //}
            if (permission == Permission.LocationWhenInUse)
            {
                if (info.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription")))
                    locationManager.RequestWhenInUseAuthorization();
                else
                    throw new UnauthorizedAccessException("On iOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates.");
            }
            return tcs.Task;
        }

        #endregion

        #region IPermissionsServiceImplementation

        public Task<PermissionStatus> CheckPermissionStatusAsync(Permission permission)
        {
            switch (permission)
            {
                case Permission.LocationWhenInUse:
                    return Task.FromResult(GetLocationPermissionStatus(permission));
            }
            return Task.FromResult(PermissionStatus.Granted);
        }

        public async Task<Dictionary<Permission, PermissionStatus>> RequestPermissionsAsync(params Permission[] permissions)
        {
            var results = new Dictionary<Permission, PermissionStatus>();
            foreach (var permission in permissions)
            {
                if (results.ContainsKey(permission))
                    continue;

                switch (permission)
                {
                    case Permission.LocationWhenInUse:
                        results.Add(permission, await RequestLocationPermissionAsync(permission).ConfigureAwait(false));
                        break;
                }

                if (!results.ContainsKey(permission))
                    results.Add(permission, PermissionStatus.Granted);
            }
            return results;
        }

        #endregion
    }
}
