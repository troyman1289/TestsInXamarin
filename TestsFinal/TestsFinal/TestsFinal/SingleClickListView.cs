using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace TestsFinal
{
    public class SingleClickListView : ListView
    {
        public static BindableProperty ItemClickCommandProperty = BindableProperty.Create(
            "ItemClickCommand",
            typeof(ICommand),
            typeof(SingleClickListView));

        public ICommand ItemClickCommand
        {
            get { return (ICommand) GetValue(ItemClickCommandProperty); }
            set { SetValue(ItemClickCommandProperty, value); }
        }

        public SingleClickListView()
        {
            ItemTapped += OnItemTapped;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null && ItemClickCommand != null && ItemClickCommand.CanExecute(e.Item))
            {
                ItemClickCommand.Execute(e.Item);
                SelectedItem = null;
            }
        }
    }
}
