using Android.Graphics;
using Android.Views;
using eShopOnContainers.Droid.Effects;
using System;
using Xamarin.Forms;

[assembly: ExportEffect(typeof(CircleEffect), "CircleEffect")]
namespace eShopOnContainers.Droid.Effects
{
    public class CircleEffect : BaseContainerEffect
    {
        private ViewOutlineProvider _originalProvider;

        protected override bool CanBeApplied()
        {
            return Container != null && (int)global::Android.OS.Build.VERSION.SdkInt >= 21;
        }

        protected override void OnAttachedInternal()
        {
            _originalProvider = Container.OutlineProvider;
            Container.OutlineProvider = new CircleOutlineProvider();
            Container.ClipToOutline = true;
        }

        protected override void OnDetachedInternal()
        {
            Container.ClipToOutline = false;
            Container.OutlineProvider = _originalProvider;
        }

        private class CircleOutlineProvider : ViewOutlineProvider
        {
            public override void GetOutline(Android.Views.View view, Outline outline)
            {
                double width = view.Width;
                double height = view.Height;

                if (width <= 0 || height <= 0)
                {
                    return;
                }

                double min = Math.Min(width, height);
                var radius = (float)(min / 2.0);

                var layerX = width > min ? (width - min) / 2 : 0;
                var layerY = height > min ? (height - min) / 2 : 0;

                outline.SetRoundRect(new Rect((int)layerX, (int)layerY, (int)(layerX + min), (int)(layerY + min)), radius);
            }
        }
    }
}
