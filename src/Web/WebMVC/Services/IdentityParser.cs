using Microsoft.eShopOnContainers.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class IdentityParser:IIdentityParser<ApplicationUser>
    {
        public ApplicationUser Parse(IPrincipal principal)
        {
            var user = new ApplicationUser();
            var claims = (ClaimsPrincipal)principal;

            user.CardHolderName = (claims.Claims.Where(x => x.Type == "card_holder").Count() > 0) ? claims.Claims.First(x => x.Type == "card_holder").Value : "";
            user.CardNumber = (claims.Claims.Where(x => x.Type == "card_number").Count() > 0) ? claims.Claims.First(x => x.Type == "card_number").Value : "";
            user.Expiration = (claims.Claims.Where(x => x.Type == "card_expiration").Count() > 0) ? claims.Claims.First(x => x.Type == "card_expiration").Value : "";
            user.CardType = (claims.Claims.Where(x => x.Type == "missing").Count() > 0) ? int.Parse(claims.Claims.First(x => x.Type == "missing").Value) : 0;
            user.City = (claims.Claims.Where(x => x.Type == "address_city").Count() > 0) ? claims.Claims.First(x => x.Type == "address_city").Value : "";
            user.Country = (claims.Claims.Where(x => x.Type == "address_country").Count() > 0) ? claims.Claims.First(x => x.Type == "address_country").Value : "";
            user.Email = (claims.Claims.Where(x => x.Type == "email").Count() > 0) ? claims.Claims.First(x => x.Type == "email").Value : "";
            user.Id = (claims.Claims.Where(x => x.Type == "sub").Count() > 0) ? claims.Claims.First(x => x.Type == "sub").Value : "";
            user.LastName = (claims.Claims.Where(x => x.Type == "last_name").Count() > 0) ? claims.Claims.First(x => x.Type == "last_name").Value : "";
            user.Name = (claims.Claims.Where(x => x.Type == "name").Count() > 0) ? claims.Claims.First(x => x.Type == "name").Value : "";
            user.PhoneNumber = (claims.Claims.Where(x => x.Type == "phone_number").Count() > 0) ? claims.Claims.First(x => x.Type == "phone_number").Value : "";
            user.SecurityNumber = (claims.Claims.Where(x => x.Type == "card_security_number").Count() > 0) ? claims.Claims.First(x => x.Type == "card_security_number").Value : "";
            user.State = (claims.Claims.Where(x => x.Type == "address_state").Count() > 0) ? claims.Claims.First(x => x.Type == "address_state").Value : "";
            user.Street = (claims.Claims.Where(x => x.Type == "address_street").Count() > 0) ? claims.Claims.First(x => x.Type == "address_street").Value : "";
            user.ZipCode = (claims.Claims.Where(x => x.Type == "address_zip_code").Count() > 0) ? claims.Claims.First(x => x.Type == "address_zip_code").Value : "";

            return user;
        }
    }
}


