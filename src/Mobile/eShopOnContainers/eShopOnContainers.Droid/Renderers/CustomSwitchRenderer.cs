using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using eShopOnContainers.Core.Controls;

namespace eShopOnContainers.Droid.Renderers
{
    public class CustomSwitchRenderer : ViewRenderer<CustomSwitch, Android.Widget.Switch>
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

            var switchControl = new Android.Widget.Switch(Forms.Context)
            {
                TextOn = this.Element.TextOn,
                TextOff = this.Element.TextOff
            };

            switchControl.CheckedChange += ControlValueChanged;
            this.Element.Toggled += ElementToggled;

            this.SetNativeControl(switchControl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Control.CheckedChange -= this.ControlValueChanged;
                this.Element.Toggled -= ElementToggled;
            }

            base.Dispose(disposing);
        }

        private void ElementToggled(object sender, ToggledEventArgs e)
        {
            this.Control.Checked = this.Element.IsToggled;
        }

        private void ControlValueChanged(object sender, EventArgs e)
        {
            this.Element.IsToggled = this.Control.Checked;
        }
    }
}