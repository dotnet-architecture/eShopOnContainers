namespace Basket.FunctionalTests;

public class BasketScenarios : BasketScenarioBase
{
    private readonly HttpClient _httpClient;

    public BasketScenarios()
    {
        _httpClient = CreateClient();
    }

    [Fact]
    public async Task Post_basket_and_response_ok_status_code()
    {
        var content = new StringContent(BuildBasket(), UTF8Encoding.UTF8, "application/json");
        var uri = "/api/v1/basket/";
        var response = await _httpClient.PostAsync(uri, content);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_basket_and_response_ok_status_code()
    {
        var response = await _httpClient
            .GetAsync(Get.GetBasket(1));
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Send_Checkout_basket_and_response_ok_status_code()
    {
        var contentBasket = new StringContent(BuildBasket(), UTF8Encoding.UTF8, "application/json");

        await _httpClient
            .PostAsync(Post.Basket, contentBasket);

        var contentCheckout = new StringContent(BuildCheckout(), UTF8Encoding.UTF8, "application/json")
        {
             Headers = { { "x-requestid", Guid.NewGuid().ToString() } }
        };

        var response = await _httpClient
            .PostAsync(Post.CheckoutOrder, contentCheckout);

        response.EnsureSuccessStatusCode();
    }

    string BuildBasket()
    {
        var order = new CustomerBasket(AutoAuthorizeMiddleware.IDENTITY_ID);

        order.Items.Add(new BasketItem
        {
            ProductId = 1,
            ProductName = ".NET Bot Black Hoodie",
            UnitPrice = 10,
            Quantity = 1
        });

        return JsonSerializer.Serialize(order);
    }

    string BuildCheckout()
    {
        var checkoutBasket = new
        {
            City = "city",
            Street = "street",
            State = "state",
            Country = "coutry",
            ZipCode = "zipcode",
            CardNumber = "1234567890123456",
            CardHolderName = "CardHolderName",
            CardExpiration = DateTime.UtcNow.AddDays(1),
            CardSecurityNumber = "123",
            CardTypeId = 1,
            Buyer = "Buyer",
            RequestId = Guid.NewGuid()
        };

        return JsonSerializer.Serialize(checkoutBasket);
    }
}
