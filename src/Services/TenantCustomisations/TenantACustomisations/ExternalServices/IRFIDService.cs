using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantACustomisations.ExternalServices
{
    public interface IRFIDService
    {
        bool IsOrderRFIDTagged(int orderNumber);
    }
}
