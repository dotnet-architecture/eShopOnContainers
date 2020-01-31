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
            
            var method1 = new Method { MethodName = "OrderStatusChangedToSubmittedIntegrationEvent" };
            var method2 = new Method { MethodName = "OrderStatusChangedToAwaitingValidationIntegrationEvent" };

            var methods = new[]
            {
                method1,
                method2
            };

            foreach(Method m in methods)
            {
                context.Method.Add(m);
            }
            
            var customisations = new[]
            {
                new Customisation {Tenant=tenant1, Method=method1 },
                new Customisation {Tenant=tenant1, Method=method2}
            };

            foreach(Customisation c in customisations)
            {
                context.Add(c);
            }

            context.SaveChanges();

        }
    }
}
