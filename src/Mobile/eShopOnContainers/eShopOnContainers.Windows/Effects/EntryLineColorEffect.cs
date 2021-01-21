using eShopOnContainers.Core.Behaviors;
using eShopOnContainers.Windows.Effects;
using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Media = Windows.UI.Xaml.Media;
using UI = Windows.UI;
using Xaml = Windows.UI.Xaml;

[assembly: ResolutionGroupName("eShopOnContainers")]
[assembly: ExportEffect(typeof(EntryLineColorEffect), "EntryLineColorEffect")]
namespace eShopOnContainers.Windows.Effects
{
    public class EntryLineColorEffect : PlatformEffect
    {
        TextBox control;

        protected override void OnAttached()
        {
            try
            {
                control = Control as TextBox;
                UpdateLineColor();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
            control = null;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            if (args.PropertyName == LineColorBehavior.LineColorProperty.PropertyName)
            {
                UpdateLineColor();
            }
        }

        private void UpdateLineColor()
        {
            if (control != null)
            {
                control.BorderThickness = new Xaml.Thickness(0, 0, 0, 1);
                var lineColor = XamarinFormColorToWindowsColor(LineColorBehavior.GetLineColor(Element));
                control.BorderBrush = new Media.SolidColorBrush(lineColor);

                var style = Xaml.Application.Current.Resources["FormTextBoxStyle"] as Xaml.Style;
                control.Style = style;
            }
        }

        private UI.Color XamarinFormColorToWindowsColor(Color xamarinColor)
        {
            return UI.Color.FromArgb((byte)(xamarinColor.A * 255),
                                     (byte)(xamarinColor.R * 255),
                                     (byte)(xamarinColor.G * 255),
                                     (byte)(xamarinColor.B * 255));
        }
    }
}
