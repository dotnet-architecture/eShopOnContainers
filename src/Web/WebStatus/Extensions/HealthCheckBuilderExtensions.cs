using Microsoft.Extensions.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebStatus.Extensions
{
    public static class HealthCheckBuilderExtensions
    {
        public static HealthCheckBuilder AddUrlCheckIfNotNull(this HealthCheckBuilder builder, string url, TimeSpan cacheDuration)
        {
            if (!string.IsNullOrEmpty(url))
            {
                builder.AddUrlCheck(url, cacheDuration);
            }

            return builder;
        }

    }
}
