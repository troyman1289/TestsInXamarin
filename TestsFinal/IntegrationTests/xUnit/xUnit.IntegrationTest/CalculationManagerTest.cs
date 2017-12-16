using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.Model;
using Backend.RestService;
using Xamarin.Forms;
using Xunit;

namespace xUnit.IntegrationTest
{
    public class CalculationManagerTest : IDisposable
    {
        private ICalculationManager _calculationManager;
        private ISqliteConnectionForTest _connectionService;

        public CalculationManagerTest()
        {
            _connectionService = DependencyService.Get<ISqliteConnectionForTest>();
           // _connectionService.TeardownAndDelete();
            var dataAccess = new DataAccess(_connectionService);
            var restService = new RestService();
            _calculationManager = new CalculationManager(dataAccess, restService);
        }

        public void Dispose()
        {
            _connectionService.TeardownAndDelete();
        }

        [Fact(DisplayName = "AddGlobalCalculationTest")]
        public void AddGlobalCalculationTest()
        {
            var globalCalculation = new GlobalCalculation();
            globalCalculation.Label = "global";

            _calculationManager.AddNewGlobalCalculation(globalCalculation,5);

            //Now we expect a global calculation and a local calculation
            //we ask the database directly   
            var connection = _connectionService.GetConnection();
            Assert.Equal(1, connection.Table<GlobalCalculation>().Count());
            Assert.Equal(1, connection.Table<LocalCalculation>().Count());
            Assert.Equal("global", connection.Table<GlobalCalculation>().First().Label);
            Assert.Equal(5, connection.Table<LocalCalculation>().First().StartOperand);
        }

        [Fact(DisplayName = "AddLocalCalculationTest")]
        public void AddLocalCalculationTest()
        {
            var globalCalculation = new GlobalCalculation();
            globalCalculation.Label = "global";
            _calculationManager.AddNewGlobalCalculation(globalCalculation, 5);

            var localCalculation = new LocalCalculation();
            _calculationManager.AddNewLocalCalculation(globalCalculation, localCalculation);

            var connection = _connectionService.GetConnection();
            Assert.Equal(2, connection.Table<LocalCalculation>().Count());
        }

        [Fact(DisplayName = "FetchDataFromServiceTest")]
        public void FetchDataFromServiceTest()
        {
            _calculationManager.FetchGlobalCalculationsFromServiceAsync().Wait();
            var connection = _connectionService.GetConnection();
            Assert.Equal(1, connection.Table<GlobalCalculation>().Count());
            Assert.Equal(2, connection.Table<LocalCalculation>().Count());
            Assert.Equal(9, connection.Table<GlobalCalculation>().First().Result);
        }
    }
}
