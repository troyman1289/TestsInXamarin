namespace Backend.Model.Operator
{
    internal class Subtraction : Operator
    {
        public Subtraction()
        {
            Label = "-";
            OperatorType = OperatorType.Subtraction;
        }

        public override decimal Calculate(decimal operand1, decimal operand2)
        {
            return operand1 - operand2;
        }

        public override int Weight
        {
            get { return 1; }
        }
    }
}