using System.Linq;
using TenantManager.Models;

namespace TenantManager.Database
{
    public static class DbInitializer
    {
        public static void Initialize(TenantManagerContext context)
        {
            context.Database.EnsureCreated();

            if (context.Tenant.Any())
            {
                return;
            }

            var tenant1 = new Tenant { TenantName = "Tekna" };
            context.Tenant.Add(tenant1);
            
            var method2 = new Method { MethodName = "OrderStatusChangedToAwaitingValidationIntegrationEvent" };

            var methods = new[]
            {
                method2
            };

            foreach(Method m in methods)
            {
                context.Method.Add(m);
            }
            
            var customisations = new[]
            {
                new Customisation {Tenant=tenant1, Method=method2, CustomisationUrl = @"http://tenantacustomisation/api/savedevents"}
            };

            foreach(Customisation c in customisations)
            {
                context.Add(c);
            }

            context.SaveChanges();

        }
    }
}
