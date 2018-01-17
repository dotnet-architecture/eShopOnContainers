using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.Catalog;
using eShopOnContainers.Core.Services.Settings;
using eShopOnContainers.Core.ViewModels;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.UnitTests.Mocks;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace eShopOnContainers.UnitTests
{
    public class CatalogViewModelTests
    {
        public CatalogViewModelTests()
        {
            ViewModelLocator.UpdateDependencies(true);
            ViewModelLocator.RegisterSingleton<ISettingsService, MockSettingsService>();
        }

        [Fact]
        public void AddCatalogItemCommandIsNotNullTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);
            Assert.NotNull(catalogViewModel.AddCatalogItemCommand);
        }

        [Fact]
        public void FilterCommandIsNotNullTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);
            Assert.NotNull(catalogViewModel.FilterCommand);
        }

        [Fact]
        public void ClearFilterCommandIsNotNullTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);
            Assert.NotNull(catalogViewModel.ClearFilterCommand);
        }

        [Fact]
        public void ProductsPropertyIsNullWhenViewModelInstantiatedTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);
            Assert.Null(catalogViewModel.Products);
        }

        [Fact]
        public void BrandsPropertyuIsNullWhenViewModelInstantiatedTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);
            Assert.Null(catalogViewModel.Brands);
        }

        [Fact]
        public void BrandPropertyIsNullWhenViewModelInstantiatedTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);
            Assert.Null(catalogViewModel.Brand);
        }

        [Fact]
        public void TypesPropertyIsNullWhenViewModelInstantiatedTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);
            Assert.Null(catalogViewModel.Types);
        }

        [Fact]
        public void TypePropertyIsNullWhenViewModelInstantiatedTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);
            Assert.Null(catalogViewModel.Type);
        }

        [Fact]
        public void IsFilterPropertyIsFalseWhenViewModelInstantiatedTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);
            Assert.False(catalogViewModel.IsFilter);
        }

        [Fact]
        public async Task ProductsPropertyIsNotNullAfterViewModelInitializationTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);

            await catalogViewModel.InitializeAsync(null);

            Assert.NotNull(catalogViewModel.Products);
        }

        [Fact]
        public async Task BrandsPropertyIsNotNullAfterViewModelInitializationTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);

            await catalogViewModel.InitializeAsync(null);

            Assert.NotNull(catalogViewModel.Brands);
        }

        [Fact]
        public async Task TypesPropertyIsNotNullAfterViewModelInitializationTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);

            await catalogViewModel.InitializeAsync(null);

            Assert.NotNull(catalogViewModel.Types);
        }

        [Fact]
        public async Task SettingProductsPropertyShouldRaisePropertyChanged()
        {
            bool invoked = false;
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);

            catalogViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals("Products"))
                    invoked = true;
            };
            await catalogViewModel.InitializeAsync(null);

            Assert.True(invoked);
        }

        [Fact]
        public async Task SettingBrandsPropertyShouldRaisePropertyChanged()
        {
            bool invoked = false;
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);

            catalogViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals("Brands"))
                    invoked = true;
            };
            await catalogViewModel.InitializeAsync(null);

            Assert.True(invoked);
        }

        [Fact]
        public async Task SettingTypesPropertyShouldRaisePropertyChanged()
        {
            bool invoked = false;
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);

            catalogViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals("Types"))
                    invoked = true;
            };
            await catalogViewModel.InitializeAsync(null);

            Assert.True(invoked);
        }

        [Fact]
        public void AddCatalogItemCommandSendsAddProductMessageTest()
        {
            bool messageReceived = false;
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);

            Xamarin.Forms.MessagingCenter.Subscribe<CatalogViewModel, CatalogItem>(this, MessageKeys.AddProduct, (sender, arg) =>
            {
                messageReceived = true;
            });
            catalogViewModel.AddCatalogItemCommand.Execute(null);

            Assert.True(messageReceived);
        }

        [Fact]
        public async Task FilterCommandSendsFilterMessageTest()
        {
            bool messageReceived = false;
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);
            await catalogViewModel.InitializeAsync(null);
            catalogViewModel.Brand = catalogViewModel.Brands.FirstOrDefault();
            catalogViewModel.Type = catalogViewModel.Types.FirstOrDefault();

            Xamarin.Forms.MessagingCenter.Subscribe<CatalogViewModel>(this, MessageKeys.Filter, (sender) =>
            {
                messageReceived = true;
            });
            catalogViewModel.FilterCommand.Execute(null);

            Assert.True(messageReceived);
        }

        [Fact]
        public async Task ClearFilterCommandResetsPropertiesTest()
        {
            var catalogService = new CatalogMockService();
            var catalogViewModel = new CatalogViewModel(catalogService);

            await catalogViewModel.InitializeAsync(null);
            catalogViewModel.ClearFilterCommand.Execute(null);

            Assert.Null(catalogViewModel.Brand);
            Assert.Null(catalogViewModel.Type);
            Assert.NotNull(catalogViewModel.Products);
        }
    }
}
