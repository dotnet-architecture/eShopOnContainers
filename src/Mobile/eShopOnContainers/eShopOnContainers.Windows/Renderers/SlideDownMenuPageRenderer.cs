using eShopOnContainers.Core.Views;
using eShopOnContainers.Windows.Renderers;
using SlideOverKit.UWP;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CatalogView), typeof(SlideDownMenuPageRenderer))]
namespace eShopOnContainers.Windows.Renderers
{
    public class SlideDownMenuPageRenderer : PageRenderer, ISlideOverKitPageRendererUWP
    {
        public Action<ElementChangedEventArgs<Page>> OnElementChangedEvent { get; set; }
        SlideOverKitUWPHandler _handler;

        public SlideDownMenuPageRenderer()
        {
            _handler = new SlideOverKitUWPHandler();
            _handler.Init(this);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            if (OnElementChangedEvent != null)
                OnElementChangedEvent(e);
        }

        protected override void Dispose(bool disposing)
        {
            _handler.Dispose();
            base.Dispose(disposing);
            _handler = null;
        }
    }
}
