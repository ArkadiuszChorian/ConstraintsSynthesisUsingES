using System.Linq;
using CSUES.Engine.Models.Terms;

namespace CSUES.Engine.Models
{
    public class Constraint
    {
        public Constraint(Term[] terms, double limitingValue)
        {
            Terms = terms;
            LimitingValue = limitingValue;          
        }

        public Term[] Terms { get; set; }
        public double LimitingValue { get; set; }

        public double[] GetTermsCoefficients()
        {
            return Terms.Select(t => t.Coefficient).ToArray();
        }

        public bool IsSatisfyingConstraint(Point point)
        {
            var constraintSum = 0.0;
            var numberOfTerms = Terms.Length;

            for (var i = 0; i < numberOfTerms; i++)
                constraintSum += Terms[i].Value(point.Coordinates[i]);

            return constraintSum <= LimitingValue;
        }       
    }
}
