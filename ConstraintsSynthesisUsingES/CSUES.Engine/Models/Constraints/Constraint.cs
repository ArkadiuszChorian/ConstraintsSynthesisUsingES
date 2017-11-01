using System.Linq;
using System.Runtime.CompilerServices;
using CSUES.Engine.Enums;
using CSUES.Engine.Models.Terms;

namespace CSUES.Engine.Models.Constraints
{
    public class Constraint
    {
        public Constraint(Term[] terms, double limitingValue, ConstraintType constraintType)
        {
            Terms = terms;
            LimitingValue = limitingValue;
            Type = constraintType;
        }

        public Term[] Terms { get; set; }
        public double LimitingValue { get; set; }
        public ConstraintType Type { get; set; }

        public double[] GetTermsCoefficients()
        {
            var numberOfTerms = Terms.Length;
            var termsCoefficients = new double[Terms.Length];

            for (var i = 0; i < numberOfTerms; i++)
                termsCoefficients[i] = Terms[i].Coefficient;

            return termsCoefficients;
        }

        public double[] GetAllCoefficients()
        {
            var numberOfTerms = Terms.Length;
            var allCoefficients = new double[Terms.Length + 1];

            for (var i = 0; i < numberOfTerms; i++)
                allCoefficients[i] = Terms[i].Coefficient;

            allCoefficients[numberOfTerms] = LimitingValue;

            return allCoefficients;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double GetLeftSideValue(Point point)
        {
            var constraintSum = 0.0;

            foreach (var term in Terms)
                constraintSum += term.Coefficient * term.Value(point.Coordinates);

            return constraintSum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSatisfyingConstraint(Point point)
        {           
            return GetLeftSideValue(point) <= LimitingValue;
        }
    }
}
