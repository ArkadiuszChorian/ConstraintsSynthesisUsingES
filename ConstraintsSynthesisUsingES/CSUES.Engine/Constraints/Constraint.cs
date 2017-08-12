using System.Linq;
using ES.Core.Models;
using ES.Core.Models.Terms;

namespace ES.Core.Constraints
{
    public class Constraint
    {
        protected Constraint(Term[] terms, double limitingValue)
        {
            //TermsCoefficients = termsCoefficients;
            Terms = terms;
            LimitingValue = limitingValue;          
        }

        public Term[] Terms { get; set; }
        //public double[] TermsCoefficients { get; set; }
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
            {
                constraintSum += Terms[i].Value(point.Coordinates[i]);
            }

            return constraintSum <= LimitingValue;
        }       
    }
}
