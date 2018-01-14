using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Business;
using Backend.Model;

namespace xUnit.IntegrationTest
{
    public class CalculationHelper
    {
        public static GlobalCalculation CreateNewGlobalCalculationAndOneOperation(
            CalculationManager manager)
        {
            var globalCalculation = new GlobalCalculation();
            manager.AddNewGlobalCalculation(globalCalculation, 8);
            var localCalculation = globalCalculation.LocalCalculations.First();
            var operation = new Operation() {
                Operand = 2,
                OperatorType = OperatorType.Addition,
            };
            manager.AddOperation(localCalculation, operation);
            manager.SetResult(localCalculation);

            return globalCalculation;
        }
    }
}
