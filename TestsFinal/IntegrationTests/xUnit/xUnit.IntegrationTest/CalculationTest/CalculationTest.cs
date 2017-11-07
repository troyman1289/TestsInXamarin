using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Model;
using xUnit.IntegrationTest;
using Xunit;

namespace xUnit.IntegrationTest
{
    [TestCaseOrderer(TestOrder.TypeName, TestOrder.AssembyName)]
    public class CalculationTest : IClassFixture<TestClassFixture>
    {
        private readonly TestClassFixture _fixture;

        public CalculationTest(TestClassFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Calculation_AddGlobalCalculationTest"), Order(1)]
        public void Calculation_AddGlobalCalculationTest()
        {
            _fixture.CheckOrder();
            var globalCalculation = new GlobalCalculation();
            _fixture.CalculationManager.AddNewGlobalCalculation(globalCalculation, 5);

            //Now we expect a global calculation and a local calculation
            //we ask the database directly   
            var connection = _fixture.Connection;
            Assert.Equal(1, connection.Table<GlobalCalculation>().Count());
            Assert.Equal(1, connection.Table<LocalCalculation>().Count());
            Assert.Equal(5, connection.Table<LocalCalculation>().First().StartOperand);
        }


        [Fact(DisplayName = "Calculation_AddOperationToFirstLocalCalculationTest"), Order(2)]
        public void Calculation_AddOperationToFirstLocalCalculationTest()
        {
            _fixture.CheckOrder();
            var globalCalculation = _fixture.CalculationManager.GetAllGlobalCalculations().First();
            _fixture.CalculationManager.LoadGlobalCalculation(globalCalculation);
            var firstLocalCalculation = globalCalculation.LocalCalculations.First();
            var operation = new Operation {
                OperatorType = OperatorType.Addition,
                Operand = 6
            };

            _fixture.CalculationManager.AddOperation(firstLocalCalculation, operation);
            _fixture.CalculationManager.SetResult(firstLocalCalculation);
            _fixture.CalculationManager.RefreshGlobalResult(globalCalculation);

            Assert.Equal(11, firstLocalCalculation.Result);
            Assert.Equal(11, globalCalculation.Result);
            Assert.NotEmpty(_fixture.Connection.Table<Operation>());
        }

        //...Further Tests
    }
}