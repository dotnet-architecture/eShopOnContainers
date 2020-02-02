using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TenantManager.Models
{
    public class Customisation
    {
        public int CustomisationId { get; set; }
        public String CustomisationUrl { get; set; }
        
        //Foreign keys
        public int TenantId { get; set; }
        public int MethodId { get; set; }

        
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        [ForeignKey("MethodId")]
        public virtual Method Method { get; set; }
    }
}
