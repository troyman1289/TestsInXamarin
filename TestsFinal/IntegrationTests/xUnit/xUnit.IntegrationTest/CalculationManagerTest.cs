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
        private ISqliteConnectionService _connectionService;

        public CalculationManagerTest()
        {
            _connectionService = DependencyService.Get<ISqliteConnectionService>();
            var dataAccess = new DataAccess(_connectionService);
            var restService = new RestService();
            _calculationManager = new CalculationManager(dataAccess, restService);
        }

        public void Dispose()
        {

        }

        [Fact]
        public void AddGlobalCalculationTest()
        {
            var globalCalculation = new GlobalCalculation();
            globalCalculation.Label = "global";
            _calculationManager.AddNewGlobalCalculation(globalCalculation,5);

            //Now we expect a global calculation and a local calculation
            //we ask the database directly   
            var connection = _connectionService.GetConnection();
            Assert.Equal(connection.Table<GlobalCalculation>().Count(),1);
            Assert.Equal(connection.Table<LocalCalculation>().Count(),1);
            Assert.Equal(connection.Table<GlobalCalculation>().First().Label,"global");
            Assert.Equal(connection.Table<LocalCalculation>().First().StartOperand,5);
        }

        [Fact]
        public void AddLocalCalculationTest()
        {
            var globalCalculation = new GlobalCalculation();
            globalCalculation.Label = "global";
            _calculationManager.AddNewGlobalCalculation(globalCalculation, 5);

            var localCalculation = new LocalCalculation();
            _calculationManager.AddNewLocalCalculation(globalCalculation, localCalculation);

            var connection = _connectionService.GetConnection();
            Assert.Equal(connection.Table<LocalCalculation>().Count(), 2);
        }

        [Fact]
        public void FetchDataFromServiceTest()
        {
            _calculationManager.FetchGlobalCalculationsFromServiceAsync().Wait();
            var connection = _connectionService.GetConnection();
            Assert.Equal(connection.Table<GlobalCalculation>().Count(), 1);
            Assert.Equal(connection.Table<LocalCalculation>().Count(), 1);
            Assert.Equal(connection.Table<GlobalCalculation>().First().Result,11);
        }
    }
}
