using eShopOnContainers.Windows.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace eShopOnContainers.Windows.Renderers
{
    [Preserve]
    public class CustomTabbedPageRenderer : TabbedPageRenderer
    {
    }
}