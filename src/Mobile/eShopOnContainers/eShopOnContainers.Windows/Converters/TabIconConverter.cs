using System;
using Xamarin.Forms;
using UI = Windows.UI;

namespace eShopOnContainers.Windows.Converters
{
    public class TabIconConverter : UI.Xaml.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is FileImageSource)
            {
                return string.Format("ms-appx:///{0}", ((FileImageSource)value).File);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}