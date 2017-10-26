namespace Backend.Model.Operator
{
    internal class Addition : Operator
    {
        public Addition()
        {
            Label = "+";
            OperatorType = OperatorType.Addition;
        }

        public override decimal Calculate(decimal operand1, decimal operand2)
        {
            return operand1 + operand2;
        }
    }
}