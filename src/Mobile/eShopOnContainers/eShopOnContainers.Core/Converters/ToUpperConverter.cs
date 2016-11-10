using System;
using System.Globalization;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Converters
{
    public class ToUpperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().ToUpperInvariant();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
