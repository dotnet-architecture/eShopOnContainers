using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Models
{
    public class Order
    {
        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        public string Id;
        public List<OrderItem> OrderItems { get; set; }
        public string OrderNumber
        {
            get
            {
                return string.Format("{0}/{1}-{2}", OrderDate.Year, OrderDate.Month, SequenceNumber);
            }
        }
        public int SequenceNumber { get; set; }
        public virtual string BuyerId { get; set; }
        public virtual Address ShippingAddress { get; set; }
        
        public virtual DateTime OrderDate { get; set; }
        public OrderState State { get; set; }

        //(CCE) public virtual Address BillingAddress { get; set; }
        //(CDLTLL) public virtual OrderStatus Status { get; set; }
    }
    
    public enum OrderState:int
    {
        InProcess = 0, 
        Delivered = 1
    }
}
