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
    public class OperationsTest
    {
        [TestMethod]
        public void AdditionTest()
        {
            var result = Operators.Addition.Calculate(4, 5);
            Assert.AreEqual(result,9);
        }
    }
}
