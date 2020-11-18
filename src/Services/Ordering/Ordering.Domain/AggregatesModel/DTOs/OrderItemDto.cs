using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.AggregatesModel.DTOs
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public string PictureUrl { get; set; }
        public int Units { get; set; } = 1;
    }
}
