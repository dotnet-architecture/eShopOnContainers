﻿using System.Threading.Tasks;
using eShopOnContainers.ViewModels.Base;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.Catalog;
using System.Windows.Input;
using System.Linq;
using eShopOnContainers.Core.Services.Basket;
using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Core.Services.User;

namespace eShopOnContainers.Core.ViewModels
{
    public class CatalogViewModel : ViewModelBase
    {
        private ObservableCollection<CatalogItem> _products;
        private ObservableCollection<CatalogBrand> _brands;
        private CatalogBrand _brand;
        private ObservableCollection<CatalogType> _types;
        private CatalogType _type;

        private IBasketService _basketService;
        private ICatalogService _productsService;
        private IUserService _userService;

        public CatalogViewModel(
            IBasketService basketService,
            ICatalogService productsService,
            IUserService userService)
        {
            _basketService = basketService;
            _productsService = productsService;
            _userService = userService;
        }

        public ObservableCollection<CatalogItem> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                RaisePropertyChanged(() => Products);
            }
        }

        public ObservableCollection<CatalogBrand> Brands
        {
            get { return _brands; }
            set
            {
                _brands = value;
                RaisePropertyChanged(() => Brands);
            }
        }

        public CatalogBrand Brand
        {
            get { return _brand; }
            set
            {
                _brand = value;
                RaisePropertyChanged(() => Brand);
                RaisePropertyChanged(() => IsFilter);
            }
        }

        public ObservableCollection<CatalogType> Types
        {
            get { return _types; }
            set
            {
                _types = value;
                RaisePropertyChanged(() => Types);
            }
        }

        public CatalogType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged(() => Type);
                RaisePropertyChanged(() => IsFilter);
            }
        }

        public bool IsFilter { get { return Brand != null || Type != null; } }

        public ICommand AddCatalogItemCommand => new Command<CatalogItem>(AddCatalogItem);

        public ICommand FilterCommand => new Command(Filter);

        public ICommand ClearFilterCommand => new Command(ClearFilter);

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            // Get Catalog, Brands and Types
            Products = await _productsService.GetCatalogAsync();
            Brands = await _productsService.GetCatalogBrandAsync();
            Types = await _productsService.GetCatalogTypeAsync();

            IsBusy = false;
        }

        private void AddCatalogItem(CatalogItem catalogItem)
        {
            // Add new item to Basket
            MessagingCenter.Send(this, MessageKeys.AddProduct, catalogItem);
        }

        private async void Filter()
        {
            if (Brand == null && Type == null)
            {
                return;
            }

            IsBusy = true;

            // Filter catalog products
            MessagingCenter.Send(this, MessageKeys.Filter);
            Products = await _productsService.FilterAsync(Brand.Id, Type.Id);

            IsBusy = false;
        }

        private async void ClearFilter()
        {
            IsBusy = true;

            Brand = null;
            Type = null;
            Products = await _productsService.GetCatalogAsync();

            IsBusy = false;
        }
    }
}