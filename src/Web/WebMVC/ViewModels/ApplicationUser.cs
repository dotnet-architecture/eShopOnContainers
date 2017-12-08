using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.eShopOnContainers.WebMVC.ViewModels
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string CardNumber { get; set; }
        public string SecurityNumber { get; set; }
        public string Expiration { get; set; }
        public string CardHolderName { get; set; }
        public int CardType { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string ZipCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
