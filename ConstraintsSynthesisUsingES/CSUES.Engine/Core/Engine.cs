using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Accord.Statistics;
using Accord.Statistics.Distributions.Univariate;
using CSUES.Engine.Benchmarks;
using CSUES.Engine.Enums;
using CSUES.Engine.Measurement;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using CSUES.Engine.PrePostProcessing;
using CSUES.Engine.Utils;
using ES.Core.Factories;
using ES.Core.Utils;

namespace CSUES.Engine.Core
{
    public class Engine : IEngine
    {
        private readonly IConstraintsBuilder _constraintsBuilder;
        private readonly IProcessor<Point[]> _pointsNormalizer;
        private IProcessor<Constraint[]> _constaintsDenormalizer;
        private readonly IProcessor<Constraint[]> _redundantConstraintsRemover;
        private readonly IAngleCalculator _meanAngleCalculator;
        private readonly Stopwatch _stoper;

        public Engine(ExperimentParameters experimentParameters, IBenchmark benchmark, IConstraintsBuilder constraintsBuilder, IProcessor<Point[]> pointsNormalizer, IProcessor<Constraint[]> redundantConstraintsRemover, IAngleCalculator meanAngleCalculator, Stopwatch stoper)
        {
            Parameters = experimentParameters;
            Benchmark = benchmark;
            _constraintsBuilder = constraintsBuilder;
            _redundantConstraintsRemover = redundantConstraintsRemover;
            _stoper = stoper;
            _meanAngleCalculator = meanAngleCalculator;
            _pointsNormalizer = pointsNormalizer;
            Statistics = new Statistics();
            EvolutionSteps = new List<IList<Constraint>>();

            MersenneTwister.Initialize(experimentParameters.Seed);

            //var normalDis = new NormalDistribution();

            //const int count = int.MaxValue / 1000;
            //var values = new double[count];
            //////var sum = 0.0;
            //for (int i = 0; i < count; i++)
            //{
            //    values[i] = MersenneTwister.Instance.NextGaussian();
            //}
            ////values = normalDis.Generate(count);

            //var mean = values.Mean();
            //var stdDev = values.StandardDeviation();
        }

        public IBenchmark Benchmark { get; set; }
        public ExperimentParameters Parameters { get; set; }
        public Statistics Statistics { get; }
        public MathModel MathModel { get; private set; }        
        public IList<IList<Constraint>> EvolutionSteps { get; }
        
        public MathModel SynthesizeModel(Point[] trainingPoints)
        {
            var means = trainingPoints.Means();
            var standardDeviations = trainingPoints.StandardDeviations(means);

            if (Parameters.UseDataNormalization)
                trainingPoints = _pointsNormalizer.ApplyProcessing(trainingPoints);

            var evolutionEnginesFactory = new EnginesFactory();
            var evolutionEngine = evolutionEnginesFactory.Create(Parameters.EvolutionParameters);
            var positiveTrainingPoints = trainingPoints.Where(tp => tp.ClassificationType == ClassificationType.Positive).ToArray();
            var negativeTrainingPoints = trainingPoints.Where(tp => tp.ClassificationType == ClassificationType.Negative).ToArray();
            var evaluator = new Evaluator(positiveTrainingPoints, negativeTrainingPoints, _constraintsBuilder);

            var bestSolution = evolutionEngine.RunEvolution(evaluator);

            Statistics.EvolutionStatistics = evolutionEngine.Statistics;
            var synthesizedConstraints = _constraintsBuilder.BuildConstraints(bestSolution);

            if (Parameters.UseDataNormalization)
            {
                _constaintsDenormalizer = new StandardScoreConstraintsDenormalizer(means, standardDeviations);
                synthesizedConstraints = _constaintsDenormalizer.ApplyProcessing(synthesizedConstraints);
            }

            if (Parameters.TrackEvolutionSteps)
            {
                var evolutionStepsAsSolutions = evolutionEngine.EvolutionSteps.ToList();

                foreach (var evolutionStepsAsSolution in evolutionStepsAsSolutions)
                {
                    var evolutionStep = _constraintsBuilder.BuildConstraints(evolutionStepsAsSolution);

                    if (Parameters.UseDataNormalization)
                        evolutionStep = _constaintsDenormalizer.ApplyProcessing(evolutionStep);

                    EvolutionSteps.Add(evolutionStep);
                }
            }

            if (Parameters.UseRedundantConstraintsRemoving)
                synthesizedConstraints = _redundantConstraintsRemover.ApplyProcessing(synthesizedConstraints);

            MathModel = new MathModel(synthesizedConstraints, Benchmark);
            Statistics.NumberOfConstraints = synthesizedConstraints.Length;

            Statistics.MeanAngle = Parameters.TypeOfBenchmark != BenchmarkType.Balln ? _meanAngleCalculator.Calculate(synthesizedConstraints, Benchmark.Constraints) : double.NaN;
            
            return MathModel;
        }

        public Statistics EvaluateModel(Point[] testPoints)
        {
            var constraints = MathModel.SynthesizedModel;

            _stoper.Restart();

            foreach (var point in testPoints)
            {
                if (constraints.IsSatisfyingConstraints(point))
                {
                    switch (point.ClassificationType)
                    {
                        case ClassificationType.Negative:
                            Statistics.FalsePositives++;
                            break;
                        case ClassificationType.Positive:
                            Statistics.TruePositives++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    switch (point.ClassificationType)
                    {
                        case ClassificationType.Negative:
                            Statistics.TrueNegatives++;
                            break;
                        case ClassificationType.Positive:
                            Statistics.FalseNegatives++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            _stoper.Stop();
            Statistics.ModelEvaluationTime = _stoper.Elapsed;
            _stoper.Reset();

            return Statistics;
        }     
    }
}