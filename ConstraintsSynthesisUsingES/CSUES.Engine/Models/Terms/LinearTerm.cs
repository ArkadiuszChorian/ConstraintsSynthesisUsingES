using ES.Core.Enums;

namespace ES.Core.Models.Terms
{
    public class LinearTerm : Term
    {
        public LinearTerm(double coefficient) : base(coefficient)
        {
        }

        public LinearTerm(double coefficient, TermType termType) : base(coefficient, termType)
        {           
        }

        public override double Value(double argument)
        {
            return argument;
        }
    }
}