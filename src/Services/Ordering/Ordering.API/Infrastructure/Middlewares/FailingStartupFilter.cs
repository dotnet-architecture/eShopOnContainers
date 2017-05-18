using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Infrastructure.Middlewares
{
    public class FailingStartupFilter : IStartupFilter
    {
        public FailingStartupFilter()
        {
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseFailingMiddleware();
                next(app);
            };
        }
    }
}
