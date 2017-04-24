using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.ViewModels.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class CardExpirationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            var monthString = value.ToString().Split('/')[0];
            var yearString = $"20{value.ToString().Split('/')[1]}";
            // Use the 'out' variable initializer to simplify 
            // the logic of validating the expiration date
            if ((int.TryParse(monthString, out var month)) &&
                (int.TryParse(yearString, out var year)))
            {
                DateTime d = new DateTime(year, month, 1);

                return d > DateTime.UtcNow;
            } else
            {
                return false;
            }
        }
    }
}
