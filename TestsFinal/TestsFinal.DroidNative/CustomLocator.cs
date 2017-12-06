using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.RestService;

namespace TestsFinal.DroidNative
{
    public class CustomLocator
    {
        private static IDataAccess _dataAccess;
        private static ICalculationManager _calculationManager;

        static CustomLocator()
        {
            Backend.DataAccess.DataAccess.Init(new Sqlite());
            _dataAccess = Backend.DataAccess.DataAccess.GetInstance();
            var restService = new RestService();
            _calculationManager = new CalculationManager();
        }

        public static IDataAccess DataAccess
        {
            get { return _dataAccess; }
        }

        public static ICalculationManager CalculationManager
        {
            get { return _calculationManager; }
        }

    }
}