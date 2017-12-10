using System;
using System.ComponentModel.DataAnnotations;

namespace WebMVC.ViewModels.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class LatitudeCoordinate : ValidationAttribute
    {
        protected override ValidationResult
                IsValid(object value, ValidationContext validationContext)
        {
            double coordinate;
            if (!double.TryParse(value.ToString(), out coordinate) || (coordinate < -90 || coordinate > 90))
            {
                return new ValidationResult
                    ("Latitude must be between -90 and 90 degrees inclusive.");
            }

            return ValidationResult.Success;
        }
    }
}
