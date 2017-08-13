using System.Diagnostics;
using ES.Core.Factories;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Mutation;
using ES.Core.MutationSupervison;
using ES.Core.PopulationGeneration;
using ES.Core.Recombination;
using ES.Core.Selection;

namespace ES.Core.Engine
{
    public class CmEngineWithRecombination : UmEngineWithRecombination
    {
        protected MutatorBase RotationsMutator;
        protected RecombinerBase RotationsRecombiner;

        public CmEngineWithRecombination(EvolutionParameters evolutionParameters, IGenericFactory<Solution> solutionsFactory, PopulationGeneratorBase populationGenerator, MutatorBase objectMutator, MutatorBase stdDeviationsMutator, ParentsSelectorBase parentsSelector, SurvivorsSelectorBase survivorsSelector, Statistics statistics, Stopwatch stoper, RecombinerBase objectRecombiner, RecombinerBase stdDeviationsRecombiner, MutatorBase rotationsMutator, RecombinerBase rotationsRecombiner) : base(evolutionParameters, solutionsFactory, populationGenerator, objectMutator, stdDeviationsMutator, parentsSelector, survivorsSelector, statistics, stoper, objectRecombiner, stdDeviationsRecombiner)
        {
            RotationsMutator = rotationsMutator;
            RotationsRecombiner = rotationsRecombiner;
        }

        protected override void Evolve(IEvaluator evaluator)
        {
            var offspringPopulationSize = Parameters.OffspringPopulationSize;
            var numberOfParentsSolutionsToSelect = Parameters.NumberOfParentsSolutionsToSelect;

            for (var i = 0; i < offspringPopulationSize; i++)
            {
                var parentsPopulation = new Solution[numberOfParentsSolutionsToSelect];

                for (var j = 0; j < numberOfParentsSolutionsToSelect; j++)
                    parentsPopulation[j] = ParentsSelector.Select(BasePopulation);

                OffspringPopulation[i] = StdDeviationsRecombiner.Recombine(parentsPopulation, OffspringPopulation[i]);
                OffspringPopulation[i] = RotationsRecombiner.Recombine(parentsPopulation, OffspringPopulation[i]);
                OffspringPopulation[i] = ObjectRecombiner.Recombine(parentsPopulation, OffspringPopulation[i]);

                OffspringPopulation[i] = StdDeviationsMutator.Mutate(OffspringPopulation[i]);
                OffspringPopulation[i] = RotationsMutator.Mutate(OffspringPopulation[i]);
                OffspringPopulation[i] = ObjectMutator.Mutate(OffspringPopulation[i]);

                OffspringPopulation[i].FitnessScore = evaluator.Evaluate(OffspringPopulation[i]);
            }

            BasePopulation = SurvivorsSelector.Select(BasePopulation, OffspringPopulation);
        }
    }
}
