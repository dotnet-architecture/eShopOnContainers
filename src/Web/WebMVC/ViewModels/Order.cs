using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopOnContainers.WebMVC.ViewModels.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Services.ModelDTOs;

namespace Microsoft.eShopOnContainers.WebMVC.ViewModels
{
    public class Order
    {
        public string OrderNumber {get;set;}

        public DateTime Date {get;set;}

        public string Status { get; set; }

        public decimal Total {get;set;}

        public string Description { get; set; }

        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }

        public string ZipCode { get; set; }
        [Required]
        [DisplayName("Card number")]
        public string CardNumber { get; set; }
        [Required]
        [DisplayName("Cardholder name")]
        public string CardHolderName { get; set; }

        public DateTime CardExpiration { get; set; }
        [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Expiration should match a valid MM/YY value")]
        [CardExpiration(ErrorMessage = "The card is expired"), Required]
        [DisplayName("Card expiration")]
        public string CardExpirationShort { get; set; }
        [Required]
        [DisplayName("Card security number")]
        public string CardSecurityNumber { get; set; }

        public int CardTypeId { get; set; }

        public string Buyer { get; set; }

        public List<SelectListItem> ActionCodeSelectList =>
           GetActionCodesByCurrentState();

        // See the property initializer syntax below. This
        // initializes the compiler generated field for this
        // auto-implemented property.
        public List<OrderItem> OrderItems { get; } = new List<OrderItem>();

        [Required]
        public Guid RequestId { get; set; }


        public void CardExpirationShortFormat()
        {
            CardExpirationShort = CardExpiration.ToString("MM/yy");
        }

        public void CardExpirationApiFormat()
        {
            var month = CardExpirationShort.Split('/')[0];
            var year = $"20{CardExpirationShort.Split('/')[1]}";

            CardExpiration = new DateTime(int.Parse(year), int.Parse(month), 1);
        }

        private List<SelectListItem> GetActionCodesByCurrentState()
        {
            var actions = new List<OrderProcessAction>();
            switch (Status?.ToLower())
            {
                case "paid":
                    actions.Add(OrderProcessAction.Ship);
                    break;
            }

            var result = new List<SelectListItem>();
            actions.ForEach(action =>
            {
                result.Add(new SelectListItem { Text = action.Name, Value = action.Code });
            });

            return result;
        }
    }

    public enum CardType
    {
        AMEX = 1
    }
}
