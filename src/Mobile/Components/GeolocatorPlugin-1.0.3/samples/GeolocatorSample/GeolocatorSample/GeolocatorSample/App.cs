using Geolocator.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace GeolocatorSample
{
  public class App : Application
  {
    public App()
    {

      var buttonGetGPS = new Button
      {
        Text = "GetGPS"
      };

      var labelGPS = new Label
      {
        Text = "GPS goes here"
      };

      buttonGetGPS.Clicked += async (sender, args) =>
      {
        var locator = CrossGeolocator.Current;
        locator.DesiredAccuracy = 50;
        labelGPS.Text = "Getting gps";

        var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

        if (position == null)
        {
          labelGPS.Text = "null gps :(";
          return;
        }
        labelGPS.Text = string.Format("Time: {0} \nLat: {1} \nLong: {2} \n Altitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \n Heading: {6} \n Speed: {7}",
          position.Timestamp, position.Latitude, position.Longitude,
          position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed);
      };

      // The root page of your application
      MainPage = new ContentPage
      {
        Content = new StackLayout
        {
          VerticalOptions = LayoutOptions.Center,
          Children = {
						buttonGetGPS,
            labelGPS
					}
        }
      };
    }

    protected override void OnStart()
    {
      // Handle when your app starts
    }

    protected override void OnSleep()
    {
      // Handle when your app sleeps
    }

    protected override void OnResume()
    {
      // Handle when your app resumes
    }
  }
}
