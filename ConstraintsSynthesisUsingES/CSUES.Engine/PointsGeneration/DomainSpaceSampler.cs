using ES.Core.Benchmarks;
using ES.Core.Models;
using ES.Core.Utils;

namespace ES.Core.PointsGeneration
{
    public class DomainSpaceSampler : IPointsGenerator
    {
        private readonly MersenneTwister _randomGenerator;

        public DomainSpaceSampler()
        {
            _randomGenerator = MersenneTwister.Instance;
        }

        public Point[] GeneratePoints(int numberOfPointsToGenerate, IBenchmark benchmark)
        {
            var numberOfDimensions = benchmark.Domains.Length;
            var points = new Point[numberOfPointsToGenerate];

            for (var i = 0; i < numberOfPointsToGenerate; i++)
            {
                points[i] = new Point(numberOfDimensions);
                var currentPoint = points[i];

                for (var j = 0; j < numberOfDimensions; j++)
                {
                    currentPoint.Coordinates[j] = _randomGenerator.NextDouble(benchmark.Domains[j].LowerLimit, benchmark.Domains[j].UpperLimit);
                }
            }

            return points;
        }

        //public Point[] GeneratePoints(int numberOfPointsToGenerate, IBenchmark benchmark)
        //{
        //    var numberOfDimensions = benchmark.Domains.Length;
        //    var points = new Point[numberOfPointsToGenerate];

        //    for (var i = 0; i < numberOfPointsToGenerate; i++)
        //    {
        //        points[i] = new Point(numberOfDimensions);
        //        var currentPoint = points[i];

        //        for (var j = 0; j < numberOfDimensions; j++)
        //        {
        //            currentPoint.Coordinates[j] = _randomGenerator.NextDouble(benchmark.Domains[j].LowerLimit, benchmark.Domains[j].UpperLimit);
        //        }
        //    }

        //    return points;
        //}
    }
}
