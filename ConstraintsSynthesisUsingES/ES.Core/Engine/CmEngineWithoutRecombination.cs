using System.Diagnostics;
using ES.Core.Factories;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Mutation;
using ES.Core.MutationSupervison;
using ES.Core.PopulationGeneration;
using ES.Core.Selection;

namespace ES.Core.Engine
{
    public class CmEngineWithoutRecombination : UmEngineWithoutRecombination
    {
        protected MutatorBase RotationsMutator;

        public CmEngineWithoutRecombination(EvolutionParameters evolutionParameters, IGenericFactory<Solution> solutionsFactory, IPopulationGenerator populationGenerator, MutatorBase objectMutator, MutatorBase stdDeviationsMutator, ParentsSelectorBase parentsSelector, SurvivorsSelectorBase survivorsSelector, Statistics statistics, Stopwatch stoper, MutatorBase rotationsMutator) : base(evolutionParameters, solutionsFactory, populationGenerator, objectMutator, stdDeviationsMutator, parentsSelector, survivorsSelector, statistics, stoper)
        {
            RotationsMutator = rotationsMutator;
        }

        protected override void Evolve(IEvaluator evaluator)
        {
            var offspringPopulationSize = Parameters.OffspringPopulationSize;

            for (var i = 0; i < offspringPopulationSize; i++)
            {
                OffspringPopulation[i] = ParentsSelector.Select(BasePopulation);

                OffspringPopulation[i] = StdDeviationsMutator.Mutate(OffspringPopulation[i]);
                OffspringPopulation[i] = RotationsMutator.Mutate(OffspringPopulation[i]);
                OffspringPopulation[i] = ObjectMutator.Mutate(OffspringPopulation[i]);

                OffspringPopulation[i].FitnessScore = evaluator.Evaluate(OffspringPopulation[i]);
            }

            BasePopulation = SurvivorsSelector.Select(BasePopulation, OffspringPopulation);
        }
    }
}
