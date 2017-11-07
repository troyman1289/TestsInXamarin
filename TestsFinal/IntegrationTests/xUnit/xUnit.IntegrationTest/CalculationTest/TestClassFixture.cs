using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Backend.Business;
using Backend.DataAccess;
using SQLite.Net;
using Xamarin.Forms;

namespace xUnit.IntegrationTest
{
    public class TestClassFixture : IDisposable
    {
        public SQLiteConnection Connection { get; private set; }
        public CalculationManager CalculationManager { get; private set; }
        private readonly ISqliteConnectionForTest _sqliteConnection;
        private List<string> _executedMethods;

        public TestClassFixture()
        {
            _sqliteConnection = DependencyService.Get<ISqliteConnectionForTest>();
            Connection = _sqliteConnection.GetConnection();
            _executedMethods = new List<string>();
            CalculationManager = new CalculationManager(new DataAccess(_sqliteConnection),null);
        }

        public void Dispose()
        {
            _sqliteConnection.TeardownAndDelete();
        }

        public void CheckOrder([CallerMemberName] string methodName = null)
        {
            if(string.IsNullOrEmpty(methodName)) return;

            var method = typeof(CalculationTest).GetRuntimeMethods().FirstOrDefault(m => string.Equals(m.Name, methodName));
            if(method == null) return;

            var orderAttributeData = method.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(OrderAttribute));
            if (orderAttributeData == null) return;

            var order = (int)orderAttributeData.ConstructorArguments.First().Value;

            if((_executedMethods.Count + 1) != order) throw new Exception("Wrong Order of Tests");

            _executedMethods.Add(methodName);
        }
    }
}