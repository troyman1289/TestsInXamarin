using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Model.Operator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class OperatorTest
    {
        [TestMethod]
        public void AdditionTest()
        {
            var result = Operators.Addition.Calculate(4, 5);
            Assert.AreEqual(result,9);
        }

        [TestMethod]
        public void SubstractionTest()
        {
            var result = Operators.Substraction.Calculate(6, 5);
            Assert.AreEqual(result, 1);

            result = Operators.Substraction.Calculate(5, 6);
            Assert.AreEqual(result, -1);
        }

        [TestMethod]
        public void MultiplicationTest()
        {
            var result = Operators.Multiplication.Calculate(4, 5);
            Assert.AreEqual(result, 20);
        }

        [TestMethod]
        public void DivisionTest()
        {
            var result = Operators.Division.Calculate(5, 10);
            Assert.AreEqual(result, (decimal)0.5);
            
            Assert.ThrowsException<DivideByZeroException>(() => Operators.Division.Calculate(5, 0));
        }
    }
}
