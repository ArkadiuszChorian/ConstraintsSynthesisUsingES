using System;
using ES.Core.Enums;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.PopulationGeneration;

namespace ES.Core.Factories
{
    public class PopulationGeneratorsFactory : IGenericFactory<PopulationGeneratorBase>
    {
        private readonly IGenericFactory<Solution> _solutionsFactory;

        public PopulationGeneratorsFactory(IGenericFactory<Solution> solutionsFactory)
        {
            _solutionsFactory = solutionsFactory;
        }

        public PopulationGeneratorBase Create(EvolutionParameters evolutionParameters)
        {
            var typeOfMutation = (MutationType) evolutionParameters.TypeOfMutation;

            switch (typeOfMutation)
            {
                case MutationType.UncorrelatedOneStep:
                    return new OsmPopulationRandomGenerator(_solutionsFactory);
                case MutationType.UncorrelatedNSteps:
                    return new NsmPopulationRandomGenerator(_solutionsFactory);
                case MutationType.Correlated:
                    return new CmPopulationRandomGenerator(_solutionsFactory);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}