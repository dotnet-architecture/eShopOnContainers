using Xamarin.Forms;

namespace eShopOnContainers.Core.Controls
{
    public class CustomSwitch : Switch
    {
        public static readonly BindableProperty TextOnProperty = BindableProperty.Create("TextOn",
            typeof(string), typeof(CustomSwitch), string.Empty);

        public static readonly BindableProperty TextOffProperty = BindableProperty.Create("TextOff",
            typeof(string), typeof(CustomSwitch), string.Empty);

        public string TextOn
        {
            get { return (string)this.GetValue(TextOnProperty); }
            set { this.SetValue(TextOnProperty, value); }
        }

        public string TextOff
        {
            get { return (string)this.GetValue(TextOffProperty); }
            set { this.SetValue(TextOffProperty, value); }
        }
    }
}