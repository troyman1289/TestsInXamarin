using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Backend.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;

namespace TestsFinal
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
            var navigationService = (NavigationService)ServiceLocator.Current.GetInstance<INavigationService>();
            navigationService.Navigation = MainPage.Navigation;
        }

        protected override void OnStart()
        {
            // Handle when your app starts

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
