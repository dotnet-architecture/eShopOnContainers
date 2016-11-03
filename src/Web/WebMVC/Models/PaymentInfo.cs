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
        public int ExpirationMonth { get; set; }    //CCE: I would simplify with a string Expiration field so I guess we are not going to validate with real data. It's a demo.. 
        public int ExpirationYear { get; set; }     //CCE: Idem.
        public string CardHolderName { get; set; }
        public CardType CardType { get; set; }      //CCE: Discuss with team if this is needed for a demo. 
        public string Expiration { get; set; }      //CCE: Added to simplify.. 
    }

    public enum CardType:int
    {
        AMEX, 
        VISA
    }
}
