using ES.Core.Enums;

namespace ES.Core.Models.Terms
{
    public class CubicTerm : Term
    {
        public CubicTerm(double coefficient) : base(coefficient)
        {
        }

        public CubicTerm(double coefficient, TermType termType) : base(coefficient, termType)
        {
        }

        public override double Value(double argument)
        {
            return argument * argument * argument;
        }
    }
}