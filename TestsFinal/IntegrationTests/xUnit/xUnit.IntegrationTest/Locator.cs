using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.RestService;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;

namespace xUnit.IntegrationTest
{
    class Locator
    {
        //private static bool _isReady = false;
        //public static void Init()
        //{
        //    if (_isReady) return;

        //    _isReady = true;
        //    ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
        //    var connectionService = DependencyService.Get<ISqliteConnectionService>();
        //    SimpleIoc.Default.Register<ISqliteConnectionService>(() => connectionService);
        //    SimpleIoc.Default.Register<IRestService, RestService>();
        //    SimpleIoc.Default.Register<IDataAccess, DataAccess>();
        //    SimpleIoc.Default.Register<ICalculationManager, CalculationManager>();
        //}
    }
}
