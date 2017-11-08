using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.RestService;
using Backend.Utils;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using TestsFinal;
using ViewModels;
using Xamarin.Forms;

namespace TestsStart
{
    public class Locator
    {
        static Locator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var connectionService = DependencyService.Get<ISqliteConnectionService>();
            var navigationService = new NavigationService();

            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
            SimpleIoc.Default.Register<ISqliteConnectionService>(() => connectionService);
            SimpleIoc.Default.Register<IPopUpService, PopUpService>();

            SimpleIoc.Default.Register<IRestService, RestService>();
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

        public GlobalCalculationViewModel GlobalCalculationViewModel
        {
            get { return ServiceLocator.Current.GetInstance<GlobalCalculationViewModel>(); }
        }

        public LocalCalculationViewModel LocalCalculationViewModel
        {
            get { return ServiceLocator.Current.GetInstance<LocalCalculationViewModel>(); }
        }
        public MainViewModel MainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }
    }
}
