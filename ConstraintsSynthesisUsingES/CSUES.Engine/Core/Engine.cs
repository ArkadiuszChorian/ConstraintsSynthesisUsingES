using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CSUES.Engine.Benchmarks;
using CSUES.Engine.Enums;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using CSUES.Engine.PrePostProcessing;
using CSUES.Engine.Utils;
using ES.Core.Factories;

namespace CSUES.Engine.Core
{
    public class Engine : IEngine
    {
        private readonly IConstraintsBuilder _constraintsBuilder;
        private readonly IProcessor<Constraint[]> _redundantConstraintsRemover;      
        private readonly Stopwatch _stoper;

        public Engine(ExperimentParameters experimentParameters, IBenchmark benchmark, IConstraintsBuilder constraintsBuilder, IProcessor<Constraint[]> redundantConstraintsRemover, Stopwatch stoper)
        {
            Parameters = experimentParameters;
            Benchmark = benchmark;
            _constraintsBuilder = constraintsBuilder;
            _redundantConstraintsRemover = redundantConstraintsRemover;
            _stoper = stoper;
            Statistics = new Statistics();
            EvolutionSteps = new List<IList<Constraint>>();
        }

        public IBenchmark Benchmark { get; set; }
        public ExperimentParameters Parameters { get; set; }
        public Statistics Statistics { get; }
        public MathModel MathModel { get; private set; }        
        public IList<IList<Constraint>> EvolutionSteps { get; }
        
        public MathModel SynthesizeModel(Point[] trainingPoints)
        {
            var evolutionEnginesFactory = new EnginesFactory();
            var evolutionEngine = evolutionEnginesFactory.Create(Parameters.EvolutionParameters);
            var positiveTrainingPoints = trainingPoints.Where(tp => tp.ClassificationType == ClassificationType.Positive).ToArray();
            var negativeTrainingPoints = trainingPoints.Where(tp => tp.ClassificationType == ClassificationType.Negative).ToArray();
            var evaluator = new Evaluator(positiveTrainingPoints, negativeTrainingPoints, _constraintsBuilder);

            var bestSolution = evolutionEngine.RunEvolution(evaluator);

            if (Parameters.TrackEvolutionSteps)
            {
                var evolutionStepsAsSolutions = evolutionEngine.EvolutionSteps.ToList();
                evolutionStepsAsSolutions.ForEach(es => EvolutionSteps.Add(_constraintsBuilder.BuildConstraints(es)));
            }

            Statistics.EvolutionStatistics = evolutionEngine.Statistics;
            var synthesizedConstraints = _constraintsBuilder.BuildConstraints(bestSolution);

            if (Parameters.UseRedundantConstraintsRemoving)
                synthesizedConstraints = _redundantConstraintsRemover.ApplyProcessing(synthesizedConstraints);

            MathModel = new MathModel(synthesizedConstraints, Benchmark);

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