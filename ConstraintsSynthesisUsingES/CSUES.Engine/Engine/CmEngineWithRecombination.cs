using ES.Core.Benchmarks;
using ES.Core.Constraints;
using ES.Core.Evaluation;
using ES.Core.Logging;
using ES.Core.Models;
using ES.Core.Mutation;
using ES.Core.MutationSupervison;
using ES.Core.PopulationGeneration;
using ES.Core.PrePostProcessing;
using ES.Core.Recombination;
using ES.Core.Selection;

namespace ES.Core.Engine
{
    public class CmEngineWithRecombination : UmEngineWithRecombination
    {
        //public CmEngineWithRecombination(IBenchmark benchmark, IPopulationGenerator populationGenerator, IEvaluator evaluator, ILogger logger, IMutator objectMutator, IMutator stdDeviationsMutator, IMutationRuleSupervisor mutationRuleSupervisor, IParentsSelector parentsParentsSelector, ISurvivorsSelector survivorsSelector, IPointsGenerator positivePointsGenerator, IPointsGenerator negativePointsGenerator, ExperimentParameters experimentParameters, Solution[] basePopulation, Solution[] offspringPopulation, IRecombiner objectRecombiner, IRecombiner stdDeviationsRecombiner, IMutator rotationsMutator, IRecombiner rotationsRecombiner) : base(benchmark, populationGenerator, evaluator, logger, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsParentsSelector, survivorsSelector, positivePointsGenerator, negativePointsGenerator, experimentParameters, basePopulation, offspringPopulation, objectRecombiner, stdDeviationsRecombiner)
        //{
        //    RotationsMutator = rotationsMutator;
        //    RotationsRecombiner = rotationsRecombiner;
        //}

        public CmEngineWithRecombination(IBenchmark benchmark, IPopulationGenerator populationGenerator, IEvaluator evaluator, ILogger logger, IMutator objectMutator, IMutator stdDeviationsMutator, IMutationRuleSupervisor mutationRuleSupervisor, IParentsSelector parentsParentsSelector, ISurvivorsSelector survivorsSelector, IProcessor<Constraint[]> redundantConstrainsRemover, ExperimentParameters experimentParameters, Statistics statistics, Solution[] basePopulation, Solution[] offspringPopulation, IRecombiner objectRecombiner, IRecombiner stdDeviationsRecombiner, IMutator rotationsMutator, IRecombiner rotationsRecombiner) : base(benchmark, populationGenerator, evaluator, logger, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsParentsSelector, survivorsSelector, redundantConstrainsRemover, experimentParameters, statistics, basePopulation, offspringPopulation, objectRecombiner, stdDeviationsRecombiner)
        {
            RotationsMutator = rotationsMutator;
            RotationsRecombiner = rotationsRecombiner;
        }

        protected IMutator RotationsMutator;
        protected IRecombiner RotationsRecombiner;

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
        //            var parentsPopulation = ParentsSelector.Select(BasePopulation);

        //            OffspringPopulation[j] = StdDeviationsRecombiner.Recombine(parentsPopulation, OffspringPopulation[j]);
        //            OffspringPopulation[j] = RotationsRecombiner.Recombine(parentsPopulation, OffspringPopulation[j]);
        //            OffspringPopulation[j] = ObjectRecombiner.Recombine(parentsPopulation, OffspringPopulation[j]);

        //            OffspringPopulation[j] = StdDeviationsMutator.Mutate(OffspringPopulation[j]);
        //            OffspringPopulation[j] = RotationsMutator.Mutate(OffspringPopulation[j]);
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
                var parentsPopulation = ParentsSelector.Select(BasePopulation);

                OffspringPopulation[j] = StdDeviationsRecombiner.Recombine(parentsPopulation, OffspringPopulation[j]);
                OffspringPopulation[j] = RotationsRecombiner.Recombine(parentsPopulation, OffspringPopulation[j]);
                OffspringPopulation[j] = ObjectRecombiner.Recombine(parentsPopulation, OffspringPopulation[j]);

                OffspringPopulation[j] = StdDeviationsMutator.Mutate(OffspringPopulation[j]);
                OffspringPopulation[j] = RotationsMutator.Mutate(OffspringPopulation[j]);
                OffspringPopulation[j] = ObjectMutator.Mutate(OffspringPopulation[j]);

                OffspringPopulation[j].FitnessScore = Evaluator.Evaluate(OffspringPopulation[j]);
            }
            BasePopulation = SurvivorsSelector.Select(BasePopulation, OffspringPopulation);
        }
    }
}
