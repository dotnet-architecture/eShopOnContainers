using System;
using TechTalk.SpecFlow;

namespace EShopApplicationWebSolution.StepDefinitions
{
    [Binding]
    public class Login_EShopApplicationSteps
    {
        [Given(@"the user is registered to EShop application")]
        public void GivenTheUserIsRegisteredToEShopApplication()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"the user is no registered to EShop application")]
        public void GivenTheUserIsNoRegisteredToEShopApplication()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"user launches EShop application")]
        public void WhenUserLaunchesEShopApplication()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"user clicks on ""(.*)"" button")]
        public void WhenUserClicksOnButton(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"user enters ""(.*)"" and ""(.*)""")]
        public void WhenUserEntersAnd(string p0, string p1)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"user clicks on ""(.*)""")]
        public void WhenUserClicksOn(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the user should be able to login to the application")]
        public void ThenTheUserShouldBeAbleToLoginToTheApplication()
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
