﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.HealthChecks;
using WebStatus.Viewmodels;

namespace WebStatus.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHealthCheckService _healthCheckSvc;
        public HomeController(IHealthCheckService checkSvc)
        {
            _healthCheckSvc = checkSvc;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _healthCheckSvc.CheckHealthAsync();

            var data = new HealthStatusViewModel(result.CheckStatus);

            foreach (var checkResult in result.Results)
            {
                data.AddResult(checkResult.Key, checkResult.Value);
            }

            return View(data);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
