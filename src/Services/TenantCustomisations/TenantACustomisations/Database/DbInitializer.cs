using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantACustomisations.ExternalServices;

namespace TenantACustomisations.Database
{
    public class DbInitializer
    {
        public void Initialize(TenantAContext context)
        {
            context.Database.EnsureCreated();

            if (context.ShippingInformation.Any())
            {
                return;
            }

            ShippingInformation shippingInformation = new ShippingInformation();
            shippingInformation.ShippingTime = DateTime.Today;
            shippingInformation.ArrivalTime = DateTime.Today.AddDays(2);
            shippingInformation.FragilityLevel = Fragility.Medium;
            shippingInformation.PriorityLevel = Priority.High;
            shippingInformation.ShippingInformationId = 1;
            shippingInformation.OrderNumber = "1";
            context.ShippingInformation.Add(shippingInformation);

            context.SaveChanges();


        }
    }
}
