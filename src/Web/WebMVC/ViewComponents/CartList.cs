﻿namespace Microsoft.eShopOnContainers.WebMVC.ViewComponents;

public class CartList : ViewComponent
{
    private readonly IBasketService _cartSvc;

    public CartList(IBasketService cartSvc) => _cartSvc = cartSvc;

    public async Task<IViewComponentResult> InvokeAsync(ApplicationUser user)
    {
        var vm = new Basket();
        try
        {
            vm = await GetItemsAsync(user);
            return View(vm);
        }
        catch (Exception ex)
        {
            ViewBag.BasketInoperativeMsg = $"Basket Service is inoperative, please try later on. ({ex.GetType().Name} - {ex.Message}))";
        }

        return View(vm);
    }

    private Task<Basket> GetItemsAsync(ApplicationUser user) => _cartSvc.GetBasket(user);
}
