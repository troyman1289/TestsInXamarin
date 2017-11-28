using System.Collections.Generic;

namespace Backend.Model.Operator
{
    public static class Operators
    {
        public static Operator Addition { get; }
        public static Operator Multiplication { get; }

        static Operators()
        {
            Addition = new Addition();
            Multiplication = new Multiplication();
        }

        public static List<Operator> GetAll()
        {
            return new List<Operator>
            {
                Addition,
                Multiplication,
            };
        }
    }
}