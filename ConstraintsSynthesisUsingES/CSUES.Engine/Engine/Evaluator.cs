using CSUES.Engine.Models;
using CSUES.Engine.Utils;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace CSUES.Engine.Engine
{
    public class Evaluator : IEvaluator
    {
        private readonly Point[] _positivePoints;
        private readonly Point[] _negativePoints;
        private readonly int _numberOfConstraintCoefficients;
       
        public Evaluator(int numberOfConstraintCoefficients, Point[] positivePoints, Point[] negativePoints)
        {
            _positivePoints = positivePoints;
            _negativePoints = negativePoints;

            _numberOfConstraintCoefficients = numberOfConstraintCoefficients;
        }
        
        public double Evaluate(Solution solution)
        {
            var numberOfPositivePointsSatisfyingConstraints = 0;
            var numberOfNegativePointsSatisfyingConstraints = 0;
            var constraints = solution.GetConstraints(_numberOfConstraintCoefficients);

            foreach (var positivePoint in _positivePoints)
            {
                if (constraints.IsSatisfyingConstraints(positivePoint))
                    numberOfPositivePointsSatisfyingConstraints++;
            }

            foreach (var negativePoint in _negativePoints)
            {
                if (constraints.IsSatisfyingConstraints(negativePoint))
                    numberOfNegativePointsSatisfyingConstraints++;
            }

            return (double)numberOfPositivePointsSatisfyingConstraints / (_positivePoints.Length + numberOfNegativePointsSatisfyingConstraints);
        }
    }
}
