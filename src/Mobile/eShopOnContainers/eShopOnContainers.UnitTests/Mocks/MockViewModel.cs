using eShopOnContainers.Core.Validations;
using eShopOnContainers.Core.ViewModels.Base;

namespace eShopOnContainers.UnitTests
{
    public class MockViewModel : ViewModelBase
	{
		private ValidatableObject<string> _forename;
		private ValidatableObject<string> _surname;

		public ValidatableObject<string> Forename
		{
			get
			{
				return _forename;
			}
			set
			{
				_forename = value;
				RaisePropertyChanged(() => Forename);
			}
		}

		public ValidatableObject<string> Surname
		{
			get
			{
				return _surname;
			}
			set
			{
				_surname = value;
				RaisePropertyChanged(() => Surname);
			}
		}

		public MockViewModel()
		{
			_forename = new ValidatableObject<string>();
			_surname = new ValidatableObject<string>();

			_forename.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Forename is required." });
			_surname.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Surname name is required." });
		}

		public bool Validate()
		{
			bool isValidForename = _forename.Validate();
			bool isValidSurname = _surname.Validate();
			return isValidForename && isValidSurname;
		}
	}
}
