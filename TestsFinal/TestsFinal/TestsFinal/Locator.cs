using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.RestService;
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
            var navigationService = new NavigationService();

            DataAccess.Init(connectionService);
            SimpleIoc.Default.Register<IDataAccess>(() => DataAccess.GetInstance());

            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
            SimpleIoc.Default.Register<ISqliteConnectionService>(() => connectionService);
            SimpleIoc.Default.Register<IPopUpService, PopUpService>();

            SimpleIoc.Default.Register<IRestService, RestService>();
            SimpleIoc.Default.Register<ICalculationManager,CalculationManager>();

            RegisterViewModelWithView<MainViewModel, MainPage>(navigationService);
            RegisterViewModelWithView<CreateGlobalCalculationViewModel, CreateGlobalCalculationPage>(navigationService);
            RegisterViewModelWithView<GlobalCalculationViewModel, GlobalCalculationPage>(navigationService);

        }

        private static void RegisterViewModelWithView<T, TK>(NavigationService navigationService) where T : NotifyingObject where TK : Page
        {
            navigationService.Configure(typeof(T).ToString(), typeof(TK));
            SimpleIoc.Default.Register<T>();
        }

        public CreateGlobalCalculationViewModel CreateGlobalCalculationViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CreateGlobalCalculationViewModel>(); }
        }

        public GlobalCalculationViewModel GlobalCalculationViewModel
        {
            get { return ServiceLocator.Current.GetInstance<GlobalCalculationViewModel>(); }
        }
        public MainViewModel MainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }
    }
}
