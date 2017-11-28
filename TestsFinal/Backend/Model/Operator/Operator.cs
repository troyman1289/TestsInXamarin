namespace Backend.Model.Operator
{
    public abstract class Operator
    {
        public OperatorType OperatorType { get; protected set; }

        public string Label { get; protected set; }

        public abstract decimal Calculate(int operand1, int operand2);

        public abstract int Weight { get; }
    }
}