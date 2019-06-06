# Getting Started with Geolocator Plugin

### API Usage

Call **CrossGeolocator.Current** from any project or PCL to gain access to APIs.

```
var locator = CrossGeolocator.Current;
locator.DesiredAccuracy = 50;

var position = await locator.GetPositionAsync (timeoutMilliseconds: 10000);

Console.WriteLine ("Position Status: {0}", position.Timestamp);
Console.WriteLine ("Position Latitude: {0}", position.Latitude);
Console.WriteLine ("Position Longitude: {0}", position.Longitude);
```

### **IMPORTANT**
Android:

You must request ACCESS_COARSE_LOCATION & ACCESS_FINE_LOCATION permission

iOS:

In iOS 8 you now have to call either RequestWhenInUseAuthorization or RequestAlwaysAuthorization on the location manager. Additionally you need to add either the concisely named NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription to your Info.plist. 
See:  http://motzcod.es/post/97662738237/scanning-for-ibeacons-in-ios-8

Windows Phone:

You must set the ID_CAP_LOCATION permission.