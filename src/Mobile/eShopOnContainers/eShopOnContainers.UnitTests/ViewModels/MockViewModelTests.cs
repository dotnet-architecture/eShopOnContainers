using eShopOnContainers.Core.ViewModels.Base;
using Xunit;

namespace eShopOnContainers.UnitTests
{
    public class MockViewModelTests
    {
        public MockViewModelTests()
        {
            ViewModelLocator.UpdateDependencies(true);
        }

        [Fact]
        public void CheckValidationFailsWhenPropertiesAreEmptyTest()
        {
            var mockViewModel = new MockViewModel();

            bool isValid = mockViewModel.Validate();

            Assert.False(isValid);
            Assert.Null(mockViewModel.Forename.Value);
            Assert.Null(mockViewModel.Surname.Value);
            Assert.False(mockViewModel.Forename.IsValid);
            Assert.False(mockViewModel.Surname.IsValid);
            Assert.NotEmpty(mockViewModel.Forename.Errors);
            Assert.NotEmpty(mockViewModel.Surname.Errors);
        }

        [Fact]
        public void CheckValidationFailsWhenOnlyForenameHasDataTest()
        {
            var mockViewModel = new MockViewModel();
            mockViewModel.Forename.Value = "John";

            bool isValid = mockViewModel.Validate();

            Assert.False(isValid);
            Assert.NotNull(mockViewModel.Forename.Value);
            Assert.Null(mockViewModel.Surname.Value);
            Assert.True(mockViewModel.Forename.IsValid);
            Assert.False(mockViewModel.Surname.IsValid);
            Assert.Empty(mockViewModel.Forename.Errors);
            Assert.NotEmpty(mockViewModel.Surname.Errors);
        }

        [Fact]
        public void CheckValidationPassesWhenOnlySurnameHasDataTest()
        {
            var mockViewModel = new MockViewModel();
            mockViewModel.Surname.Value = "Smith";

            bool isValid = mockViewModel.Validate();

            Assert.False(isValid);
            Assert.Null(mockViewModel.Forename.Value);
            Assert.NotNull(mockViewModel.Surname.Value);
            Assert.False(mockViewModel.Forename.IsValid);
            Assert.True(mockViewModel.Surname.IsValid);
            Assert.NotEmpty(mockViewModel.Forename.Errors);
            Assert.Empty(mockViewModel.Surname.Errors);
        }

        [Fact]
        public void CheckValidationPassesWhenBothPropertiesHaveDataTest()
        {
            var mockViewModel = new MockViewModel();
            mockViewModel.Forename.Value = "John";
            mockViewModel.Surname.Value = "Smith";

            bool isValid = mockViewModel.Validate();

            Assert.True(isValid);
            Assert.NotNull(mockViewModel.Forename.Value);
            Assert.NotNull(mockViewModel.Surname.Value);
            Assert.True(mockViewModel.Forename.IsValid);
            Assert.True(mockViewModel.Surname.IsValid);
            Assert.Empty(mockViewModel.Forename.Errors);
            Assert.Empty(mockViewModel.Surname.Errors);
        }

        [Fact]
        public void SettingForenamePropertyShouldRaisePropertyChanged()
        {
            bool invoked = false;
            var mockViewModel = new MockViewModel();

            mockViewModel.Forename.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals("Value"))
                    invoked = true;
            };
            mockViewModel.Forename.Value = "John";

            Assert.True(invoked);
        }

        [Fact]
        public void SettingSurnamePropertyShouldRaisePropertyChanged()
        {
            bool invoked = false;
            var mockViewModel = new MockViewModel();

            mockViewModel.Surname.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals("Value"))
                    invoked = true;
            };
            mockViewModel.Surname.Value = "Smith";

            Assert.True(invoked);
        }
    }
}
