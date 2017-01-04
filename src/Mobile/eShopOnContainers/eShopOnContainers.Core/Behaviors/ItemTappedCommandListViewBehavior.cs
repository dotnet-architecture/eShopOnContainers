using System.Windows.Input;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Behaviors
{
    public sealed class ItemTappedCommandListViewBehavior
    {
        public static readonly BindableProperty ItemTappedCommandProperty =
            BindableProperty.CreateAttached(
                "ItemTappedCommand",
                typeof(ICommand),
                typeof(ItemTappedCommandListViewBehavior),
                default(ICommand),
                BindingMode.OneWay,
                null,
                PropertyChanged);

        private static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var listView = bindable as ListView;
            if (listView != null)
            {
                listView.ItemTapped -= ListViewOnItemTapped;
                listView.ItemTapped += ListViewOnItemTapped;
            }
        }

        private static void ListViewOnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var list = sender as ListView;
            if (list != null && list.IsEnabled && !list.IsRefreshing)
            {
                list.SelectedItem = null;
                var command = GetItemTappedCommand(list);
                if (command != null && command.CanExecute(e.Item))
                {
                    command.Execute(e.Item);
                }
            }
        }

        public static ICommand GetItemTappedCommand(BindableObject bindableObject)
        {
            return (ICommand)bindableObject.GetValue(ItemTappedCommandProperty);
        }

        public static void SetItemTappedCommand(BindableObject bindableObject, object value)
        {
            bindableObject.SetValue(ItemTappedCommandProperty, value);
        }
    }
}