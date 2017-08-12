using System.Diagnostics;
using ES.Core.Engine;
using ES.Core.Enums;
using ES.Core.Factories.Interfaces;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Mutation;
using ES.Core.PopulationGeneration;
using ES.Core.Recombination;
using ES.Core.Selection;

namespace ES.Core.Factories
{
    public class EnginesFactory : IEnginesFactory
    {
        public EngineBase Create(EvolutionParameters evolutionParameters, IGenericFactory<Solution> solutionsFactory,
            IGenericFactory<IPopulationGenerator> populationGeneratorsFactory, IGenericFactory<MutatorBase> objectMutatorsFactory,
            IGenericFactory<MutatorBase> stdDevsMutatorsFactory, IGenericFactory<ParentsSelectorBase> parentsSelectorsFactory,
            IGenericFactory<SurvivorsSelectorBase> survivorsSelectorsFactory, IGenericFactory<MutatorBase> rotationsMutatorsFactory = null,
            IGenericFactory<RecombinerBase> objectRecombinersFactory = null, IGenericFactory<RecombinerBase> stdDevsRecombinersFactory = null,
            IGenericFactory<RecombinerBase> rotationsRecombinersFactory = null)
        {
            EngineBase engine;

            var populationGenerator = populationGeneratorsFactory.Create(evolutionParameters);
            var objectMutator = objectMutatorsFactory.Create(evolutionParameters);
            var stdDevsMutator = stdDevsMutatorsFactory.Create(evolutionParameters);
            var rotationsMutator = rotationsMutatorsFactory?.Create(evolutionParameters);
            var parentsSelector = parentsSelectorsFactory.Create(evolutionParameters);
            var survivorsSelector = survivorsSelectorsFactory.Create(evolutionParameters);
            var objectRecombiner = objectRecombinersFactory?.Create(evolutionParameters);
            var stdDevsRecombiner = stdDevsRecombinersFactory?.Create(evolutionParameters);
            var rotationsRecombiner = rotationsRecombinersFactory?.Create(evolutionParameters);
            var evolutionStatistics = new EvolutionStatistics();
            var stoper = new Stopwatch();

            var typeOfMutation = (MutationType) evolutionParameters.TypeOfMutation;

            if (typeOfMutation == MutationType.Correlated)
            {                
                if (evolutionParameters.UseRecombination)
                {
                    engine = new CmEngineWithRecombination(evolutionParameters, solutionsFactory, populationGenerator, objectMutator, stdDevsMutator, parentsSelector, survivorsSelector, evolutionStatistics, stoper, objectRecombiner, stdDevsRecombiner, rotationsMutator, rotationsRecombiner);
                }
                else
                {
                    engine = new CmEngineWithoutRecombination(evolutionParameters, solutionsFactory, populationGenerator, objectMutator, stdDevsMutator, parentsSelector, survivorsSelector, evolutionStatistics, stoper, rotationsMutator);
                }
            }
            else
            {
                if (evolutionParameters.UseRecombination)
                {
                    engine = new UmEngineWithRecombination(evolutionParameters, solutionsFactory, populationGenerator, objectMutator, stdDevsMutator, parentsSelector, survivorsSelector, evolutionStatistics, stoper, objectRecombiner, stdDevsRecombiner);
                }
                else
                {
                    engine = new UmEngineWithoutRecombination(evolutionParameters, solutionsFactory, populationGenerator, objectMutator, stdDevsMutator, parentsSelector, survivorsSelector, evolutionStatistics, stoper);
                }
            }

            return engine;
        }
    }
}