using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.Utils;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using ViewModels;
using Xamarin.Forms;

namespace TestsFinal
{
    public class Locator
    {
        static Locator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var connectionService = DependencyService.Get<ISqliteConnectionService>();
            SimpleIoc.Default.Register<ISqliteConnectionService>(() => connectionService);

            var navigationService = new NavigationService();
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);

            SimpleIoc.Default.Register<IDataAccess,DataAccess>();
            SimpleIoc.Default.Register<ICalculationManager,CalculationManager>();
            SimpleIoc.Default.Register<GlobalCalculationViewModel, GlobalCalculationViewModel>();
            SimpleIoc.Default.Register<LocalCalculationViewModel, LocalCalculationViewModel>();

            RegisterViewModelWithView<MainViewModel, MainPage>(navigationService);
            RegisterViewModelWithView<GlobalCalculationViewModel, GlobalCalculationPage>(navigationService);
            RegisterViewModelWithView<LocalCalculationViewModel, LocalCalculationPage>(navigationService);

        }

        private static void RegisterViewModelWithView<T, TK>(NavigationService navigationService) where T : NotifyingObject where TK : Page
        {
            navigationService.Configure(typeof(T).ToString(), typeof(TK));
            SimpleIoc.Default.Register<T>();
        }

        public static GlobalCalculationViewModel GlobalCalculationViewModel
        {
            get { return ServiceLocator.Current.GetInstance<GlobalCalculationViewModel>(); }
        }

        public static LocalCalculationViewModel LocalCalculationViewModel
        {
            get { return ServiceLocator.Current.GetInstance<LocalCalculationViewModel>(); }
        }
        public static MainViewModel MainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }
    }
}
