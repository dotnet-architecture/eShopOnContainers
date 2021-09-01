namespace Microsoft.eShopOnContainers.Services.Basket.API.Model;

public class CustomerBasket
{
    public string BuyerId { get; set; }

    public List<BasketItem> Items { get; set; } = new();

    public CustomerBasket()
    {

    }

    public CustomerBasket(string customerId)
    {
        BuyerId = customerId;
    }
}

