using CSUES.Engine.Enums;

namespace CSUES.Engine.Models.Terms
{
    public class QuadraticTerm : Term
    {
        public QuadraticTerm(double coefficient) : base(coefficient, 2, TermType.Quadratic)
        {
        }

        public override double Value(double argument)
        {
            return argument * argument;
        }
    }
}