using CSUES.Engine.Enums;

namespace CSUES.Engine.Models.Terms
{
    public class CubicTerm : Term
    {
        public CubicTerm(double coefficient) : base(coefficient, null, TermType.Cubic)
        {
        }

        public override double Value(params double[] arguments)
        {
            return arguments[0] * arguments[0] * arguments[0];
        }
    }
}