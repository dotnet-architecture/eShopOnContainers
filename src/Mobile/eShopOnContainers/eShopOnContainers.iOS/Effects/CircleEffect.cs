using CoreAnimation;
using CoreGraphics;
using eShopOnContainers.iOS.Effects;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(CircleEffect), "CircleEffect")]
namespace eShopOnContainers.iOS.Effects
{
    public class CircleEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            UpdateCircle();
        }

        protected override void OnDetached()
        {
            Container.Layer.Mask = null;
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == VisualElement.WidthProperty.PropertyName ||
                args.PropertyName == VisualElement.HeightProperty.PropertyName)
            {
                UpdateCircle();
            }
        }

        private void UpdateCircle()
        {
            double width = ((VisualElement)Element).Width;
            double height = ((VisualElement)Element).Height;

            if (width <= 0 || height <= 0)
            {
                return;
            }

            double min = Math.Min(width, height);
            var layerX = width > min ? (width - min) / 2 : 0;
            var layerY = height > min ? (height - min) / 2 : 0;

            var mask = new CAShapeLayer();
            mask.Path = CGPath.EllipseFromRect(new CGRect(layerX, layerY, min, min));
            Container.Layer.Mask = mask;
        }
    }
}
