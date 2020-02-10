using Ordering.API.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantACustomisations.ExternalServices
{
    public interface IShippingService
    {
        ShippingInformation CalculateShippingInformation(int orderId);
    }
}
