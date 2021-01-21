using Xamarin.Forms;
using UI = Windows.UI;

namespace eShopOnContainers.Windows.Helpers
{
    public static class ColorHelper
    {
        public static UI.Color XamarinFormColorToWindowsColor(Color xamarinColor)
        {
            return UI.Color.FromArgb((byte)(xamarinColor.A * 255),
                                     (byte)(xamarinColor.R * 255),
                                     (byte)(xamarinColor.G * 255),
                                     (byte)(xamarinColor.B * 255));
        }
    }
}
