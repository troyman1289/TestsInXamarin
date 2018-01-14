using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Model;
using xUnit.IntegrationTest;
using xUnit.IntegrationTest.CalculationTest;
using Xunit;

namespace xUnit.IntegrationTest.CalculationTest
{
    [TestCaseOrderer(TestOrderer.TypeName, TestOrderer.AssembyName)]
    public class CalculationTest : IClassFixture<TestClassFixture>
    {
        private readonly TestClassFixture _fixture;

        public CalculationTest(TestClassFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Calculation_AddGlobalCalculationTest"), Order(1)]
        public void AddGlobalCalculationTest()
        {
            _fixture.CheckOrder();
            _fixture.CalculationManager.AddNewGlobalCalculation(_fixture.GlobalCalculation, 5);

            //Now we expect a global calculation and a local calculation
            //we ask the database directly   
            Assert.Equal(1, _fixture.Connection.Table<GlobalCalculation>().Count());
            Assert.Equal(1, _fixture.Connection.Table<LocalCalculation>().Count());
            Assert.Equal(5, _fixture.Connection.Table<LocalCalculation>().First().StartOperand);
        }


        [Fact(DisplayName = "CalculationTest_AddOperationToFirstLocalCalculationTest"), Order(2)]
        public void AddOperationToFirstLocalCalculationTest()
        {
            _fixture.CheckOrder();
            var firstLocalCalculation = _fixture.GlobalCalculation.LocalCalculations.First();
            var operation = new Operation {
                OperatorType = OperatorType.Addition,
                Operand = 6
            };

            _fixture.CalculationManager.AddOperation(firstLocalCalculation, operation);
            _fixture.CalculationManager.SetResult(firstLocalCalculation);
            _fixture.CalculationManager.RefreshGlobalResult(_fixture.GlobalCalculation);

            Assert.Equal(11, firstLocalCalculation.Result);
            Assert.Equal(11, _fixture.GlobalCalculation.Result);
            Assert.NotEmpty(_fixture.Connection.Table<Operation>());
        }
    }
}