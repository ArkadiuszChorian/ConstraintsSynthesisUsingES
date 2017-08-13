using CSUES.Engine.Enums;

namespace CSUES.Engine.Models.Terms
{
    public class CubicTerm : Term
    {
        public CubicTerm(double coefficient) : base(coefficient, 3, TermType.Cubic)
        {
        }

        public override double Value(double argument)
        {
            return argument * argument * argument;
        }
    }
}