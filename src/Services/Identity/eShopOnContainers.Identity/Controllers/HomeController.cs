// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using eShopOnContainers.Identity;
using IdentityServer4.Quickstart.UI.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IOptions<AppSettings> _settings;

        public HomeController(IIdentityServerInteractionService interaction, IOptions<AppSettings> settings)
        {
            _interaction = interaction;
            _settings = settings;
        }

        public IActionResult Index(string returnUrl)
        {
            if (returnUrl != "")
                return Redirect(_settings.Value.MvcClient);

            return View();
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return View("Error", vm);
        }
    }
}