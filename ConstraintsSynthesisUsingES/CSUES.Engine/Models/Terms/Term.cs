using ES.Core.Enums;

namespace ES.Core.Models.Terms
{
    public abstract class Term
    {
        protected Term(double coefficient)
        {
            Coefficient = coefficient;
        }

        protected Term(double coefficient, TermType termType)
        {
            Coefficient = coefficient;
            Type = termType;
        }

        public abstract double Value(double argument);

        public double Coefficient { get; set; }
        public TermType Type { get; set; }
    }
}