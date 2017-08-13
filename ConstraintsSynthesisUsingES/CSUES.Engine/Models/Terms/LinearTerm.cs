using CSUES.Engine.Enums;

namespace CSUES.Engine.Models.Terms
{
    public class LinearTerm : Term
    {
        public LinearTerm(double coefficient) : base(coefficient, 1, TermType.Linear)
        {           
        }

        public override double Value(double argument)
        {
            return argument;
        }
    }
}