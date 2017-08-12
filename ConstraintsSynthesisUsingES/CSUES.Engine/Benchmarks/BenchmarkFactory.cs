using System;
using ES.Core.Models;

namespace ES.Core.Benchmarks
{
    public static class BenchmarkFactory
    {
        public static IBenchmark GetBenchmark(ExperimentParameters experimentParameters)
        {
            switch (experimentParameters.TypeOfBenchmark)
            {
                case ExperimentParameters.BenchmarkType.Balln:
                    return new BallnBenchmark(experimentParameters);
                case ExperimentParameters.BenchmarkType.Cuben:
                    return new CubenBenchmark(experimentParameters);
                case ExperimentParameters.BenchmarkType.Simplexn:
                    return new SimplexnBenchmark(experimentParameters);
                case ExperimentParameters.BenchmarkType.Other:
                    return new GeneralBenchmark(experimentParameters.ConstraintsToPointsGeneration, experimentParameters);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
