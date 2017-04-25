using Xunit;
using eShopOnContainers.Core.ViewModels.Base;

namespace eShopOnContainers.UnitTests
{
	public class MockViewModelTests
	{
		[Fact]
		public void CheckValidationFailsWhenPropertiesAreEmptyTest()
		{
			ViewModelLocator.RegisterDependencies(true);
			var mockViewModel = new MockViewModel();

			bool isValid = mockViewModel.Validate();

			Assert.False(isValid);
		}

		[Fact]
		public void CheckValidationFailsWhenOnlyForenameHasDataTest()
		{
			ViewModelLocator.RegisterDependencies(true);
			var mockViewModel = new MockViewModel();
			mockViewModel.Forename.Value = "John";

			bool isValid = mockViewModel.Validate();

			Assert.False(isValid);
		}

		[Fact]
		public void CheckValidationPassesWhenOnlySurnameHasDataTest()
		{
			ViewModelLocator.RegisterDependencies(true);
			var mockViewModel = new MockViewModel();
			mockViewModel.Surname.Value = "Smith";

			bool isValid = mockViewModel.Validate();

			Assert.False(isValid);
		}

		[Fact]
		public void CheckValidationPassesWhenPropertiesHaveDataTest()
		{
			ViewModelLocator.RegisterDependencies(true);
			var mockViewModel = new MockViewModel();
			mockViewModel.Forename.Value = "John";
			mockViewModel.Surname.Value = "Smith";

			bool isValid = mockViewModel.Validate();

			Assert.True(isValid);
		}

		[Fact]
		public void SettingForenamePropertyShouldRaisePropertyChanged()
		{
			bool invoked = false;

			ViewModelLocator.RegisterDependencies(true);
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

			ViewModelLocator.RegisterDependencies(true);
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
