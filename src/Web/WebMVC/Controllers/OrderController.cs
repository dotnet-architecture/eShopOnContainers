using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.Models;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    public class OrderController : Controller
    {
        private IOrderingService _orderSvc;
        public OrderController(IOrderingService orderSvc)
        {
            _orderSvc = orderSvc;
        }

        public IActionResult AddToCart()
        {
            return View();
        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Index(Order item)
        {
            _orderSvc.AddOrder(item);
            return View(_orderSvc.GetOrders());
        }
    }
}