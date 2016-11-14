using System;
using SlideOverKit;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Views
{
    public partial class CatalogView : ContentPage, IMenuContainerPage
    {
        public CatalogView()
        {
            InitializeComponent();

            SlideMenu = new FiltersView();
        }

        public Action HideMenuAction
        {
            get;
            set;
        }

        public Action ShowMenuAction
        {
            get;
            set;
        }

        public SlideMenuView SlideMenu
        {
            get;
            set;
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            if (SlideMenu.IsShown)
            {
                HideMenuAction?.Invoke();
            }
            else
            {
                ShowMenuAction?.Invoke();
            }
        }
    }
}