using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantManager.Models
{
    public class Customisation
    {
        public int CustomisationId { get; set; }
        public int TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
        public int MethodId { get; set; }
        public virtual Method Method { get; set; }
    }
}
