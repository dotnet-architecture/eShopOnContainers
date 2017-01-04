using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Models.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class CardExpirationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            var month = value.ToString().Split('/')[0];
            var year = $"20{value.ToString().Split('/')[1]}";
            DateTime d = new DateTime(int.Parse(year), int.Parse(month), 1);

            return d > DateTime.UtcNow;
        }
    }
}
