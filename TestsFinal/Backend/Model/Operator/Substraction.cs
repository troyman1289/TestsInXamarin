namespace Backend.Model.Operator
{
    internal class Substraction : Operator
    {
        public Substraction()
        {
            Label = "-";
            OperatorType = OperatorType.Subtraction;
        }

        public override decimal Calculate(decimal operand1, decimal operand2)
        {
            return operand1 - operand2;
        }
    }
}