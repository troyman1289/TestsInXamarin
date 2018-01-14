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
            var result2 = Operators.Addition.Calculate(5, (decimal)1.5);

            Assert.AreEqual(result, 9);
            Assert.AreEqual(result2, (decimal)6.5);
        }

        [TestMethod]
        public void MultiplicationTest()
        {
            var result = Operators.Multiplication.Calculate(4, 5);
            var result2 = Operators.Multiplication.Calculate(5, (decimal)1.5);

            Assert.AreEqual(result, 20);
            Assert.AreEqual(result2, (decimal)7.5);
        }

        [TestMethod]
        public void SubtractionTest()
        {
            var result = Operators.Subtraction.Calculate(5, 4);

            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void DivisionTest()
        {
            var result = Operators.Division.Calculate(3, 2);

            Assert.IsTrue(result == (decimal)1.5);
            Assert.ThrowsException<DivideByZeroException>(
                () => Operators.Division.Calculate(5, 0));
        }
    }
}
