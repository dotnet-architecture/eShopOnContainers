using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Models.Orders
{
    public class OrderItem : ExtendedBindableObject
    {
        private string _productImage;
        private string _productId;
        private Guid _orderId;
        private string _productName;
        private decimal _unitPrice;
        private int _quantity;
        private decimal _discount;
        private ObservableCollection<int> _numbers;

        public OrderItem()
        {
            Numbers = NumericHelper.GetNumericList();
        }

        public string ProductId
        {
            get { return _productId; }
            set
            {
                _productId = value;
                RaisePropertyChanged(() => ProductId);
            }
        }

        public Guid OrderId
        {
            get { return _orderId; }
            set
            {
                _orderId = value;
                RaisePropertyChanged(() => OrderId);
            }
        }

        public string ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
                RaisePropertyChanged(() => ProductName);
            }
        }

        public decimal UnitPrice
        {
            get { return _unitPrice; }
            set
            {
                _unitPrice = value;
                RaisePropertyChanged(() => UnitPrice);
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                RaisePropertyChanged(() => Quantity);
                RaisePropertyChanged(() => Total);

                MessagingCenter.Send(this, MessengerKeys.UpdateProduct);
            }
        }

        public decimal Discount
        {
            get { return _discount; }
            set
            {
                _discount = value;
                RaisePropertyChanged(() => Discount);
            }
        }

        public decimal Total { get { return Quantity * UnitPrice; } }

        public string ProductImage
        {
            get { return _productImage; }
            set
            {
                _productImage = value;
                RaisePropertyChanged(() => ProductImage);
            }
        }

        public ObservableCollection<int> Numbers
        {
            get { return _numbers; }
            set
            {
                _numbers = value;
                RaisePropertyChanged(() => Numbers);
            }
        }

        public override string ToString()
        {
            return String.Format("Product Id: {0}, Quantity: {1}", ProductId, Quantity);
        }
    }
}