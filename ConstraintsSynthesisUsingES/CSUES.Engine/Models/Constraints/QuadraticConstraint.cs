using System.Runtime.CompilerServices;
using CSUES.Engine.Enums;
using CSUES.Engine.Models.Terms;

namespace CSUES.Engine.Models.Constraints
{
    public class QuadraticConstraint : Constraint
    {
        public QuadraticConstraint(Term[] terms, double limitingValue) : base(terms, limitingValue, ConstraintType.Quadratic)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override double GetLeftSideValue(Point point)
        {
            var numberOfDimensions = point.Coordinates.Length;
            var numberOfTerms = Terms.Length;
            var constraintSum = 0.0;

            for (var i = 0; i < numberOfTerms; i += numberOfDimensions)
            {
                for (var j = 0; j < numberOfDimensions; j++)
                {
                    var termIndex = i + j;
                    constraintSum += Terms[termIndex].Coefficient * Terms[termIndex].Value(point.Coordinates[j]);
                }
            }

            return constraintSum;
        }
    }
}