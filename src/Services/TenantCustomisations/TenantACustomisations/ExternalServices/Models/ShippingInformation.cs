using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantACustomisations.ExternalServices
{
    public class ShippingInformation
    {
        public int ShippingInformationId { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime ShippingTime { get; set; }
        public Priority PriorityLevel {get;set;}
        public Fragility FragilityLevel { get; set; }
        public String OrderNumber { get; set; }
    }
}
