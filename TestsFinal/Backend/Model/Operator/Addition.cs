namespace Backend.Model.Operator
{
    internal class Addition : Operator
    {
        public Addition()
        {
            Label = "+";
            OperatorType = OperatorType.Addition;
        }

        public override decimal Calculate(int operand1, int operand2)
        {
            return operand1 + operand2;
        }

        public override int Weight
        {
            get { return 1; }
        }
    }
}