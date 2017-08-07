namespace eShopOnContainers.Core.Models.Orders
{
    public class CancelOrderCommand
    {
        public int OrderNumber { get; }

        public CancelOrderCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}