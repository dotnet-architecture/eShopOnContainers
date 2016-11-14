using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.ViewModels.Base;

namespace eShopOnContainers.Core.ViewModels
{
    public class OrderDetailViewModel : ViewModelBase
    {
        private Order _order;

        public Order Order
        {
            get { return _order; }
            set
            {
                _order = value;
                RaisePropertyChanged(() => Order);
            }
        }

        public override Task InitializeAsync(object navigationData)
        {
            if(navigationData is Order)
            {
                Order = navigationData as Order;
            }

            return base.InitializeAsync(navigationData);
        }
    }
}