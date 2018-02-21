﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Common.API
{
    public class PaginatedItemsViewModel<TEntity> where TEntity : class
	{
		public int PageIndex { get; private set; }

		public int PageSize { get; private set; }

		public long Count { get; private set; }

		public IEnumerable<TEntity> Data { get; private set; }

		public PaginatedItemsViewModel(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
		{
			this.PageIndex = pageIndex;
			this.PageSize = pageSize;
			this.Count = count;
			this.Data = data;
		}
	}
}
