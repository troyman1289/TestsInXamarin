using System;
using System.Linq;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.Model;
using Backend.RestService;
using NUnit.Framework;
using Xamarin.Forms;

namespace NUnit.IntegrationTest
{
    [TestFixture]
    public class CalculationManagerTest
    {
        private CalculationManager _calculationManager;
        private ISqliteConnectionService _connectionService;

        [SetUp]
        public void Setup()
        {
            _connectionService = DependencyService.Get<ISqliteConnectionService>();
            DataAccess.Init(_connectionService);
            var dataAccess = DataAccess.GetInstance();
            var restService = new RestService();
            _calculationManager = new CalculationManager(dataAccess, restService);
        }

        [TearDown]
        public void TearDown()
        {
            var connection = _connectionService.GetConnection();
            DatabaseHelper.CleanupDatabase(connection);
        }

        [Test(Description = "AddNewGlobalCalculationTest")]
        public void AddGlobalCalculationTest()
        {
            var globalCalculation = new GlobalCalculation();
            globalCalculation.Label = "global";

            _calculationManager.AddNewGlobalCalculation(globalCalculation, 5);

            var connection = _connectionService.GetConnection();
            Assert.AreEqual(1, connection.Table<GlobalCalculation>().Count());
            Assert.AreEqual(1, connection.Table<LocalCalculation>().Count());
            Assert.AreEqual("global", connection.Table<GlobalCalculation>().First().Label);
            Assert.AreEqual(5, connection.Table<LocalCalculation>().First().StartOperand);
        }

        [Test]
        public void AddLocalCalculationTest()
        {
            var globalCalculation = new GlobalCalculation();
            var localCalculation = new LocalCalculation();

            _calculationManager.AddNewGlobalCalculation(globalCalculation, 5);
            _calculationManager.AddNewLocalCalculation(globalCalculation, localCalculation);

            var connection = _connectionService.GetConnection();
            Assert.AreEqual(connection.Table<LocalCalculation>().Count(), 2);
        }

        [Test]
        public void FetchDataFromServiceTest()
        {
            _calculationManager.FetchGlobalCalculationsFromServiceAsync().Wait();
            var connection = _connectionService.GetConnection();

            Assert.True(connection.Table<GlobalCalculation>().Any());
            Assert.True(connection.Table<LocalCalculation>().Count() > 1 
                && connection.Table<LocalCalculation>().Count() < 4);
            Assert.True(connection.Table<GlobalCalculation>().First().Result > 9);
        }
    }
}
