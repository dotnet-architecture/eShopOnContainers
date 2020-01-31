using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TenantManager.Models
{
    public class Method
    {
        public int MethodId { get; set; }
        public String MethodName { get; set; }
        public List<Customisation> Customisations { get; set; }
    }
}
