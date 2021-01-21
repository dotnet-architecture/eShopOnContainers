using eShopOnContainers.Core.Controls;
using eShopOnContainers.Windows.Controls;
using eShopOnContainers.Windows.Extensions;
using eShopOnContainers.Windows.Helpers;
using eShopOnContainers.Windows.Renderers;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;
using Xaml = Windows.UI.Xaml;

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
                Debug.WriteLine("No FormsPivot found. Badge not added.");
                return;
            }

            for (var i = 0; i < Control.Items.Count; i++)
            {
                AddTabBadge(i);
            }
        }

        private void AddTabBadge(int tabIndex)
        {
            if (Element == null)
            {
                return;
            }

            var element = Element.Children[tabIndex];

            if (element != null)
            {
                var dataTemplate = Xaml.Application.Current.Resources["TabbedPageHeaderTemplate"] as
                    Xaml.DataTemplate;

                Control.HeaderTemplate = dataTemplate;
                Control.SelectionChanged += OnControlSelectionChanged;

                element.PropertyChanged += OnTabbedPagePropertyChanged;
            }
        }

        private void OnControlSelectionChanged(object sender, Xaml.Controls.SelectionChangedEventArgs e)
        {
            SetSelectedTab((FormsPivot)sender);
        }

        protected virtual void OnTabbedPagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var element = sender as Element;

            if (element == null)
                return;

            var tabItems = Control.GetDescendantsOfType<TabItem>();
            var tabItem = tabItems.FirstOrDefault(t => t.Label == ((Page)element).Title);

            if (tabItem == null)
            {
                return;
            }

            if (e.PropertyName == CustomTabbedPage.BadgeTextProperty.PropertyName)
            {
                tabItem.BadgeText = CustomTabbedPage.GetBadgeText(element);
                return;
            }

            if (e.PropertyName == CustomTabbedPage.BadgeColorProperty.PropertyName)
            {
                tabItem.BadgeColor = new Xaml.Media.SolidColorBrush(
                    ColorHelper.XamarinFormColorToWindowsColor(
                    CustomTabbedPage.GetBadgeColor(element)));
            }

            SetSelectedTab(Control);
        }

        private void SetSelectedTab(FormsPivot pivot)
        {
            var tabItems = Control.GetDescendantsOfType<TabItem>();
            var tabItem = tabItems.FirstOrDefault(t => t.Label == ((Page)pivot.SelectedItem).Title);

            if (tabItem == null)
            {
                return;
            }

            foreach (var item in tabItems)
                item.Opacity = 0.5;

            tabItem.Opacity = 1.0;
        }
    }
}