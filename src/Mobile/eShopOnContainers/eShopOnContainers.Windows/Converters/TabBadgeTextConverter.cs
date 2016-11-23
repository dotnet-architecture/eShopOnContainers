using eShopOnContainers.Core.Controls;
using System;
using Xamarin.Forms;
using UI = Windows.UI;

namespace eShopOnContainers.Windows.Converters
{
    public class TabBadgeTextConverter : UI.Xaml.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Page)
            {
                var badgeText = CustomTabbedPage.GetBadgeText((Page)value);

                return badgeText;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
