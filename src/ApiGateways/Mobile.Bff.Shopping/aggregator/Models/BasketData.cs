﻿using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models
{
	public class BasketData
	{
		public string BuyerId { get; set; }
		public IList<BasketDataItem> Items { get; set; }

		public BasketData(string buyerId)
		{
			BuyerId = buyerId;
			Items = new List<BasketDataItem>();
		}
	}

	public class BasketDataItem
	{
		public string Id { get; set; }
		public string ProductId { get; set; }
		public string ProductName { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal OldUnitPrice { get; set; }
		public int Quantity { get; set; }
		public string PictureUrl { get; set; }

	}
}
