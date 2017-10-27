using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Controllers;
using Moq;
using System.Security.Claims;
using Xunit;

namespace UnitTest.Account
{
    public class AccountControllerTest
    {
        private readonly Mock<HttpContext> _httpContextMock;

        public AccountControllerTest()
        {
            _httpContextMock = new Mock<HttpContext>();
        }

        /* TBD: Find a way to mock HttpContext GetTokenAsync method */
        //[Fact]
        //public void Signin_with_token_success()
        //{
        //    //Arrange
        //    var fakeCP = GenerateFakeClaimsIdentity();
        //    var mockAuth = new Mock<AuthenticationManager>();

        //    _httpContextMock.Setup(x => x.User)
        //        .Returns(new ClaimsPrincipal(fakeCP));

        //    _httpContextMock.Setup(c => c.Authentication)
        //        .Returns(mockAuth.Object);

        //    //Act
        //    var accountController = new AccountController();
        //    accountController.ControllerContext.HttpContext = _httpContextMock.Object;
        //    var actionResult = accountController.SignIn("").Result;

        //    //Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);
        //    Assert.Equal(redirectResult.ActionName, "Index");
        //    Assert.Equal(redirectResult.ControllerName, "Catalog");
        //}

        private ClaimsIdentity GenerateFakeClaimsIdentity()
        {
            var ci = new ClaimsIdentity();
            ci.AddClaim(new Claim("access_token", "fakeToken"));
            return ci;
        }
    }
}
