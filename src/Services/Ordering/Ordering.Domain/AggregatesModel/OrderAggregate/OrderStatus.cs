namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate
{
    using Seedwork;
    using SeedWork;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OrderStatus
        : Enumeration
    {
        public static OrderStatus InProcess = new OrderStatus(1, nameof(InProcess).ToLowerInvariant());
        public static OrderStatus Shipped = new OrderStatus(2, nameof(Shipped).ToLowerInvariant());
        public static OrderStatus Canceled = new OrderStatus(3, nameof(Canceled).ToLowerInvariant());

        protected OrderStatus()
        {
        }

        public OrderStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<OrderStatus> List()
        {
            return new[] { InProcess, Shipped, Canceled };
        }

        public static OrderStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
