using System;
using System.Collections.Generic;
using System.Linq;
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
        private CalculationManager _calculationManager;

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
        [ClassCleanup]
        public static void Cleanup()
        {

        }

        /// <summary>
        /// Setup before each test
        /// </summary>
        [TestInitialize]
        public void TestSetup()
        {
            var globalcalculations = new List<GlobalCalculation>();
            var mockDatabase = new Mock<IDataAccess>();
            mockDatabase
                .Setup(da => da.GetAllGlobalCalculations())
                .Returns(globalcalculations);

            //mockDatabase
            //    .Setup(da => da.Insert(It.IsAny<object>()))
            //    .Callback<object>((o) => {
            //        globalcalculations.Add((GlobalCalculation)o);
            //    });
            //mockDatabase.Object
            //    .Insert(new GlobalCalculation() { Label = "X" });

            _calculationManager = new CalculationManager(mockDatabase.Object, null);
        }

        /// <summary>
        /// Cleanup after each test
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            
        }

        /// <summary>
        /// We expect that the last operation will be removed if we have divide by zero
        /// </summary>
        [TestMethod]
        public void DivisionByZeroTest()
        {
            //Arrange
            var globalCalculation = new GlobalCalculation();
            _calculationManager.AddNewGlobalCalculation(globalCalculation, 2);
            var localCalculation = globalCalculation.LocalCalculations.First();
            var operation = new Operation { OperatorType = OperatorType.Addition, Operand = 2 };
            _calculationManager.AddOperation(localCalculation, operation);
            operation = new Operation { OperatorType = OperatorType.Division, Operand = 0 };
            _calculationManager.AddOperation(localCalculation, operation);

            //Act
            _calculationManager.SetResult(localCalculation);

            //Assert
            Assert.AreEqual(globalCalculation.LocalCalculations.First().Result, 4);
            Assert.AreEqual(globalCalculation.LocalCalculations.First().Operations.Count, 1);
        }
    }
}
