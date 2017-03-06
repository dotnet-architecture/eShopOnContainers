using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace eShopOnContainers.Core.Extensions
{
    public static class ObservableExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {

            foreach (T item in source)
            {
                collection.Add(item);
            }

            return collection;

        }
    }
}