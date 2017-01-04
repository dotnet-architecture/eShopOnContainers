using Xamarin.UITest;

namespace eShopOnContainers.UITests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp
                    .Android
                    .ApkFile(@"..\..\..\eShopOnContainers.Droid\bin\Release\eShopOnContainers.Droid.apk")
                    .StartApp();
            }

            return ConfigureApp
                .iOS
                .StartApp();
        }
    }
}

