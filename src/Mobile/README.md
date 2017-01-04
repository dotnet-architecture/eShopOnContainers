#eShopOnContainers

eShopOnContainers is a reference app whose imagined purpose is to serve the mobile workforce of a fictitious company that sells products. The app allow to manage the catalog, view products, manage the basket and the orders.

<img src="Images/eShopOnContainers_Architecture_Diagram.png" alt="eShopOnContainers" Width="800" />

###Supported platforms: iOS, Android and Windows

###The app architecture consists of two parts:
  1. A Xamarin.Forms mobile app for iOS, Android and Windows.
  2. Several .NET Web API microservices deployed as Docker containers.

##Xamarin.Forms App (eShopOnContainers)

This project exercises the following platforms, frameworks or features:

* Xamarin.Forms
  * XAML
  * Bindings
  * Converters
  * Central Styles
  * Custom Renderers
  * Animations
  * IoC
  * Messaging Center
  * Custom Controls
  * Cross Plugins
  * XFGloss
* xUnit Tests
* Azure Mobile Services
  * C# backend
  * WebAPI
  * Entity Framework
  * Identity Server 4

##Three platforms
The app targets **three** platforms:

* iOS
* Android
* Universal Windows Platform (UWP)
    * UWP supported only in Visual Studio, not Xamarin Studio


As of 19/12/2016, eShopOnContainers features **89.2% code share** (7.2% iOS / 16.7% Android / 8.7% Windows).

##Licenses

This project uses some third-party assets with a license that requires attribution:

- [Xamarin.Plugins](https://github.com/jamesmontemagno/Xamarin.Plugins): by James Montemagno
- [FFImageLoading](https://github.com/daniel-luberda/FFImageLoading): by Daniel Luberda
- [ACR User Dialogs](https://github.com/aritchie/userdialogs): by Allan Ritchie
- [Xamarin.Forms Animation Helpers](https://github.com/jsuarezruiz/Xamanimation): by Javier Suárez
- [SlideOverKit](https://github.com/XAM-Consulting/SlideOverKit): by XAM-Consulting

## Requirements
### Requirements for Dec. 2016 version of eShopOnContainers

* [Visual Studio __2015__](https://www.visualstudio.com/en-us/products/vs-2015-product-editions.aspx) (14.0 or higher) to compile C# 6 langage features (or Visual Studio MacOS)
* Xamarin add-ons for Visual Studio (available via the Visual Studio installer)
* __Visual Studio Community Edition is fully supported!__
* Android SDK Tools 25.2.3 or higher

## Setup

#### [1. Ensure the Xamarin platform is installed](http://developer.xamarin.com/guides/cross-platform/getting_started/installation/)

#### 2. Ensure Xamarin are updated
Xamarin will periodically automatically check for updates. You can also manually check for updates.

<img src="Images/Updates.png" alt="Ensure Xamarin are updated" Width="600" />

### 3. Project Setup

Restore NuGet packages for the project.

### 4. Ensure Android Emulator is installed
You can use any Android emulator although it is highly recommended to use an x86 based version.

<img src="Images/AndroidEmulator.png" alt="Visual Studio Android Emulator" Width="600" />

**Note**: The Visual Studio Android Emulator cannot run well inside a virtual machine or over Remote Desktop or VNC since it relies on virtualization and OpenGL. 

To deploy and debug the application on a physical device, refer to these [link](https://developer.xamarin.com/guides/android/deployment,_testing,_and_metrics/debug-on-device/).

### 5. Ensure Mac connection
To set up the Mac host, you must enable communication between the Xamarin extension for Visual Studio and your Mac.

<img src="Images/MacAgent.png" alt="Connect with a Mac" Width="600" />

## Screens

The app has the following screens:

* a auth screen
* a catalog list
* a profile section with a order list
* a readonly order detail screen
* a customizable basket
* a checkout screen

<img src="Images/Auth.png" alt="Login" Width="210" />
<img src="Images/Catalog.png" alt="Catalog" Width="210" />
<img src="Images/Filter.png" alt="Filter catalog" Width="210" />
<img src="Images/Profile.png" alt="Profile" Width="210" />
<img src="Images/OrderDetail.png" alt="Order details" Width="210" />
<img src="Images/ShoppingCart.png" alt="Basket" Width="210" />
<img src="Images/Settings.png" alt="Settings" Width="210" />

## Clean and Rebuild
If you see build issues when pulling updates from the repo, try cleaning and rebuilding the solution.

## Copyright and license
* Code and documentation copyright 2016 Microsoft Corp. Code released under the [MIT license](https://opensource.org/licenses/MIT).