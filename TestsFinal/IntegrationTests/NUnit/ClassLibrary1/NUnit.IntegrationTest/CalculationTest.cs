using System.Collections.Generic;
using System.Linq;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.Model;
using Backend.RestService;
using NUnit.Framework;
using SQLite.Net;
using Xamarin.Forms;

namespace NUnit.IntegrationTest
{
    [TestFixture]
    public class CalculationTest
    {
        private SQLiteConnection _connection;
        private CalculationManager _calculationManager;
        private ISqliteConnectionService _sqliteConnection;


        [OneTimeSetUp]
        public void Setup()
        {
            _sqliteConnection = DependencyService.Get<ISqliteConnectionService>();
            DataAccess.Init(_sqliteConnection);
            _connection = _sqliteConnection.GetConnection();
            _calculationManager = new CalculationManager(DataAccess.GetInstance(), null);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            DatabaseHelper.CleanupDatabase(_connection);
        }

        [Test, Order(1)]
        public void Calculation_AddGlobalCalculationTest()
        {
            var globalCalculation = new GlobalCalculation();
            _calculationManager.AddNewGlobalCalculation(globalCalculation, 5);

            //Now we expect a global calculation and a local calculation
            //we ask the database directly   
            Assert.AreEqual(1, _connection.Table<GlobalCalculation>().Count());
            Assert.AreEqual(1, _connection.Table<LocalCalculation>().Count());
            Assert.AreEqual(5, _connection.Table<LocalCalculation>().First().StartOperand);
        }


        [Test, Order(2)]
        public void Calculation_AddOperationToFirstLocalCalculationTest()
        {
            var globalCalculation = _calculationManager.GetAllGlobalCalculations().First();
            _calculationManager.LoadGlobalCalculation(globalCalculation);
            var firstLocalCalculation = globalCalculation.LocalCalculations.First();
            var operation = new Operation {
                OperatorType = OperatorType.Addition,
                Operand = 6
            };

            _calculationManager.AddOperation(firstLocalCalculation, operation);
            _calculationManager.SetResult(firstLocalCalculation);
            _calculationManager.RefreshGlobalResult(globalCalculation);

            Assert.AreEqual(11, firstLocalCalculation.Result);
            Assert.AreEqual(11, globalCalculation.Result);
            Assert.IsNotEmpty(_connection.Table<Operation>());
        }

        //...Further Tests
    }
}