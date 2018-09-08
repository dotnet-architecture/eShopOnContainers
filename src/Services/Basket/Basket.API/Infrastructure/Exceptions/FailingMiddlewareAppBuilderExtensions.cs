using Microsoft.AspNetCore.Builder;
using System;

namespace Basket.API.Infrastructure.Middlewares
{
	public static class FailingMiddlewareAppBuilderExtensions
	{
		public static IApplicationBuilder UseFailingMiddleware(this IApplicationBuilder builder) 
			=> UseFailingMiddleware(builder, null);
		public static IApplicationBuilder UseFailingMiddleware(this IApplicationBuilder builder, Action<FailingOptions> action)
		{
			var options = new FailingOptions();
			action?.Invoke(options);
			builder.UseMiddleware<FailingMiddleware>(options);
			return builder;
		}
	}
}
