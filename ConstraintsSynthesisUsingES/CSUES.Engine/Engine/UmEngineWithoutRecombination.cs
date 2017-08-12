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

namespace ES.Core.Engine
{
    public class UmEngineWithoutRecombination : EngineBase
    {
        //public UmEngineWithoutRecombination(IBenchmark benchmark, IPopulationGenerator populationGenerator, IEvaluator evaluator, ILogger logger, IMutator objectMutator, IMutator stdDeviationsMutator, IMutationRuleSupervisor mutationRuleSupervisor, IParentsSelector parentsParentsSelector, ISurvivorsSelector survivorsSelector, IPointsGenerator positivePointsGenerator, IPointsGenerator negativePointsGenerator, ExperimentParameters experimentParameters, Solution[] basePopulation, Solution[] offspringPopulation) : base(benchmark, populationGenerator, evaluator, logger, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsParentsSelector, survivorsSelector, positivePointsGenerator, negativePointsGenerator, experimentParameters, basePopulation, offspringPopulation)
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

        //protected IProcessor<Constraint[]> RedundantConstriantsRemover;

        public UmEngineWithoutRecombination(IBenchmark benchmark, IPopulationGenerator populationGenerator, IEvaluator evaluator, ILogger logger, IMutator objectMutator, IMutator stdDeviationsMutator, IMutationRuleSupervisor mutationRuleSupervisor, IParentsSelector parentsParentsSelector, ISurvivorsSelector survivorsSelector, IProcessor<Constraint[]> redundantConstrainsRemover, ExperimentParameters experimentParameters, Statistics statistics, Solution[] basePopulation, Solution[] offspringPopulation) : base(benchmark, populationGenerator, evaluator, logger, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsParentsSelector, survivorsSelector, redundantConstrainsRemover, experimentParameters, statistics, basePopulation, offspringPopulation)
        {
            //Benchmark = benchmark;
            //PopulationGenerator = populationGenerator;
            //Evaluator = evaluator;
            //Logger = logger;
            //ObjectMutator = objectMutator;
            //StdDeviationsMutator = stdDeviationsMutator;
            //MutationRuleSupervisor = mutationRuleSupervisor;
            //ParentsSelector = parentsParentsSelector;
            //SurvivorsSelector = survivorsSelector;
            //RedundantConstriantsRemover = redundantConstrainsRemover;
            //ExperimentParameters = experimentParameters;
            //BasePopulation = basePopulation;
            //OffspringPopulation = offspringPopulation;
        }

        //public IBenchmark Benchmark { get; set; }
        //public IPopulationGenerator PopulationGenerator { get; set; }
        //public IEvaluator Evaluator { get; set; }
        //public ILogger Logger { get; set; }
        //public IMutator ObjectMutator { get; set; }
        //public IMutator StdDeviationsMutator { get; set; }
        //public IMutationRuleSupervisor MutationRuleSupervisor { get; set; }
        //public IParentsSelector ParentsSelector { get; set; }
        //public ISurvivorsSelector SurvivorsSelector { get; set; }
        //public IPointsGenerator PositivePointsGenerator { get; set; }
        //public IPointsGenerator NegativePointsGenerator { get; set; }
        //public ExperimentParameters ExperimentParameters { get; set; }
        //public Solution[] BasePopulation { get; set; }
        //public Solution[] OffspringPopulation { get; set; }
        //public Solution[] InitialPopulation { get; set; }

        //public override void SynthesizeModel(Point[] trainingPoints)
        //{
        //    var offspringPopulationSize = ExperimentParameters.OffspringPopulationSize;
        //    var numberOfGenerations = ExperimentParameters.NumberOfGenerations;

        //    BasePopulation = PopulationGenerator.GeneratePopulation(ExperimentParameters);

        //    for (var i = 0; i < offspringPopulationSize; i++)
        //        OffspringPopulation[i] = new Solution(ExperimentParameters);

        //    InitialPopulation = BasePopulation.DeepCopyByExpressionTree();

        //    for (var i = 0; i < numberOfGenerations; i++)
        //    {
        //        for (var j = 0; j < offspringPopulationSize; j++)
        //        {
        //            OffspringPopulation[j] = ParentsSelector.Select(BasePopulation)[0];

        //            OffspringPopulation[j] = StdDeviationsMutator.Mutate(OffspringPopulation[j]);
        //            OffspringPopulation[j] = ObjectMutator.Mutate(OffspringPopulation[j]);

        //            OffspringPopulation[j].FitnessScore = Evaluator.Evaluate(OffspringPopulation[j]);
        //        }
        //        BasePopulation = SurvivorsSelector.Select(BasePopulation, OffspringPopulation);
        //    }
        //}        

        protected override void Evolve(int offspringPopulationSize)
        {
            for (var j = 0; j < offspringPopulationSize; j++)
            {
                OffspringPopulation[j] = ParentsSelector.Select(BasePopulation)[0];

                OffspringPopulation[j] = StdDeviationsMutator.Mutate(OffspringPopulation[j]);
                OffspringPopulation[j] = ObjectMutator.Mutate(OffspringPopulation[j]);

                OffspringPopulation[j].FitnessScore = Evaluator.Evaluate(OffspringPopulation[j]);
            }
            BasePopulation = SurvivorsSelector.Select(BasePopulation, OffspringPopulation);
        }
    }
}
