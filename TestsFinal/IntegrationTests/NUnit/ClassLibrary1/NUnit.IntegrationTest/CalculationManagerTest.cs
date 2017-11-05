using System;
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
        private ICalculationManager _calculationManager;
        private ISqliteConnectionForTest _connectionService;

        [SetUp]
        public void Setup()
        {
            _connectionService = DependencyService.Get<ISqliteConnectionForTest>();
           // _connectionService.TeardownAndDelete();
            var dataAccess = new DataAccess(_connectionService);
            var restService = new RestService();
            _calculationManager = new CalculationManager(dataAccess, restService);
        }

        [TearDown]
        public void TearDown()
        {
            _connectionService.TeardownAndDelete();
        }

        [Test(Description = "d")]
        public void AddGlobalCalculationTest()
        {
            var globalCalculation = new GlobalCalculation();
            globalCalculation.Label = "global";
            _calculationManager.AddNewGlobalCalculation(globalCalculation,5);

            //Now we expect a global calculation and a local calculation
            //we ask the database directly   
            var connection = _connectionService.GetConnection();
            Assert.AreEqual(connection.Table<GlobalCalculation>().Count(),1);
            Assert.AreEqual(connection.Table<LocalCalculation>().Count(),1);
            Assert.AreEqual(connection.Table<GlobalCalculation>().First().Label,"global");
            Assert.AreEqual(connection.Table<LocalCalculation>().First().StartOperand,5);
        }

        [Test]
        public void AddLocalCalculationTest()
        {
            var globalCalculation = new GlobalCalculation();
            globalCalculation.Label = "global";
            _calculationManager.AddNewGlobalCalculation(globalCalculation, 5);

            var localCalculation = new LocalCalculation();
            _calculationManager.AddNewLocalCalculation(globalCalculation, localCalculation);

            var connection = _connectionService.GetConnection();
            Assert.AreEqual(connection.Table<LocalCalculation>().Count(), 2);
        }

        [Test]
        public void FetchDataFromServiceTest()
        {
            _calculationManager.FetchGlobalCalculationsFromServiceAsync().Wait();
            var connection = _connectionService.GetConnection();
            Assert.AreEqual(connection.Table<GlobalCalculation>().Count(), 1);
            Assert.AreEqual(connection.Table<LocalCalculation>().Count(), 2);
            Assert.AreEqual(connection.Table<GlobalCalculation>().First().Result,9);
        }
    }
}
