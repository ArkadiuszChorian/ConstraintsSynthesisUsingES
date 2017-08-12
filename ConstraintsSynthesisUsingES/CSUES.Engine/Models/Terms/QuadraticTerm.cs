using ES.Core.Enums;

namespace ES.Core.Models.Terms
{
    public class QuadraticTerm : Term
    {
        public QuadraticTerm(double coefficient) : base(coefficient)
        {
        }

        public QuadraticTerm(double coefficient, TermType termType) : base(coefficient, termType)
        {
        }

        public override double Value(double argument)
        {
            return argument * argument;
        }
    }
}