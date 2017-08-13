using CSUES.Engine.Enums;
using CSUES.Engine.Models;
using ES.Core.Utils;

namespace CSUES.Engine.PointsGeneration
{
    public class PointsGenerator
    {
        private readonly MersenneTwister _randomGenerator;

        public PointsGenerator()
        {
            _randomGenerator = MersenneTwister.Instance;
        }

        public virtual Point[] GeneratePoints(int numberOfPointsToGenerate, Domain[] domains, Constraint[] constraints = null)
        {
            var points = new Point[numberOfPointsToGenerate];

            for (var i = 0; i < numberOfPointsToGenerate; i++)
                points[i] = GetAllowedPoint(domains, constraints);

            return points;
        }

        protected virtual Point GetAllowedPoint(Domain[] domains, Constraint[] constraints)
        {
            var numberOfDimensions = domains.Length;
            var point = new Point(numberOfDimensions, ClassificationType.Neutral);

            for (var i = 0; i < numberOfDimensions; i++)
                point.Coordinates[i] = _randomGenerator.NextDouble(domains[i].LowerLimit, domains[i].UpperLimit);

            return point;
        }
    }
}