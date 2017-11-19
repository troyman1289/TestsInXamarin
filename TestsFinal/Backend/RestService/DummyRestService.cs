using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Interfaces;
using Backend.Model;

namespace Backend.RestService
{
    public class DummyRestService : IRestService
    {
        public async Task<IList<GlobalCalculation>> FetchGlobalCalculations()
        {
            return await Task.Run(async () => {
                var globalCalculation = new GlobalCalculation { Label = "FromDummy", Result = 12 };
                var localCalculation = new LocalCalculation { StartOperand = 5, Result = 12 };
                var operation = new Operation {
                    Order = 1,
                    OperatorType = OperatorType.Addition,
                    Operand = 5
                };
                localCalculation.Operations.Add(operation);
                operation = new Operation {
                    Order = 1,
                    OperatorType = OperatorType.Addition,
                    Operand = 2
                };
                localCalculation.Operations.Add(operation);

                await Task.Delay(5000);
                return new List<GlobalCalculation> { globalCalculation };
            });
        }
    }
}
