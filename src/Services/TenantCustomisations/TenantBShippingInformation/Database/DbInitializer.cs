using System;
using System.Linq;
using TenantAShippingInformation.Models;

namespace TenantAShippingInformation.Database
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
