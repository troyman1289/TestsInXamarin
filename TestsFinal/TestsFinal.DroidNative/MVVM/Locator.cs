using Android.App;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.RestService;
using Backend.Utils;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using ViewModels;
using INavigationService = Backend.Interfaces.INavigationService;

namespace TestsFinal.DroidNative.MVVM
{
    public class Locator
    {
        public static void Init()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var connectionService = new Sqlite();
            var navigationService = new NavigationService();

            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
            SimpleIoc.Default.Register<ISqliteConnectionService>(() => connectionService);
            SimpleIoc.Default.Register<IPopUpService, PopUpService>();

            SimpleIoc.Default.Register<IRestService, RestService>();
            SimpleIoc.Default.Register<ICalculationManager,CalculationManager>();

            RegisterViewModelWithView<MainViewModel, MainActivityWithMVVM>(navigationService);
            RegisterViewModelWithView<CreateGlobalCalculationViewModel, CreateGlobalCalculationActivityMVVM>(navigationService);
            RegisterViewModelWithView<GlobalCalculationViewModel, GlobalCalculationActivityMVVM>(navigationService);

        }

        private static void RegisterViewModelWithView<T, TK>(NavigationService navigationService) where T : NotifyingObject where TK : Activity
        {
            navigationService.Configure(typeof(T).ToString(), typeof(TK));
            SimpleIoc.Default.Register<T>();
        }
    }
}
