using TenantBShippingInformation.Models;

namespace TenantBShippingInformation.ExternalServices
{
    public interface IShippingService
    {
        ShippingInformation CalculateShippingInformation(int orderId);
    }
}
