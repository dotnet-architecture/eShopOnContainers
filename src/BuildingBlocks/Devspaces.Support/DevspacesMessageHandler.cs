namespace Devspaces.Support;

public class DevspacesMessageHandler : DelegatingHandler
{
    private const string DevspacesHeaderName = "azds-route-as";
    private readonly IHttpContextAccessor _httpContextAccessor;
    public DevspacesMessageHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var req = _httpContextAccessor.HttpContext.Request;

        if (req.Headers.ContainsKey(DevspacesHeaderName))
        {
            request.Headers.Add(DevspacesHeaderName, req.Headers[DevspacesHeaderName] as IEnumerable<string>);
        }
        return base.SendAsync(request, cancellationToken);
    }
}
