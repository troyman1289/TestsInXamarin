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
        private static CalculationManager _manager;
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
            _manager = new CalculationManager(mockDatabase.Object,null);
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

            _manager.AddNewGlobalCalculation(_globalCalculation, 5);

            Assert.IsTrue(_globalCalculation.LocalCalculations.Any());
        }

        /// <summary>
        /// 5 + 6
        /// </summary>
        [Test, Order(2)]
        public void AddOperationToFirstLocalCalculationTest()
        {
            //C1_AddGlobalCalculationTest();
            var firstLocalCalculation = _globalCalculation.LocalCalculations.First();
            var operation = new Operation
            {
                OperatorType = OperatorType.Addition,
                Operand = 6
            };

            _manager.AddOperation(firstLocalCalculation,operation);
            _manager.SetResult(firstLocalCalculation);
            _manager.RefreshGlobalResult(_globalCalculation);

            Assert.AreEqual(firstLocalCalculation.Result, 11);
            Assert.AreEqual(_globalCalculation.Result, 11);
        }

        /// <summary>
        /// 5+6*(2-4) = -7
        /// </summary>
        [Test, Order(3)]
        public void C3_AddOperationsWithBracketToLocalCalculationTest()
        {
            var firstLocalCalculation = _globalCalculation.LocalCalculations.First();
            var operation = new Operation
            {
                BracketType = BracketType.Open,
                Operand = 2,
                OperatorType = OperatorType.Multiplication
            };
            _manager.AddOperation(firstLocalCalculation, operation);

            operation = new Operation {
                BracketType = BracketType.Close,
                Operand = 4,
                OperatorType = OperatorType.Subtraction
            };
            _manager.AddOperation(firstLocalCalculation,operation);

            _manager.SetResult(firstLocalCalculation);

            Assert.AreEqual(firstLocalCalculation.Result, -7);
        }

        /// <summary>
        /// 5+6*(2-4)+2*11 = 15
        /// </summary>
        [Test, Order(4)]
        public void AddEndOperationsToLocalCalculationTest()
        {
            var firstLocalCalculation = _globalCalculation.LocalCalculations.First();
            var operation = new Operation {
                Operand = 2,
                OperatorType = OperatorType.Addition
            };
            _manager.AddOperation(firstLocalCalculation, operation);
            operation = new Operation {
                Operand = 11,
                OperatorType = OperatorType.Multiplication
            };
            _manager.AddOperation(firstLocalCalculation, operation);
            _manager.SetResult(firstLocalCalculation);

            Assert.AreEqual(firstLocalCalculation.Result, 15);
        }

        /// <summary>
        /// 5+6*(2-4)+2*11 = 15
        /// 15/2*4 = 30
        /// </summary>
        [Test, Order(5)]
        public void C4_AddSecondLocalOperationWithOperations()
        {
            var secondLocalCalculation = new LocalCalculation();
            _manager.AddNewLocalCalculation(_globalCalculation, secondLocalCalculation);
            var operation = new Operation {
                Operand = 2,
                OperatorType = OperatorType.Division
            };
            _manager.AddOperation(secondLocalCalculation, operation);
            operation = new Operation {
                Operand = 4,
                OperatorType = OperatorType.Multiplication
            };
            _manager.AddOperation(secondLocalCalculation, operation);
            _manager.SetResult(secondLocalCalculation);
            _manager.RefreshGlobalResult(_globalCalculation);

            Assert.AreEqual(secondLocalCalculation.Order,2);
            Assert.AreEqual(secondLocalCalculation.Result,30);
            Assert.AreEqual(_globalCalculation.Result,30);
        }
    }
}
