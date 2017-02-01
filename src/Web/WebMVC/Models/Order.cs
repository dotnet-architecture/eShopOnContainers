using Microsoft.eShopOnContainers.WebMVC.Models.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Models
{
    public class Order
    {
        public Order() {
            OrderItems = new List<OrderItem>();
        }

        public string OrderNumber {get;set;}

        public DateTime Date {get;set;}

        public string Status { get; set; }

        public decimal Total {get;set;}

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

        public List<OrderItem> OrderItems { get; }

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
    }

    public enum CardType
    {
        AMEX = 1
    }
}
