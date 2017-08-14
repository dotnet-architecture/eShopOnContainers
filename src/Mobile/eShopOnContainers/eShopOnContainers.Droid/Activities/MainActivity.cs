using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using FFImageLoading.Forms.Droid;
using Acr.UserDialogs;
using Android.Content;
using Android.Runtime;
using FFImageLoading;
using System;
using Plugin.Permissions;

namespace eShopOnContainers.Droid.Activities
{
    [Activity(
        Label = "eShopOnContainers",
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            FormsAppCompatActivity.ToolbarResource = Resource.Layout.Toolbar;
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.Tabs;

            base.OnCreate(bundle);

            SupportActionBar.SetDisplayShowHomeEnabled(true); // Show or hide the default home button
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowCustomEnabled(true); // Enable overriding the default toolbar layout
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            UserDialogs.Init(this);
            CachedImageRenderer.Init();
            LoadApplication(new App());

            Window window = this.Window;
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.SetStatusBarColor(Android.Graphics.Color.Rgb(0, 166, 156));
        }

        /// <summary>
        /// FFImageLoading image service preserves in heap memory of the device every image newly downloaded 
        /// from url. In order to avoid application crash, you should reclaim memory in low memory situations.
        /// </summary>
        public override void OnTrimMemory([GeneratedEnum] TrimMemory level)
        {
            ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            base.OnTrimMemory(level);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

