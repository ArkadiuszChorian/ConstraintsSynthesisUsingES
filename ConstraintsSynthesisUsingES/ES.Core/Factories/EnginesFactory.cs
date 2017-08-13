using System.Diagnostics;
using ES.Core.Engine;
using ES.Core.Enums;
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
        public IEngine Create(EvolutionParameters evolutionParameters, IGenericFactory<Solution> solutionsFactory,
            IGenericFactory<PopulationGeneratorBase> populationGeneratorsFactory, IGenericFactory<MutatorBase> objectMutatorsFactory,
            IGenericFactory<MutatorBase> stdDevsMutatorsFactory, IGenericFactory<ParentsSelectorBase> parentsSelectorsFactory,
            IGenericFactory<SurvivorsSelectorBase> survivorsSelectorsFactory, IGenericFactory<MutatorBase> rotationsMutatorsFactory = null,
            IGenericFactory<RecombinerBase> objectRecombinersFactory = null, IGenericFactory<RecombinerBase> stdDevsRecombinersFactory = null,
            IGenericFactory<RecombinerBase> rotationsRecombinersFactory = null)
        {
            IEngine engine;

            var populationGenerator = populationGeneratorsFactory.Create(evolutionParameters);
            var objectMutator = objectMutatorsFactory.Create(evolutionParameters);
            var stdDevsMutator = stdDevsMutatorsFactory.Create(evolutionParameters);
            var rotationsMutator = rotationsMutatorsFactory?.Create(evolutionParameters);
            var parentsSelector = parentsSelectorsFactory.Create(evolutionParameters);
            var survivorsSelector = survivorsSelectorsFactory.Create(evolutionParameters);
            var objectRecombiner = objectRecombinersFactory?.Create(evolutionParameters);
            var stdDevsRecombiner = stdDevsRecombinersFactory?.Create(evolutionParameters);
            var rotationsRecombiner = rotationsRecombinersFactory?.Create(evolutionParameters);
            var statistics = new Statistics();
            var stoper = new Stopwatch();

            var typeOfMutation = (MutationType) evolutionParameters.TypeOfMutation;

            if (typeOfMutation == MutationType.Correlated)
            {                
                if (evolutionParameters.UseRecombination)
                {
                    engine = new CmEngineWithRecombination(evolutionParameters, solutionsFactory, populationGenerator, objectMutator, stdDevsMutator, parentsSelector, survivorsSelector, statistics, stoper, objectRecombiner, stdDevsRecombiner, rotationsMutator, rotationsRecombiner);
                }
                else
                {
                    engine = new CmEngineWithoutRecombination(evolutionParameters, solutionsFactory, populationGenerator, objectMutator, stdDevsMutator, parentsSelector, survivorsSelector, statistics, stoper, rotationsMutator);
                }
            }
            else
            {
                if (evolutionParameters.UseRecombination)
                {
                    engine = new UmEngineWithRecombination(evolutionParameters, solutionsFactory, populationGenerator, objectMutator, stdDevsMutator, parentsSelector, survivorsSelector, statistics, stoper, objectRecombiner, stdDevsRecombiner);
                }
                else
                {
                    engine = new UmEngineWithoutRecombination(evolutionParameters, solutionsFactory, populationGenerator, objectMutator, stdDevsMutator, parentsSelector, survivorsSelector, statistics, stoper);
                }
            }

            return engine;
        }

        public IEngine Create(EvolutionParameters evolutionParameters)
        {
            IEngine engine;

            IGenericFactory<Solution> solutionsFactory = new SolutionsFactory();
            IGenericFactory<PopulationGeneratorBase> populationGeneratorsFactory = new PopulationGeneratorsFactory(solutionsFactory);
            IGenericFactory<MutatorBase> objectMutatorsFactory = new ObjectMutatorsFactory();
            IGenericFactory<MutatorBase> stdDevsMutatorsFactory = new StdDevsMutatorsFactory();
            IGenericFactory<ParentsSelectorBase> parentsSelectorsFactory = new ParentsSelectorsFactory();
            IGenericFactory<SurvivorsSelectorBase> survivorsSelectorsFactory = new SurvivorsSelectorsFactory();
            
            var typeOfMutation = (MutationType)evolutionParameters.TypeOfMutation;

            if (typeOfMutation == MutationType.Correlated)
            {
                IGenericFactory<MutatorBase> rotationsMutatorsFactory = new RotationsMutatorsFactory();

                if (evolutionParameters.UseRecombination)
                {
                    IGenericFactory<RecombinerBase> objectRecombinersFactory = new ObjectRecombinersFactory();
                    IGenericFactory<RecombinerBase> stdDevsRecombinersFactory = new StdDevsRecombinersFactory();
                    IGenericFactory<RecombinerBase> rotationsRecombinersFactory = new RotationsRecombinersFactory();

                    engine = Create(evolutionParameters, solutionsFactory, populationGeneratorsFactory,
                        objectMutatorsFactory, stdDevsMutatorsFactory, parentsSelectorsFactory, survivorsSelectorsFactory, rotationsMutatorsFactory, objectRecombinersFactory, stdDevsRecombinersFactory, rotationsRecombinersFactory);
                }
                else
                {
                    engine = Create(evolutionParameters, solutionsFactory, populationGeneratorsFactory,
                        objectMutatorsFactory, stdDevsMutatorsFactory, parentsSelectorsFactory, survivorsSelectorsFactory, rotationsMutatorsFactory);
                }
            }
            else
            {
                if (evolutionParameters.UseRecombination)
                {
                    IGenericFactory<RecombinerBase> objectRecombinersFactory = new ObjectRecombinersFactory();
                    IGenericFactory<RecombinerBase> stdDevsRecombinersFactory = new StdDevsRecombinersFactory();

                    engine = Create(evolutionParameters, solutionsFactory, populationGeneratorsFactory,
                        objectMutatorsFactory, stdDevsMutatorsFactory, parentsSelectorsFactory, survivorsSelectorsFactory, null, objectRecombinersFactory, stdDevsRecombinersFactory);
                }
                else
                {
                    engine = Create(evolutionParameters, solutionsFactory, populationGeneratorsFactory,
                        objectMutatorsFactory, stdDevsMutatorsFactory, parentsSelectorsFactory, survivorsSelectorsFactory);
                }
            }

            return engine;
        }
    }
}