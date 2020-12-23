namespace Microsoft.eShopOnContainers.WebMVC.ViewModels.CartViewModels
{
    public class CartComponentViewModel
    {
        public int ItemsCount { get; set; }
        public string Disabled => (ItemsCount == 0) ? "is-disabled" : "";
    }
}
