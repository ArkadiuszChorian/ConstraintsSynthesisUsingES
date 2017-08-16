using CSUES.Engine.Models.Terms;

namespace CSUES.Engine.Models.Constraints
{
    public abstract class Constraint
    {
        protected Constraint(Term[] terms, double limitingValue)
        {
            Terms = terms;
            LimitingValue = limitingValue;          
        }

        public Term[] Terms { get; set; }
        public double LimitingValue { get; set; }

        //public double[] GetTermsCoefficients()
        //{
        //    return Terms.Select(t => t.Coefficient).ToArray();
        //}

        public abstract bool IsSatisfyingConstraint(Point point);
    }
}
