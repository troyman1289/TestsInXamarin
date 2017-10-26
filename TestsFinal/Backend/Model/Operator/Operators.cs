using System.Collections.Generic;

namespace Backend.Model.Operator
{
    public static class Operators
    {
        public static Operator Addition { get; }
        public static Operator Substraction { get; }

        static Operators()
        {
            Addition = new Addition();
            Substraction = new Substraction();
        }

        public static List<Operator> GetAll()
        {
            return new List<Operator>
            {
                Addition,
                Substraction
            };
        }
    }
}