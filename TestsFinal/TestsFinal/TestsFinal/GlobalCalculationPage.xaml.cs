using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.Model;
using Microsoft.Practices.ServiceLocation;
using ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestsFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GlobalCalculationPage : ContentPage
    {
        public GlobalCalculationPage()
        {
            InitializeComponent();
        }
    }
}