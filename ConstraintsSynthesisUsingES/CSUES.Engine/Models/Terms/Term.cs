using CSUES.Engine.Enums;

namespace CSUES.Engine.Models.Terms
{
    public abstract class Term
    {
        protected Term(double coefficient, double power, TermType termType)
        {
            Coefficient = coefficient;
            Power = power;
            Type = termType;
        }

        public abstract double Value(double argument);

        public double Coefficient { get; set; }
        public double Power { get; private set; }
        public TermType Type { get; private set; }
    }
}