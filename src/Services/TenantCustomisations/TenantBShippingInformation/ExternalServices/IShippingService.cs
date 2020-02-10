using TenantAShippingInformation.Models;

namespace TenantAShippingInformation.ExternalServices
{
    public interface IShippingService
    {
        ShippingInformation CalculateShippingInformation(int orderId);
    }
}
