using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Model.Operator
{
    public class Multiplication : Operator
    {
        public Multiplication()
        {
            Label = "*";
            OperatorType = OperatorType.Multiplication;
        }

        public override decimal Calculate(decimal operand1, decimal operand2)
        {
            return operand1 * operand2;
        }

        public override int Weight
        {
            get { return 2; }
        }
    }
}
