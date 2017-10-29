using System;
using Backend.Business;
using Backend.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTest
{
    [TestClass]
    public class CalculationManagerTest
    {

        //SETUP???


        [TestMethod]
        public void TestMethod1()
        {
            var mockDatabase = new Mock<IDataAccess>();
            var manager = new CalculationManager(mockDatabase.Object);


        }
    }
}
