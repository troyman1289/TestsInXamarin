﻿using System;
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
            var mockDatabase = new Mock<IDataAccess>();
            _manager = new CalculationManager(mockDatabase.Object,null);
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

        /// <summary>
        /// We expect that the last operation will be removed if we have divide by zero
        /// </summary>
        [TestMethod]
        public void DivisionByZeroTest()
        {
            //Arrange
            var globalCalculation = new GlobalCalculation();
            _manager.AddNewGlobalCalculation(globalCalculation, 2);
            var localCalculation = globalCalculation.LocalCalculations.First();
            var operation = new Operation { OperatorType = OperatorType.Addition, Operand = 2 };
            _manager.AddOperation(localCalculation, operation);
            operation = new Operation { OperatorType = OperatorType.Division, Operand = 0 };
            _manager.AddOperation(localCalculation, operation);

            //Act
            _manager.SetResult(localCalculation);

            //Assert
            Assert.AreEqual(globalCalculation.LocalCalculations.First().Result, 4);
            Assert.AreEqual(globalCalculation.LocalCalculations.First().Operations.Count, 1);
        }
    }
}
