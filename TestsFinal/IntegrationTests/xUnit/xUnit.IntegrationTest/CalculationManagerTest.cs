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
        private CalculationManager _calculationManager;
        private ISqliteConnectionService _connectionService;

        public CalculationManagerTest()
        {
            _connectionService = DependencyService.Get<ISqliteConnectionService>();
            DataAccess.Init(_connectionService);
            var dataAccess = DataAccess.GetInstance();
            var restService = new RestService();
            _calculationManager = new CalculationManager(dataAccess, restService);
        }

        public void Dispose()
        {
            var connection = _connectionService.GetConnection();
            DatabaseHelper.CleanupDatabase(connection);
        }

        [Fact(DisplayName = "AddNewGlobalCalculationTest")]
        public void AddNewGlobalCalculationTest()
        {
            var globalCalculation = new GlobalCalculation();
            globalCalculation.Label = "global";

            _calculationManager.AddNewGlobalCalculation(globalCalculation, 5);

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
            var localCalculation = new LocalCalculation();

            _calculationManager.AddNewGlobalCalculation(globalCalculation, 5);
            _calculationManager.AddNewLocalCalculation(globalCalculation, localCalculation);

            var connection = _connectionService.GetConnection();
            Assert.Equal(2, connection.Table<LocalCalculation>().Count());
        }

        [Fact(DisplayName = "FetchDataFromServiceTest", Skip = "no reason")]
        public void FetchDataFromServiceTest()
        {
            _calculationManager.FetchGlobalCalculationsFromServiceAsync().Wait();
            var connection = _connectionService.GetConnection();
            //Assert.ThrowsAsync(
            //    typeof(HttpRequestException),
            //    _calculationManager.FetchGlobalCalculationsFromServiceAsync).Wait();
            Assert.True(connection.Table<GlobalCalculation>().Any());
            Assert.InRange(connection.Table<LocalCalculation>().Count(), 1, 4);
            Assert.True(connection.Table<GlobalCalculation>().First().Result > 9);
        }
    }
}
