using System;
using TenantAShippingInformation.Models;

namespace TenantAShippingInformation.ExternalServices
{
    public class MockedShippingService : IShippingService
    {
        public ShippingInformation CalculateShippingInformation(int orderId)
        {
            ShippingInformation shippingInformation = new ShippingInformation();
            shippingInformation.ShippingTime = DateTime.Today;
            shippingInformation.ArrivalTime = DateTime.Today.AddDays(2);
            shippingInformation.FragilityLevel = Fragility.Medium;
            shippingInformation.PriorityLevel = Priority.High;
            shippingInformation.OrderNumber = orderId.ToString();

            return shippingInformation;
        }
    }
}
