using System.Collections.Generic;

namespace Backend.Model.Operator
{
    public static class Operators
    {
        public static Operator Addition { get; }
        public static Operator Substraction { get; }
        public static Operator Multiplication { get; }
        public static Operator Division { get; }

        static Operators()
        {
            Addition = new Addition();
            Substraction = new Substraction();
            Multiplication = new Multiplication();
            Division = new Division();
        }

        public static List<Operator> GetAll()
        {
            return new List<Operator>
            {
                Addition,
                Substraction,
                Multiplication,
                Division
            };
        }
    }
}