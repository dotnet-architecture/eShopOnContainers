using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Models.Basket
{
    public class BasketItem : ExtendedBindableObject
    {
        private string _id;
        private string _productId;
        private string _productName;
        private decimal _unitPrice;
        private int _quantity;
        private string _pictureUrl;
        private ObservableCollection<int> _numbers;

        public BasketItem()
        {
            Numbers = NumericHelper.GetNumericList();
        }

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(() => Id);
            }
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

        public string PictureUrl
        {
            get { return _pictureUrl; }
            set
            {
                _pictureUrl = value;
                RaisePropertyChanged(() => PictureUrl);
            }
        }

        public decimal Total { get { return Quantity * UnitPrice; } }

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
