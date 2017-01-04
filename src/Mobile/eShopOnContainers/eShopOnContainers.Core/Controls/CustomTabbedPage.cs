using Xamarin.Forms;

namespace eShopOnContainers.Core.Controls
{
    public class CustomTabbedPage : TabbedPage
    {
        public static BindableProperty BadgeTextProperty = 
            BindableProperty.CreateAttached("BadgeText", typeof(string), typeof(CustomTabbedPage), default(string), 
                BindingMode.OneWay);

        public static BindableProperty BadgeColorProperty = 
            BindableProperty.CreateAttached("BadgeColor", typeof(Color), typeof(CustomTabbedPage), Color.Default, 
                BindingMode.OneWay);

        public static string GetBadgeText(BindableObject view)
        {
            return (string)view.GetValue(BadgeTextProperty);
        }

        public static void SetBadgeText(BindableObject view, string value)
        {
            view.SetValue(BadgeTextProperty, value);
        }
               
        public static Color GetBadgeColor(BindableObject view)
        {
            return (Color)view.GetValue(BadgeColorProperty);
        }

        public static void SetBadgeColor(BindableObject view, Color value)
        {
            view.SetValue(BadgeColorProperty, value);
        }
    }
}