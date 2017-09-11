using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CSUES.Engine.Benchmarks;
using CSUES.Engine.Enums;
using CSUES.Engine.Measurement;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using CSUES.Engine.PrePostProcessing;
using CSUES.Engine.Utils;
using ES.Core.Factories;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.PopulationGeneration;
using ES.Core.Utils;
using Statistics = CSUES.Engine.Models.Statistics;

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
            NormalizedEvolutionSteps = new List<IList<Constraint>>();

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
        public IList<Point> NormalizedTrainingPoints { get; set; }
        public IList<Constraint> NormalizedSynthesizedConstraints { get; set; }
        public IList<IList<Constraint>> NormalizedEvolutionSteps { get; }
        public IDictionary<int, EvolutionStep> CoreEvolutionSteps { get; set; }

        public MathModel SynthesizeModel(Point[] trainingPoints)
        {
            var means = trainingPoints.Means();
            var standardDeviations = trainingPoints.StandardDeviations(means);

            if (Parameters.UseDataNormalization)
            {               
                trainingPoints = _pointsNormalizer.ApplyProcessing(trainingPoints);
                NormalizedTrainingPoints = trainingPoints.DeepCopyByExpressionTree();
            }

            var evolutionEnginesFactory = new EnginesFactory();
            var evolutionEngine = evolutionEnginesFactory.Create(Parameters.EvolutionParameters);
            var positiveTrainingPoints = trainingPoints.Where(tp => tp.ClassificationType == ClassificationType.Positive).ToArray();
            var negativeTrainingPoints = trainingPoints.Where(tp => tp.ClassificationType == ClassificationType.Negative).ToArray();
            var evaluator = new Evaluator(positiveTrainingPoints, negativeTrainingPoints, _constraintsBuilder);

            //HACK TODO

            //var numberOfConstraintsCoefficients = Parameters.MaximumNumberOfConstraints * (Parameters.NumberOfDimensions + 1);
            //var constraintsCoefficients = new List<double>(numberOfConstraintsCoefficients);
            //var benchmarkConstraintsIndexer = 0;

            //for (var i = 0; i < Parameters.MaximumNumberOfConstraints; i++)
            //{
            //    if (benchmarkConstraintsIndexer >= Benchmark.Constraints.Length)
            //        benchmarkConstraintsIndexer = 0;
                
            //    constraintsCoefficients.AddRange(Benchmark.Constraints[benchmarkConstraintsIndexer].Terms.Select(t => t.Coefficient));
            //    constraintsCoefficients.Add(Benchmark.Constraints[benchmarkConstraintsIndexer++].LimitingValue);
            //}

            //PopulationGeneratorBase.ObjectCoefficients = constraintsCoefficients.ToArray();

            //
            
            Solution bestSolution;

            if (Parameters.UseSeeding)
            {
                var singleConstraintModel = Parameters.TypeOfBenchmark == BenchmarkType.Balln && Parameters.AllowQuadraticTerms
                    ? Benchmark.Constraints.Take(1).DeepCopyByExpressionTree().ToArray()
                    : new[] { new LinearConstraint(Benchmark.Constraints.First().Terms.Where(t => t.Type == TermType.Linear).ToArray(), 0) };
                var singleConstraintBuilder = new ConstraintsBuilder(singleConstraintModel);
                var singleConstraintEvaluator = new Evaluator(positiveTrainingPoints, negativeTrainingPoints, singleConstraintBuilder);
                var seedingProcessor = new SeedingProcessor(singleConstraintEvaluator, _constraintsBuilder, positiveTrainingPoints);

                bestSolution = evolutionEngine.RunEvolution(evaluator, seedingProcessor);
            }
            else
            {
                bestSolution = evolutionEngine.RunEvolution(evaluator);
            }

            CoreEvolutionSteps = evolutionEngine.EvolutionSteps;

            Statistics.EvolutionStatistics = evolutionEngine.Statistics;
            var synthesizedConstraints = _constraintsBuilder.BuildConstraints(bestSolution);

            if (Parameters.UseDataNormalization)
            {
                NormalizedSynthesizedConstraints = synthesizedConstraints.DeepCopyByExpressionTree();
                _constaintsDenormalizer = new StandardScoreConstraintsDenormalizer(means, standardDeviations);
                synthesizedConstraints = _constaintsDenormalizer.ApplyProcessing(synthesizedConstraints);
            }

            if (Parameters.TrackEvolutionSteps)
            {
                var evolutionStepsAsSolutions = evolutionEngine.EvolutionStepsSimple.ToList();

                foreach (var evolutionStepsAsSolution in evolutionStepsAsSolutions)
                {
                    var evolutionStep = _constraintsBuilder.BuildConstraints(evolutionStepsAsSolution);                   

                    if (Parameters.UseDataNormalization)
                    {
                        NormalizedEvolutionSteps.Add(evolutionStep.DeepCopyByExpressionTree());
                        evolutionStep = _constaintsDenormalizer.ApplyProcessing(evolutionStep);                        
                    }

                    EvolutionSteps.Add(evolutionStep);
                }
            }

            if (Parameters.UseRedundantConstraintsRemoving)
            {
                _stoper.Restart();
                synthesizedConstraints = _redundantConstraintsRemover.ApplyProcessing(synthesizedConstraints);
                _stoper.Stop();
                Statistics.RedundantConstraintsRemovingTime = _stoper.Elapsed;
            }               

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