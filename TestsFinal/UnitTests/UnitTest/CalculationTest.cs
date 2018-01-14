using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Business;
using Backend.Interfaces;
using Backend.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTest
{
    public class CalculationTest
    {
        private static CalculationManager _calculationManager;
        private static GlobalCalculation _globalCalculation;

        [ClassInitialize]
        public static void ClassInit()
        {
            var globalCalculations = new List<GlobalCalculation>();
            var mockDatabase = new Mock<IDataAccess>();
            mockDatabase.Setup(access => access.GetAllGlobalCalculations()).Returns(globalCalculations);
            _calculationManager = new CalculationManager(mockDatabase.Object, null);
            _globalCalculation = new GlobalCalculation();
        }


        /// <summary>
        /// GlobalCalculation with StartOperand 5
        /// </summary>
        [TestMethod]
        public void AddGlobalCalculationTest()
        {
            _globalCalculation = new GlobalCalculation();

            _calculationManager
                .AddNewGlobalCalculation(_globalCalculation, 5);

            Assert.IsTrue(_globalCalculation.LocalCalculations.Any());
        }

        /// <summary>
        /// 5 + 6
        /// </summary>
        [TestMethod]
        public void AddOperationToFirstLocalCalculationTest()
        {
            var firstLocalCalculation =
                _globalCalculation.LocalCalculations.First();
            var operation = new Operation {
                OperatorType = OperatorType.Addition,
                Operand = 6
            };

            _calculationManager
                .AddOperation(firstLocalCalculation, operation);
            _calculationManager
                .SetResult(firstLocalCalculation);
            _calculationManager
                .RefreshGlobalResult(_globalCalculation);

            Assert.AreEqual(firstLocalCalculation.Result, 11);
            Assert.AreEqual(_globalCalculation.Result, 11);
        }
    }
}
