using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            var tenant1 = new Tenant() { TenantName = "Tekna" };
            var tenant2 = new Tenant() { TenantName = "NITO" };
            var tenant3 = new Tenant() { TenantName = "LO" };

            var tenants = new Tenant[]
            {
                tenant1,
                tenant2,
                tenant3
            };
          
            foreach(Tenant t in tenants)
            {
                context.Tenant.Add(t);
            }

            context.SaveChanges();

            var method1 = new Method() { MethodName = "GetPrice" };
            var method2 = new Method() { MethodName = "GetItem" };

            var methods = new Method[]
            {
                method1,
                method2
            };

            foreach(Method m in methods)
            {
                context.Method.Add(m);
            }

            context.SaveChanges();

            var customisations = new Customisation[]
            {
                new Customisation(){Tenant=tenant1, Method=method1 },
                new Customisation(){Tenant=tenant1, Method=method2},
                new Customisation(){Tenant=tenant2, Method=method1 }
            };

            foreach(Customisation c in customisations)
            {
                context.Add(c);
            }

            context.SaveChanges();

        }
    }
}
