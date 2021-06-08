namespace Microsoft.eShopOnContainers.WebMVC.ViewModels
{
    public record BasketItem
    {
        public string Id { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal OldUnitPrice { get; init; }
        public int Quantity { get; init; }
        public string PictureUrl { get; init; }
    }
}
