using Xamarin.Forms;

namespace eShopOnContainers.Core.Views
{
    public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            AuthWebView.Navigating += OnAuthWebViewNavigating;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            AuthWebView.Navigating -= OnAuthWebViewNavigating;
        }

        private void OnAuthWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {

        }
    }
}
