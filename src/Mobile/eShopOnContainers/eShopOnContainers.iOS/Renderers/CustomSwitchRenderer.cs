using eShopOnContainers.Core.Controls;
using eShopOnContainers.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomSwitch), typeof(CustomSwitchRenderer))]
namespace eShopOnContainers.iOS.Renderers
{
    public class CustomSwitchRenderer : ViewRenderer<CustomSwitch, UISwitch>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomSwitch> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                this.Element.Toggled -= ElementToggled;
                return;
            }

            if (this.Element == null)
            {
                return;
            }

            var uiSwitchControl = new UISwitch();

            uiSwitchControl.ValueChanged += ControlValueChanged;
            this.Element.Toggled += ElementToggled;

            this.SetNativeControl(uiSwitchControl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Control.ValueChanged -= this.ControlValueChanged;
                this.Element.Toggled -= ElementToggled;
            }

            base.Dispose(disposing);
        }

        private void ElementToggled(object sender, ToggledEventArgs e)
        {
            this.Control.On = Element.IsToggled;
        }

        private void ControlValueChanged(object sender, System.EventArgs e)
        {
            this.Element.IsToggled = this.Control.On;
        }
    }
}