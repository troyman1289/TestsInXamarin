using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.Model;
using SQLite.Net;
using Xamarin.Forms;

namespace xUnit.IntegrationTest.CalculationTest
{
    public class TestClassFixture : IDisposable
    {
        public SQLiteConnection Connection { get; private set; }
        public CalculationManager CalculationManager { get; private set; }

        public GlobalCalculation GlobalCalculation { get; }

        private List<string> _executedMethods = new List<string>();

        public TestClassFixture()
        {
            var connectionService = DependencyService.Get<ISqliteConnectionService>();
            Connection = connectionService.GetConnection();
            DataAccess.Init(connectionService);
            var dataAccess = DataAccess.GetInstance();
            CalculationManager = new CalculationManager(dataAccess, null);
            GlobalCalculation = new GlobalCalculation();
        }


        public void Dispose()
        {
            DatabaseHelper.CleanupDatabase(Connection);
        }

        public void CheckOrder([CallerMemberName] string methodName = null)
        {
            if (string.IsNullOrEmpty(methodName))
                return;

            var method = typeof(CalculationTest).GetRuntimeMethods()
                .FirstOrDefault(m => string.Equals(m.Name, methodName));
            if (method == null)
                return;

            var orderAttributeData = method.CustomAttributes
                .FirstOrDefault(a => a.AttributeType == typeof(OrderAttribute));
            if (orderAttributeData == null)
                return;

            var order = (int)orderAttributeData.ConstructorArguments.First().Value;

            if ((_executedMethods.Count + 1) != order)
                throw new Exception("Wrong Order of Tests");

            _executedMethods.Add(methodName);
        }
    }
}