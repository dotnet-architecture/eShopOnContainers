﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
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
        private readonly Mock<HttpContext> _httpContextMock;

        public AccountControllerTest()
        {
            _httpContextMock = new Mock<HttpContext>();
        }

        [Fact]
        public void Signin_with_token_success()
        {
            //Arrange
            var fakeCP = GenerateFakeClaimsIdentity();
            var mockAuth = new Mock<AuthenticationManager>();

            _httpContextMock.Setup(x => x.User)
                .Returns(new ClaimsPrincipal(fakeCP));

            _httpContextMock.Setup(c => c.Authentication)
                .Returns(mockAuth.Object);

            //Act
            var accountController = new AccountController();
            accountController.ControllerContext.HttpContext = _httpContextMock.Object;
            var actionResult = accountController.SignIn("").Result;

            //Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal(redirectResult.ActionName, "Index");
            Assert.Equal(redirectResult.ControllerName, "Catalog");
        }

        private ClaimsIdentity GenerateFakeClaimsIdentity()
        {
            var ci = new ClaimsIdentity();
            ci.AddClaim(new Claim("access_token", "fakeToken"));
            return ci;
        }
    }
}
