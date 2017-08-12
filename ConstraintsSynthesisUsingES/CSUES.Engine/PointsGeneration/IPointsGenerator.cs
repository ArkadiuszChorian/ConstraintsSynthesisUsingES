using ES.Core.Benchmarks;
using ES.Core.Models;

namespace ES.Core.PointsGeneration
{
    public interface IPointsGenerator
    {
        //Point[] GeneratePoints(int numberOfPointsToGenerate, List<Constraint> constraints);
        Point[] GeneratePoints(int numberOfPointsToGenerate, IBenchmark benchmark);
    }
}
