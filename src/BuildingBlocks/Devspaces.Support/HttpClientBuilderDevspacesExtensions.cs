namespace Devspaces.Support;

public static class HttpClientBuilderDevspacesExtensions
{
    public static IHttpClientBuilder AddDevspacesSupport(this IHttpClientBuilder builder)
    {
        builder.AddHttpMessageHandler<DevspacesMessageHandler>();
        return builder;
    }
}
