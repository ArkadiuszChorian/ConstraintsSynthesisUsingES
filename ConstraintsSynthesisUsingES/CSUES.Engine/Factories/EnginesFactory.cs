using System.Diagnostics;
using CSUES.Engine.Core;
using CSUES.Engine.Measurement;
using CSUES.Engine.Models;
using CSUES.Engine.PointsGeneration;
using CSUES.Engine.PrePostProcessing;

namespace CSUES.Engine.Factories
{
    public class EnginesFactory : IGenericFactory<IEngine>
    {
        public IEngine Create(ExperimentParameters experimentParameters)
        {
            var benchmarksFactory = new BenchmarksFactory();
            var pointsGenerator = new PointsGenerator();
            var benchmark = benchmarksFactory.Create(experimentParameters);
            var constraintsBuilder = new ConstraintsBuilder(benchmark.Constraints, experimentParameters);
            var pointsNormalizer = new StandardScorePointsNormalizer();
            var redundantConstraintsRemover = new RedundantConstraintsRemover(pointsGenerator, benchmark.Domains, experimentParameters);
            var meanAngleCalculator = new MeanAngleCalculator();
            var stoper = new Stopwatch();

            return new Core.Engine(experimentParameters, benchmark, constraintsBuilder, pointsNormalizer, redundantConstraintsRemover, meanAngleCalculator, stoper);
        }
    }
}