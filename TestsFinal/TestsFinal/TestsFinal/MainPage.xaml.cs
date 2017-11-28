using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DataAccess;
using Backend.Model;
using ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestsFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void MenuItem_OnClicked(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var viewCell = (ViewCell)menuItem.BindingContext;
            var item = (GlobalCalculation)viewCell.BindingContext;
            ((MainViewModel)BindingContext).GlobalCalculations.Remove(item);
            DataAccess.GetInstance().Remove(item);
        }
    }
}
