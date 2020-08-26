using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.ModelDTOs
{
    public class OrderDTO
    {
        public int OrderNumber { get; set; }

        public OrderDTO()
        {
            // Intenetionally left blank
        }

        public OrderDTO(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}
