using System.Runtime.CompilerServices;
using CSUES.Engine.Enums;
using CSUES.Engine.Models.Terms;

namespace CSUES.Engine.Models.Constraints
{
    public class LinearConstraint : Constraint
    {
        public LinearConstraint(Term[] terms, double limitingValue) : base(terms, limitingValue, ConstraintType.Linear)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override double GetLeftSideValue(Point point)
        {
            var numberOfTerms = Terms.Length;
            var constraintSum = 0.0;

            for (var i = 0; i < numberOfTerms; i++)
                constraintSum += Terms[i].Coefficient * Terms[i].Value(point.Coordinates[i]);

            return constraintSum;
        }
    }
}