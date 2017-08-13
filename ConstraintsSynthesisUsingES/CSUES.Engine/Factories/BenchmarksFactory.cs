using System;
using CSUES.Engine.Benchmarks;
using CSUES.Engine.Enums;
using CSUES.Engine.Models;

namespace CSUES.Engine.Factories
{
    public class BenchmarksFactory : IGenericFactory<IBenchmark>
    {
        public IBenchmark Create(ExperimentParameters experimentParameters)
        {
            var termsFactory = new TermsFactory();

            switch (experimentParameters.TypeOfBenchmark)
            {
                case BenchmarkType.Balln:
                    return new BallnBenchmark(experimentParameters, termsFactory);
                case BenchmarkType.Cuben:
                    return new CubenBenchmark(experimentParameters, termsFactory);
                case BenchmarkType.Simplexn:
                    return new SimplexnBenchmark(experimentParameters, termsFactory);
                case BenchmarkType.Other:
                    return new GenericBenchmark(experimentParameters);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}