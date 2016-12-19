using System;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Controls
{
    public class BindablePicker : Picker
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource",
            typeof(IEnumerable), typeof(BindablePicker), null, propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem",
            typeof(object), typeof(BindablePicker), null, BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

        public static readonly BindableProperty ItemSelectedCommandProperty = BindableProperty.Create("ItemSelectedCommand",
          typeof(ICommand), typeof(BindablePicker), null);

        public BindablePicker()
        {
            SelectedIndexChanged += (o, e) =>
            {
                if (SelectedIndex < 0 || ItemsSource == null || !ItemsSource.GetEnumerator().MoveNext())
                {
                    SelectedItem = null;
                    return;
                }

                var index = 0;

                foreach (var item in ItemsSource)
                {
                    if (index == SelectedIndex)
                    {
                        SelectedItem = item;
                        break;
                    }

                    index++;
                }
            };
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public Object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set
            {
                if (SelectedItem != value)
                {
                    SetValue(SelectedItemProperty, value);
                    InternalUpdateSelectedIndex();
                }
            }
        }

        public ICommand ItemSelectedCommand
        {
            get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value); }
        }

        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

        private void InternalUpdateSelectedIndex()
        {
            var selectedIndex = -1;
            if (ItemsSource != null)
            {
                var index = 0;

                foreach (var item in ItemsSource)
                {
                    string strItem = item?.ToString();

                    if (item != null && SelectedItem != null
                        && !string.IsNullOrEmpty(strItem)
                        && item.ToString().Equals(SelectedItem.ToString()))
                    {
                        selectedIndex = index;
                        break;
                    }

                    index++;
                }
            }

            SelectedIndex = selectedIndex;
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var boundPicker = (BindablePicker)bindable;

            if (Equals(newValue, null) && !Equals(oldValue, null))
                return;

            boundPicker.Items.Clear();

            if (!Equals(newValue, null))
            {
                foreach (var item in (IEnumerable)newValue)
                    boundPicker.Items.Add(item.ToString());
            }

            boundPicker.InternalUpdateSelectedIndex();
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var boundPicker = (BindablePicker)bindable;

            boundPicker.ItemSelected?.Invoke(boundPicker, 
                new SelectedItemChangedEventArgs(newValue));

            if(boundPicker.ItemSelectedCommand != null)
            {
                boundPicker.ItemSelectedCommand.Execute(newValue);
            }

            boundPicker.InternalUpdateSelectedIndex();
        }
    }
}