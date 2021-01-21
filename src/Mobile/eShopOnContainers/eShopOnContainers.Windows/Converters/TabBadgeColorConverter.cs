using eShopOnContainers.Core.Controls;
using eShopOnContainers.Windows.Helpers;
using System;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using UI = Windows.UI;

namespace eShopOnContainers.Windows.Converters
{
    public class TabBadgeColorConverter : UI.Xaml.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Page)
            {
                var badgeColor = CustomTabbedPage.GetBadgeColor((Page)value);

                var color = ColorHelper.XamarinFormColorToWindowsColor(badgeColor);

                return new SolidColorBrush(color);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
