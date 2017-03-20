using Microsoft.Practices.Unity;
using eShopOnContainers.Core.ViewModels;
using eShopOnContainers.Services;
using System;
using eShopOnContainers.Core.Services.Catalog;
using eShopOnContainers.Core.Services.OpenUrl;
using eShopOnContainers.Core.Services.RequestProvider;
using eShopOnContainers.Core.Services.Basket;
using eShopOnContainers.Core.Services.Identity;
using eShopOnContainers.Core.Services.Order;
using eShopOnContainers.Core.Services.User;

namespace eShopOnContainers.ViewModels.Base
{
    public class ViewModelLocator
    {
        private bool _useMockService;
        private readonly IUnityContainer _unityContainer;

        private static readonly ViewModelLocator _instance = new ViewModelLocator();

        public static ViewModelLocator Instance
        {
            get { return _instance; }
        }

        public bool UseMockService
        {
            get { return _useMockService; }
            set { _useMockService = value; ; }
        }

        protected ViewModelLocator()
        {
            _unityContainer = new UnityContainer();

            // Services
            _unityContainer.RegisterType<IDialogService, DialogService>();
            RegisterSingleton<INavigationService, NavigationService>();
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

        public void UpdateDependencies(bool useMockServices)
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
        
        public T Resolve<T>()
        {
            return _unityContainer.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _unityContainer.Resolve(type);
        }

        public void Register<T>(T instance)
        {
            _unityContainer.RegisterInstance<T>(instance);
        }

        public void Register<TInterface, T>() where T : TInterface
        {
            _unityContainer.RegisterType<TInterface, T>();
        }

        public void RegisterSingleton<TInterface, T>() where T : TInterface
        {
            _unityContainer.RegisterType<TInterface, T>(new ContainerControlledLifetimeManager());
        }
    }
}