using System;
using System.Diagnostics;
using System.Linq;
using CSUES.Engine.Engine;
using ES.Core.Benchmarks;
using ES.Core.Constraints;
using ES.Core.Evaluation;
using ES.Core.Logging;
using ES.Core.Models;
using ES.Core.Mutation;
using ES.Core.MutationSupervison;
using ES.Core.PopulationGeneration;
using ES.Core.PrePostProcessing;
using ES.Core.Selection;
using ES.Core.Utils;

namespace ES.Core.Engine
{
    public abstract class EngineBase : IEngine
    {
        //protected IBenchmark Benchmark;
        protected IPopulationGenerator PopulationGenerator;
        protected IEvaluator Evaluator;
        protected ILogger Logger;
        protected IMutator ObjectMutator;
        protected IMutator StdDeviationsMutator;
        protected IMutationRuleSupervisor MutationRuleSupervisor;
        protected IParentsSelector ParentsSelector;
        protected ISurvivorsSelector SurvivorsSelector;
        protected IProcessor<Constraint[]> RedundantConstriantsRemover;
        protected Solution[] BasePopulation;
        protected Solution[] OffspringPopulation;
        protected Constraint[] SynthesizedModel;
        protected Solution[] InitialPopulation;
        protected Stopwatch Stoper;

        //protected EngineBase(IBenchmark benchmark, IPopulationGenerator populationGenerator, IEvaluator evaluator, ILogger logger, IMutator objectMutator, IMutator stdDeviationsMutator, IMutationRuleSupervisor mutationRuleSupervisor, IParentsSelector parentsParentsSelector, ISurvivorsSelector survivorsSelector, IPointsGenerator positivePointsGenerator, IPointsGenerator negativePointsGenerator, ExperimentParameters experimentParameters, Solution[] basePopulation, Solution[] offspringPopulation)
        //{
        //    Benchmark = benchmark;
        //    PopulationGenerator = populationGenerator;
        //    Evaluator = evaluator;
        //    Logger = logger;
        //    ObjectMutator = objectMutator;
        //    StdDeviationsMutator = stdDeviationsMutator;
        //    MutationRuleSupervisor = mutationRuleSupervisor;
        //    ParentsSelector = parentsParentsSelector;
        //    SurvivorsSelector = survivorsSelector;
        //    PositivePointsGenerator = positivePointsGenerator;
        //    NegativePointsGenerator = negativePointsGenerator;
        //    ExperimentParameters = experimentParameters;
        //    BasePopulation = basePopulation;
        //    OffspringPopulation = offspringPopulation;
        //}

        protected EngineBase(IBenchmark benchmark, IPopulationGenerator populationGenerator, IEvaluator evaluator, ILogger logger, IMutator objectMutator, IMutator stdDeviationsMutator, IMutationRuleSupervisor mutationRuleSupervisor, IParentsSelector parentsParentsSelector, ISurvivorsSelector survivorsSelector, IProcessor<Constraint[]> redundantConstrainsRemover, ExperimentParameters experimentParameters, Statistics statistics, Solution[] basePopulation, Solution[] offspringPopulation)
        {
            Benchmark = benchmark;
            PopulationGenerator = populationGenerator;
            Evaluator = evaluator;
            Logger = logger;
            ObjectMutator = objectMutator;
            StdDeviationsMutator = stdDeviationsMutator;
            MutationRuleSupervisor = mutationRuleSupervisor;
            ParentsSelector = parentsParentsSelector;
            SurvivorsSelector = survivorsSelector;
            RedundantConstriantsRemover = redundantConstrainsRemover;
            ExperimentParameters = experimentParameters;
            Statistics = statistics;
            BasePopulation = basePopulation;
            OffspringPopulation = offspringPopulation;
            Stoper = new Stopwatch();
        }

        public MathModel MathModel { get; set; }
        public ExperimentParameters ExperimentParameters { get; set; }       
        public Statistics Statistics { get; set; }
        public IBenchmark Benchmark { get; set; }       

        //public IList<Constraint> GetSynthesizedModel()
        //{
        //    return SynthesizedModel;
        //}

        //public IList<Constraint> GetReferenceModel()
        //{
        //    return Benchmark.Constraints;
        //}

        public virtual MathModel SynthesizeModel(Point[] trainingPoints)
        {
            var offspringPopulationSize = ExperimentParameters.OffspringPopulationSize;
            var numberOfGenerations = ExperimentParameters.NumberOfGenerations;          
                
            Evaluator.PositivePoints = trainingPoints.Where(tp => tp.ClassificationType == ClassificationType.Positive).ToArray();
            Evaluator.NegativePoints = trainingPoints.Where(tp => tp.ClassificationType == ClassificationType.Negative).ToArray();

            BasePopulation = PopulationGenerator.GeneratePopulation(ExperimentParameters);

            for (var i = 0; i < offspringPopulationSize; i++)
                OffspringPopulation[i] = new Solution(ExperimentParameters);

            InitialPopulation = BasePopulation.DeepCopyByExpressionTree();

            Stoper.Restart();

            for (var i = 0; i < numberOfGenerations; i++)
            {
                Evolve(offspringPopulationSize);                             
            }

            Stoper.Stop();

            Statistics.TotalEvolutionTime = Stoper.Elapsed;
            Statistics.MeanSingleGenerationEvolutionTime = TimeSpan.FromTicks(Statistics.TotalEvolutionTime.Ticks / numberOfGenerations);

            Stoper.Restart();

            SynthesizedModel = RedundantConstriantsRemover.ApplyProcessing(BasePopulation.First().GetConstraints(ExperimentParameters));

            Stoper.Stop();

            Statistics.RedundantConstraintsRemovingTime = Stoper.Elapsed;
            Statistics.TotalSynthesisTime = Statistics.TotalEvolutionTime + Stoper.Elapsed;

            Stoper.Reset();

            MathModel = new MathModel(SynthesizedModel, Benchmark);
            return MathModel;
        }

        public virtual Statistics EvaluateModel(Point[] testPoints)
        {           
            var numberOfPoints = testPoints.Length;
            var constraints = MathModel.SynthesizedModel;

            Stoper.Restart();

            for (var i = 0; i < numberOfPoints; i++)
            {
                if (constraints.IsSatisfyingConstraints(testPoints[i]))
                {
                    switch (testPoints[i].ClassificationType)
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
                    switch (testPoints[i].ClassificationType)
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

            Stoper.Stop();
            Statistics.ModelEvaluationTime = Stoper.Elapsed;
            Stoper.Reset();

            return Statistics;
        }

        protected abstract void Evolve(int offspringPopulationSize);
    }
}
