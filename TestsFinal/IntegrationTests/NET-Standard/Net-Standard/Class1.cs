using System;
using Backend.Interfaces;
using Moq;
using Xunit;

namespace Net_Standard
{
    public class Class1
    {
        //private Mock<IPopUpService> _mockPopUp;

        //public Class1()
        //{
        //    _mockPopUp = new Mock<IPopUpService>();
        //}

        [Fact]
        public void Bam()
        {
            var mockPopUp = new Mock<IPopUpService>();
            Assert.True(true);
        }
    }
}
