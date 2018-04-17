using Android;
using Android.App;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using eShopOnContainers.Core.Models.Permissions;
using eShopOnContainers.Core.Services.Permissions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace eShopOnContainers.Droid.Services
{
    public class PermissionsService : IPermissionsService
    {
        const int _permissionCode = 25;
        object _locker = new object();
        TaskCompletionSource<Dictionary<Permission, PermissionStatus>> _tcs;
        Dictionary<Permission, PermissionStatus> _results;
        IList<string> _requestedPermissions;

        public static IPermissionsService Instance = new PermissionsService();

        #region Internal Implementation

        List<string> GetManifestNames(Permission permission)
        {
            var permissionNames = new List<string>();
            switch (permission)
            {
                case Permission.LocationAlways:
                case Permission.LocationWhenInUse:
                case Permission.Location:
                    {
                        if (HasPermissionInManifest(Manifest.Permission.AccessCoarseLocation))
                            permissionNames.Add(Manifest.Permission.AccessCoarseLocation);

                        if (HasPermissionInManifest(Manifest.Permission.AccessFineLocation))
                            permissionNames.Add(Manifest.Permission.AccessFineLocation);
                    }
                    break;
            }
            return permissionNames;
        }

        bool HasPermissionInManifest(string permission)
        {
            try
            {
                if (_requestedPermissions != null)
                    return _requestedPermissions.Any(r => r.Equals(permission, StringComparison.InvariantCultureIgnoreCase));

                // Try to use current activity else application context
                var context = MainApplication.CurrentContext ?? Application.Context;

                if (context == null)
                {
                    Debug.WriteLine("Unable to detect current Activity or Application Context.");
                    return false;
                }

                var info = context.PackageManager.GetPackageInfo(context.PackageName, Android.Content.PM.PackageInfoFlags.Permissions);
                if (info == null)
                {
                    Debug.WriteLine("Unable to get package info, will not be able to determine permissions to request.");
                    return false;
                }

                _requestedPermissions = info.RequestedPermissions;
                if (_requestedPermissions == null)
                {
                    Debug.WriteLine("There are no requested permissions, please check to ensure you have marked the permissions that you want to request.");
                    return false;
                }

                return _requestedPermissions.Any(r => r.Equals(permission, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception ex)
            {
                Console.Write("Unable to check manifest for permission: " + ex);
            }
            return false;
        }

        Permission GetPermissionForManifestName(string permission)
        {
            switch (permission)
            {
                case Manifest.Permission.AccessCoarseLocation:
                case Manifest.Permission.AccessFineLocation:
                    return Permission.Location;
            }

            return Permission.Unknown;
        }

        public void OnRequestPermissionResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            if (requestCode != _permissionCode)
                return;

            if (_tcs == null)
                return;

            for (var i = 0; i < permissions.Length; i++)
            {
                if (_tcs.Task.Status == TaskStatus.Canceled)
                    return;

                var permission = GetPermissionForManifestName(permissions[i]);
                if (permission == Permission.Unknown)
                    continue;

                lock (_locker)
                {
                    if (permission == Permission.Location)
                    {
                        if (!_results.ContainsKey(Permission.LocationWhenInUse))
                            _results.Add(Permission.LocationWhenInUse, grantResults[i] == Android.Content.PM.Permission.Granted ? PermissionStatus.Granted : PermissionStatus.Denied);
                    }

                    if (!_results.ContainsKey(permission))
                        _results.Add(permission, grantResults[i] == Android.Content.PM.Permission.Granted ? PermissionStatus.Granted : PermissionStatus.Denied);
                }
            }

            _tcs.TrySetResult(_results);
        }

        #endregion

        #region IPermissionsService Implementation

        public Task<PermissionStatus> CheckPermissionStatusAsync(Permission permission)
        {
            var names = GetManifestNames(permission);
            if (names == null)
            {
                Debug.WriteLine("No Android specific permissions needed for: " + permission);
                return Task.FromResult(PermissionStatus.Granted);
            }

            if (names.Count == 0)
            {
                Debug.WriteLine("No permissions found in manifest for: " + permission);
                return Task.FromResult(PermissionStatus.Unknown);
            }

            var context = MainApplication.CurrentContext ?? Application.Context;
            if (context == null)
            {
                Debug.WriteLine("Unable to detect current Activity or Application Context.");
                return Task.FromResult(PermissionStatus.Unknown);
            }

            bool targetsMOrHigher = context.ApplicationInfo.TargetSdkVersion >= Android.OS.BuildVersionCodes.M;
            foreach (var name in names)
            {
                if (targetsMOrHigher)
                {
                    if (ContextCompat.CheckSelfPermission(context, name) != Android.Content.PM.Permission.Granted)
                        return Task.FromResult(PermissionStatus.Denied);
                }
                else
                {
                    if (PermissionChecker.CheckSelfPermission(context, name) != PermissionChecker.PermissionGranted)
                        return Task.FromResult(PermissionStatus.Denied);
                }
            }
            return Task.FromResult(PermissionStatus.Granted);
        }

        public async Task<Dictionary<Permission, PermissionStatus>> RequestPermissionsAsync(params Permission[] permissions)
        {
            if (_tcs != null && !_tcs.Task.IsCompleted)
            {
                _tcs.SetCanceled();
                _tcs = null;
            }
            lock (_locker)
            {
                _results = new Dictionary<Permission, PermissionStatus>();
            }

            var context = MainApplication.CurrentContext;
            if (context == null)
            {
                Debug.WriteLine("Unable to detect current Activity.");
                foreach (var permission in permissions)
                {
                    lock (_locker)
                    {
                        if (!_results.ContainsKey(permission))
                            _results.Add(permission, PermissionStatus.Unknown);
                    }
                }

                return _results;
            }

            var permissionsToRequest = new List<string>();
            foreach (var permission in permissions)
            {
                var result = await CheckPermissionStatusAsync(permission).ConfigureAwait(false);
                if (result != PermissionStatus.Granted)
                {
                    var names = GetManifestNames(permission);
                    if ((names?.Count ?? 0) == 0)
                    {
                        lock (_locker)
                        {
                            if (!_results.ContainsKey(permission))
                                _results.Add(permission, PermissionStatus.Unknown);
                        }
                        continue;
                    }

                    permissionsToRequest.AddRange(names);
                }
                else
                {
                    lock (_locker)
                    {
                        if (!_results.ContainsKey(permission))
                            _results.Add(permission, PermissionStatus.Granted);
                    }
                }
            }

            if (permissionsToRequest.Count == 0)
                return _results;

            _tcs = new TaskCompletionSource<Dictionary<Permission, PermissionStatus>>();
            ActivityCompat.RequestPermissions((Activity)context, permissionsToRequest.ToArray(), _permissionCode);
            return await _tcs.Task.ConfigureAwait(false);
        }

        #endregion
    }
}
