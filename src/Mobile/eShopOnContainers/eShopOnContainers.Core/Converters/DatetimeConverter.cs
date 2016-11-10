using System;
using System.Globalization;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Converters
{
    public class DatetimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DateTime)
            {
                var date = (DateTime)value;

                return date.ToString("MMMM dd yyyy").ToUpper();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
