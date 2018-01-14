using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace xUnit.IntegrationTest
{
    public class TestOrderer : ITestCaseOrderer
    {
        public const string TypeName = "xUnit.IntegrationTest.TestOrderer";

        public const string AssembyName = "xUnit.IntegrationTest";

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(
            IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            return testCases.OrderBy(GetOrderOfTest);
        }

        private static int GetOrderOfTest<TTestCase>(
            TTestCase testCase)
            where TTestCase : ITestCase
        {
            var attribute = testCase.TestMethod.Method
                .ToRuntimeMethod()
                .GetCustomAttribute<OrderAttribute>();
            return attribute?.Order ?? 0;
        }
    }
}   