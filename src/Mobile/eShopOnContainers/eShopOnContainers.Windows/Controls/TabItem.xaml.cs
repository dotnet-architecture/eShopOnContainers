using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace eShopOnContainers.Windows.Controls
{
    public sealed partial class TabItem : UserControl
    {
        public static readonly DependencyProperty IconProperty = 
            DependencyProperty.Register("Icon", typeof(string), typeof(TabItem), null);

        public string Icon
        {
            get { return GetValue(IconProperty) as string; }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = 
            DependencyProperty.Register("Label", typeof(string), typeof(TabItem), null);

        public string BadgeText
        {
            get { return GetValue(BadgeTextProperty) as string; }
            set { SetValue(BadgeTextProperty, value); }
        }

        public static readonly DependencyProperty BadgeTextProperty =
            DependencyProperty.Register("BadgeText", typeof(string), typeof(TabItem), null);

        public SolidColorBrush BadgeColor
        {
            get { return GetValue(BadgeColorProperty) as SolidColorBrush; }
            set { SetValue(BadgeColorProperty, value); }
        }

        public static readonly DependencyProperty BadgeColorProperty =
            DependencyProperty.Register("BadgeColor", typeof(SolidColorBrush), typeof(TabItem), null);

        public string Label
        {
            get { return GetValue(LabelProperty) as string; }
            set { SetValue(LabelProperty, value); }
        }

        public TabItem()
        {
            this.InitializeComponent();
        }
    }
}