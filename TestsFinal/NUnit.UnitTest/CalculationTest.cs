using System.Collections.Generic;
using System.Linq;
using Backend.Business;
using Backend.Interfaces;
using Backend.Model;
using Moq;
using NUnit.Framework;

namespace NUnit.UnitTest
{
    [TestFixture]
    public class CalculationTest
    {
        private static CalculationManager _calculationManager;
        private static GlobalCalculation _globalCalculation;

        ///// <summary>
        ///// general setup - called once
        ///// </summary>
        [OneTimeSetUp]
        public static void ClassInit()
        {
            var globalCalculations = new List<GlobalCalculation>();
            var mockDatabase = new Mock<IDataAccess>();
            mockDatabase.Setup(access => access.GetAllGlobalCalculations()).Returns(globalCalculations);
            _calculationManager = new CalculationManager(mockDatabase.Object, null);
            _globalCalculation = new GlobalCalculation();
        }

        /// <summary>
        /// general cleanup - called once
        /// </summary>
        [OneTimeTearDown]
        public static void Cleanup()
        {

        }

        /// <summary>
        /// GlobalCalculation with StartOperand 5
        /// </summary>
        [Test, Order(1)]
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
        [Test, Order(2)]
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

        /// <summary>
        /// 5+6*(2-4) = -7
        /// </summary>
        [Test, Order(3)]
        public void AddOperationsWithBracketTest()
        {
            var firstLocalCalculation = _globalCalculation.LocalCalculations.First();
            var openOperation = new Operation {
                BracketType = BracketType.Open,
                Operand = 2,
                OperatorType = OperatorType.Multiplication
            };
            var closeOperation = new Operation {
                BracketType = BracketType.Close,
                Operand = 4,
                OperatorType = OperatorType.Subtraction
            };

            _calculationManager.AddOperation(firstLocalCalculation, openOperation);
            _calculationManager.AddOperation(firstLocalCalculation, closeOperation);
            _calculationManager.SetResult(firstLocalCalculation);

            Assert.AreEqual(firstLocalCalculation.Result, -7);
        }

        /// <summary>
        /// 5+6*(2-4) = -7
        /// -7
        /// </summary>
        [Test, Order(4)]
        public void AddSecondLocalOperationWithOperations()
        {
            var secondLocalCalculation = new LocalCalculation();

            _calculationManager.AddNewLocalCalculation(_globalCalculation, secondLocalCalculation);

            Assert.AreEqual(_globalCalculation.LocalCalculations.Count, 2);
            Assert.AreEqual(secondLocalCalculation.StartOperand, -7);
        }
    }
}
