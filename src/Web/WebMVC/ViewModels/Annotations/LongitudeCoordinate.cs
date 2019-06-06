using System;
using System.ComponentModel.DataAnnotations;

namespace WebMVC.ViewModels.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class LongitudeCoordinate : ValidationAttribute
    {
        protected override ValidationResult
                IsValid(object value, ValidationContext validationContext)
        {
            double coordinate;
            if (!double.TryParse(value.ToString(), out coordinate) || (coordinate < -180 || coordinate > 180))
            {
                return new ValidationResult
                    ("Longitude must be between -180 and 180 degrees inclusive.");
            }

            return ValidationResult.Success;            
        }
    }
}
