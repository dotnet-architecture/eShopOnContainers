using Microsoft.Practices.Unity;
using eShopOnContainers.Core.Services.Orders;
using eShopOnContainers.Core.Services.Products;
using eShopOnContainers.Core.ViewModels;
using eShopOnContainers.Services;
using System;

namespace eShopOnContainers.ViewModels.Base
{
    public class ViewModelLocator
    {
        private readonly IUnityContainer _unityContainer;

        private static readonly ViewModelLocator _instance = new ViewModelLocator();

        public static ViewModelLocator Instance
        {
            get
            {
                return _instance;
            }
        }

        protected ViewModelLocator()
        {
            _unityContainer = new UnityContainer();

            // services
            _unityContainer.RegisterType<IDialogService, DialogService>();
            RegisterSingleton<INavigationService, NavigationService>();
            _unityContainer.RegisterType<IProductsService, FakeProductsService>();
            _unityContainer.RegisterType<IOrdersService, FakeOrdersService>();

            // view models
            _unityContainer.RegisterType<CartViewModel>();
            _unityContainer.RegisterType<ProductsViewModel>();
            _unityContainer.RegisterType<LoginViewModel>();
            _unityContainer.RegisterType<MainViewModel>();
            _unityContainer.RegisterType<OrderDetailViewModel>();
            _unityContainer.RegisterType<OrdersViewModel>();
            _unityContainer.RegisterType<ProfileViewModel>();
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
