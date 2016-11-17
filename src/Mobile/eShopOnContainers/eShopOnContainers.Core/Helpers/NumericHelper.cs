using System.Collections.ObjectModel;

namespace eShopOnContainers.Core.Helpers
{
    public class NumericHelper
    {
        public static ObservableCollection<int> GetNumericList(int count = 100)
        {
            var result = new ObservableCollection<int>();
            for (int i = 0; i < count; i++)
            {
                result.Add(i);
            }

            return result;
        }
    }
}