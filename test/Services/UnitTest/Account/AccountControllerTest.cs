using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Controllers;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Account
{
    public class AccountControllerTest
    {
        private readonly Mock<IIdentityParser<ApplicationUser>> _identityParserMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public AccountControllerTest()
        {
            _identityParserMock = new Mock<IIdentityParser<ApplicationUser>>();
            _httpContextMock = new Mock<HttpContext>();
        }

        [Fact]
        public void Signin_with_token_success()
        {
            //Arrange
            var fakeCP = GenerateFakeClaimsIdentity();

            _httpContextMock.Setup(x => x.User)
                .Returns(new ClaimsPrincipal(fakeCP));
                       
            //Act
            var accountController = new AccountController(_identityParserMock.Object);
            accountController.ControllerContext.HttpContext = _httpContextMock.Object;
            var actionResult = accountController.SignIn("");

            //Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal(redirectResult.ActionName, "Index");
            Assert.Equal(redirectResult.ControllerName, "Catalog");
            Assert.Equal(accountController.ViewData["access_token"], "fakeToken");
        }

        private ClaimsIdentity GenerateFakeClaimsIdentity()
        {
            var ci = new ClaimsIdentity();
            ci.AddClaim(new Claim("access_token", "fakeToken"));
            return ci;
        }
    }
}
