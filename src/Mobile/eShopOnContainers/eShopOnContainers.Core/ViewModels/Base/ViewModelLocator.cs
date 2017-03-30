using Microsoft.Practices.Unity;
using eShopOnContainers.Services;
using System;
using System.Globalization;
using System.Reflection;
using eShopOnContainers.Core.Services.Catalog;
using eShopOnContainers.Core.Services.OpenUrl;
using eShopOnContainers.Core.Services.RequestProvider;
using eShopOnContainers.Core.Services.Basket;
using eShopOnContainers.Core.Services.Identity;
using eShopOnContainers.Core.Services.Order;
using eShopOnContainers.Core.Services.User;
using Xamarin.Forms;

namespace eShopOnContainers.Core.ViewModels.Base
{
    public static class ViewModelLocator
    {
		private static readonly IUnityContainer _unityContainer = new UnityContainer();

		public static readonly BindableProperty AutoWireViewModelProperty =
			BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

		public static bool GetAutoWireViewModel(BindableObject bindable)
		{
			return (bool)bindable.GetValue(ViewModelLocator.AutoWireViewModelProperty);
		}

		public static void SetAutoWireViewModel(BindableObject bindable, bool value)
		{
			bindable.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
		}

		public static bool UseMockService { get; set; }

		public static void Initialize()
		{
			// Services
			_unityContainer.RegisterType<IDialogService, DialogService>();
			_unityContainer.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
			_unityContainer.RegisterType<IOpenUrlService, OpenUrlService>();
			_unityContainer.RegisterType<IRequestProvider, RequestProvider>();
			_unityContainer.RegisterType<IIdentityService, IdentityService>();
			_unityContainer.RegisterType<ICatalogService, CatalogMockService>();
			_unityContainer.RegisterType<IBasketService, BasketMockService>();
			_unityContainer.RegisterType<IUserService, UserMockService>();

			// View models
			_unityContainer.RegisterType<BasketViewModel>();
			_unityContainer.RegisterType<CatalogViewModel>();
			_unityContainer.RegisterType<CheckoutViewModel>();
			_unityContainer.RegisterType<LoginViewModel>();
			_unityContainer.RegisterType<MainViewModel>();
			_unityContainer.RegisterType<OrderDetailViewModel>();
			_unityContainer.RegisterType<ProfileViewModel>();
			_unityContainer.RegisterType<SettingsViewModel>();
		}

		public static void UpdateDependencies(bool useMockServices)
		{
			// Change injected dependencies
			if (useMockServices)
			{
				_unityContainer.RegisterInstance<ICatalogService>(new CatalogMockService());
				_unityContainer.RegisterInstance<IBasketService>(new BasketMockService());
				_unityContainer.RegisterInstance<IOrderService>(new OrderMockService());
				_unityContainer.RegisterInstance<IUserService>(new UserMockService());

				UseMockService = true;
			}
			else
			{
				var requestProvider = Resolve<IRequestProvider>();
				_unityContainer.RegisterInstance<ICatalogService>(new CatalogService(requestProvider));
				_unityContainer.RegisterInstance<IBasketService>(new BasketService(requestProvider));
				_unityContainer.RegisterInstance<IOrderService>(new OrderService(requestProvider));
				_unityContainer.RegisterInstance<IUserService>(new UserService(requestProvider));

				UseMockService = false;
			}
		}

		public static T Resolve<T>()
		{
			return _unityContainer.Resolve<T>();
		}

		private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var view = bindable as Element;
			if (view == null)
			{
				return;
			}

			var viewType = view.GetType();
			var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
			var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
			var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

			var viewModelType = Type.GetType(viewModelName);
			if (viewModelType == null)
			{
				return;
			}
			var viewModel = _unityContainer.Resolve(viewModelType);
			view.BindingContext = viewModel;
		}
	}
}