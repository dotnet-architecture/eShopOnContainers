using eShopOnContainers.Core.Controls;
using eShopOnContainers.Windows.Renderers;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomSwitch), typeof(CustomSwitchRenderer))]
namespace eShopOnContainers.Windows.Renderers
{
    public class CustomSwitchRenderer : ViewRenderer<CustomSwitch, ToggleSwitch>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomSwitch> e)
        {
            base.OnElementChanged(e);

            if (this.Element == null)
            {
                return;
            }

            if (e.OldElement != null)
            {
                this.Element.Toggled -= ElementToggled;
                return;
            }

            var toggleSwitchControl = new ToggleSwitch
            {
                OnContent = this.Element.TextOn,
                OffContent = this.Element.TextOff
            };

            toggleSwitchControl.Toggled += ControlToggled;
            this.Element.Toggled += ElementToggled;

            this.SetNativeControl(toggleSwitchControl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Control.Toggled -= this.ControlToggled;
                this.Element.Toggled -= ElementToggled;
            }

            base.Dispose(disposing);
        }

        private void ElementToggled(object sender, ToggledEventArgs e)
        {
            this.Control.IsOn = this.Element.IsToggled;
        }

        private void ControlToggled(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Element.IsToggled = this.Control.IsOn;
        }
    }
}