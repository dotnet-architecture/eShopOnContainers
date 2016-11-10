
using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Views;

namespace eShopOnContainers.Droid.Activities
{
    [Activity(
        Label = "eShopOnContainers", 
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            var x = typeof(Xamarin.Forms.Themes.LightThemeResources);
            x = typeof(Xamarin.Forms.Themes.Android.UnderlineEffect);

            Window window = this.Window;
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.SetStatusBarColor(Android.Graphics.Color.Rgb(0, 166, 156));
        }
    }
}

