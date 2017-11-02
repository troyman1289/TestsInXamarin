using System;
using Backend.Business;
using Backend.Interfaces;
using Backend.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTest
{
    [TestClass]
    public class CalculationManagerTest
    {
        private CalculationManager _manager;

        ///// <summary>
        ///// general setup - called once
        ///// </summary>
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {

        }

        ///// <summary>
        ///// general cleanup - called once
        ///// </summary>
        //[ClassCleanup]
        //public static void Cleanup()
        //{

        //}

        /// <summary>
        /// Setup before each test
        /// </summary>
        [TestInitialize]
        public void TestSetup()
        {
            var mockDatabase = new Mock<IDataAccess>();
            _manager = new CalculationManager(mockDatabase.Object);
        }

        /// <summary>
        /// Cleanup after each test
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            
        }

        [TestMethod]
        public void AddOperationTest()
        {
            var localCalculation = new LocalCalculation();
            localCalculation.Operations.Add(new Operation{Order = 1});
            localCalculation.Operations.Add(new Operation{Order = 2});
            var operation = new Operation();
            _manager.AddOperation(localCalculation, operation);

            Assert.AreEqual(localCalculation.Operations.Count,3);
            Assert.AreEqual(operation.Order,3);
        }

        [TestMethod]
        public void CalculationTest()
        {
            var localCalculation = new LocalCalculation();
            localCalculation.Operations.Add(new Operation { Order = 1 });
            localCalculation.Operations.Add(new Operation { Order = 2 });
            var operation = new Operation();
            _manager.AddOperation(localCalculation, operation);

            Assert.AreEqual(localCalculation.Operations.Count, 3);
            Assert.AreEqual(operation.Order, 3);
        }
    }
}
