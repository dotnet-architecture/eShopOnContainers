using EShopApplicationWebSolution.Properties;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EShopApplicationWebSolution.StepDefinitions
{

    using Bdd.Core.Entities;
    using Bdd.Core.Utils;
    using Bdd.Core.Web.Executors;
    using Bdd.Core.Web.StepDefinitions;
    using Bdd.Core.Web.Utils;
    using NUnit.Framework;
    using Ocaramba;
    using SmartFormat;
    using TechTalk.SpecFlow;

    [Binding]
    public class Login_EShopApplicationSteps : WebStepDefinitionBase
    {

        [When(@"user launches EShop application")]
        public void WhenUserLaunchesEShopApplication()
        {
            var url = Smart.Format(BaseConfiguration.GetUrlValue);
            this.Get<UrlPage>().NavigateToUrl(url);
        }

        [When(@"user clicks on ""(.*)""")]
        public async Task WhenUserClicksOnAsync(string linkText)
        {
            this.Get<ElementPage>().GetElement(nameof(Resources.DivLinks), linkText).Click();
        }

        [When(@"user clicks on ""(.*)"" option")]
        public void WhenUserClicksOnOption(string linkText)
        {
            this.Get<ElementPage>().GetElement(nameof(Resources.DivLinks), linkText).Click();
        }

        [When(@"user enter Email and Password of ""(.*)""")]
        public void WhenIEnterEmailAndPasswordOf(string user)
        {
            var userdata = this.ScenarioContext.GetCredential<Credentials>(this.FeatureContext, user, "input=Credentials.xlsx");
            this.Get<ElementPage>().EnterText(nameof(Resources.Id), userdata.User, "Email");
            this.Get<ElementPage>().EnterText(nameof(Resources.Id), userdata.Password, "Password");
            this.ScenarioContext[user] = userdata.User;
        }

        [When(@"user click on ""(.*)"" button")]
        public void WhenIClickOnButton(string buttonText)
        {
            this.Get<ElementPage>().GetElement(nameof(Resources.ButtonByText), buttonText).Click();
        }

        [Then(@"verify if ""(.*)"" is logged-in")]
        public void ThenIVerifyIfIsLogged_In(string user)
        {
            var addToCartPresent = this.Get<ElementPage>().CheckIfElementIsPresent(nameof(Resources.ButtonByText), 30, "ADD TO CART");
            var isloggedIn = this.Get<ElementPage>().CheckIfElementIsPresent(nameof(Resources.DivLinks), 30, this.ScenarioContext[user].ToString());

            Assert.IsTrue(addToCartPresent&& isloggedIn, $"Login Falied");
        }

        [Given(@"the user is no registered to EShop application")]
        public void GivenTheUserIsNoRegisteredToEShopApplication()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the user should not be able to login to the application")]
        public void ThenTheUserShouldNotBeAbleToLoginToTheApplication()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
