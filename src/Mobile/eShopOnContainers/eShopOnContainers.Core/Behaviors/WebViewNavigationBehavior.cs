using System.Windows.Input;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Behaviors
{
    public class WebViewNavigationBehavior : Behavior<WebView>
    {
        private VisualElement _element;

        public static readonly BindableProperty NavigateCommandProperty =
                BindableProperty.Create("NavigateCommand", typeof(ICommand),
                    typeof(WebViewNavigationBehavior), default(ICommand),
                    BindingMode.OneWay, null);

        public ICommand NavigateCommand
        {
            get { return (ICommand)GetValue(NavigateCommandProperty); }
            set { SetValue(NavigateCommandProperty, value); }
        }

        protected override void OnAttachedTo(WebView bindable)
        {
            _element = bindable;
            bindable.Navigating += OnWebViewNavigating;
            bindable.BindingContextChanged += OnBindingContextChanged;
        }

        protected override void OnDetachingFrom(WebView bindable)
        {
            _element = null;
            BindingContext = null;
            bindable.Navigating -= OnWebViewNavigating;
            bindable.BindingContextChanged -= OnBindingContextChanged;
        }

        private void OnBindingContextChanged(object sender, System.EventArgs e)
        {
            BindingContext = _element?.BindingContext;
        }

        private void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (NavigateCommand != null && NavigateCommand.CanExecute(e.Url))
            {
                NavigateCommand.Execute(e.Url);
            }
        }
    }
}
