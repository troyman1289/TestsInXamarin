using System;

namespace xUnit.IntegrationTest.CalculationTest
{
    public class OrderAttribute : Attribute
    {
        public int Order { get; }

        public OrderAttribute(int order)
        {
            Order = order;
        }
    }
}