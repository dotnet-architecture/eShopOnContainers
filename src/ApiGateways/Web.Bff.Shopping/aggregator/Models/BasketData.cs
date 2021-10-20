namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;

public class BasketData
{
    public string BuyerId { get; set; }

    public List<BasketDataItem> Items { get; set; } = new();

    public BasketData()
    {
    }

    public BasketData(string buyerId)
    {
        BuyerId = buyerId;
    }
}

