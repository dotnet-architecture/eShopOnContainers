namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services;

public class CouponService : ICouponService
{
    public readonly HttpClient _httpClient;
    private readonly UrlsConfig _urls;
    private readonly ILogger<CouponService> _logger;

    public CouponService(HttpClient httpClient, IOptions<UrlsConfig> config, ILogger<CouponService> logger)
    {
        _urls = config.Value;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> CheckCouponByCodeNumberAsync(string codeNumber)
    {
        _logger.LogInformation("Call coupon api with codenumber: {codeNumber}", codeNumber);

        var url = new Uri($"{_urls.Catalog}/api/v1/coupon/{codeNumber}");

        var response = await _httpClient.GetAsync(url);

        _logger.LogInformation("Coupon api response: {@response}", response);

        return response;
    }
}
