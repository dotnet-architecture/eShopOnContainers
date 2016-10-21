using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Models
{
    public class PaymentInfo
    {
        public Guid Id { get; set; }
        public string CardNumber {get;set;}
        public string SecurityNumber { get; set; }
        public int ExpirationMonth { get; set; } 
        public int ExpirationYear { get; set; }
        public string CardHolderName { get; set; }
        public CardType CardType { get; set; }
    }

    public enum CardType:int
    {
        AMEX, 
        VISA
    }
}
