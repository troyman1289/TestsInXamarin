using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using ViewModels;
using Xamarin.Forms;

namespace TestsFinal
{
    public static class Locator
    {
        static Locator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IDataAccess,DataAccess>();
            SimpleIoc.Default.Register<ICalculationManager,CalculationManager>();
            SimpleIoc.Default.Register<GlobalCalculationViewModel, GlobalCalculationViewModel>();
            SimpleIoc.Default.Register<LocalCalculationViewModel, LocalCalculationViewModel>();
            SimpleIoc.Default.Register<MainViewModel, MainViewModel>();
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
