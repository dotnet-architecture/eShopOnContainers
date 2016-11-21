using eShopOnContainers.Windows.Renderers;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace eShopOnContainers.Windows.Renderers
{
    [Preserve]
    public class CustomTabbedPageRenderer : TabbedPageRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                Debug.WriteLine("No TabLayout found. Badge not added.");
                return;
            }

            for (var i = 0; i < Control.Items.Count; i++)
            {
                AddTabBadge(i);
            }
        }

        private void AddTabBadge(int tabIndex)
        {
         
        }
    }
}