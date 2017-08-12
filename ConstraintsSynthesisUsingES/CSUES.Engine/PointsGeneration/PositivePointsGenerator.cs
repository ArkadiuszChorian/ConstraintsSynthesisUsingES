using ES.Core.Benchmarks;
using ES.Core.Models;
using ES.Core.Utils;

namespace ES.Core.PointsGeneration
{
    public class PositivePointsGenerator : IPointsGenerator
    {
        private readonly MersenneTwister _randomGenerator;

        public PositivePointsGenerator()
        {
            _randomGenerator = MersenneTwister.Instance;
        }
           
        public Point[] GeneratePoints(int numberOfPointsToGenerate, IBenchmark benchmark)
        {
            //TODO: Check if constraints have common space. Now, if they don't have, algorithm will stuck in while loop.

            var numberOfDimensions = benchmark.Domains.Length;
            var constraints = benchmark.Constraints;
            var numberOfConstraints = constraints.Length;         
            var points = new Point[numberOfPointsToGenerate];

            for (var i = 0; i < numberOfPointsToGenerate; i++)
            {
                points[i] = new Point(numberOfDimensions, ClassificationType.Positive);
                var currentPoint = points[i];
                var isSatsfyngConstraints = false;

                while (isSatsfyngConstraints == false)
                {
                    isSatsfyngConstraints = true;

                    for (var j = 0; j < numberOfDimensions; j++)
                    {
                        currentPoint.Coordinates[j] = _randomGenerator.NextDouble(benchmark.Domains[j].LowerLimit, benchmark.Domains[j].UpperLimit);
                    }

                    for (var j = 0; j < numberOfConstraints; j++)
                    {
                        if (constraints[j].IsSatisfyingConstraint(currentPoint)) continue;
                        isSatsfyngConstraints = false;
                        break;
                    }
                }
            }

            return points;
        }
    }
}
