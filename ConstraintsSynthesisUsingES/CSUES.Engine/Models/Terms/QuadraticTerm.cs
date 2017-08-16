using CSUES.Engine.Enums;

namespace CSUES.Engine.Models.Terms
{
    public class QuadraticTerm : Term
    {
        public QuadraticTerm(double coefficient) : base(coefficient, 2, TermType.Quadratic)
        {
        }

        public override double Value(params double[] arguments)
        {
            return arguments[0] * arguments[0];
        }
    }
}