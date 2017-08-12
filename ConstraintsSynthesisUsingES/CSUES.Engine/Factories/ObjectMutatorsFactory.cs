using System;
using ES.Core.Enums;
using ES.Core.Models;
using ES.Core.Mutation;

namespace ES.Core.Factories
{
    public class ObjectMutatorsFactory : IGenericFactory<MutatorBase>
    {
        public MutatorBase Create(EvolutionParameters evolutionParameters)
        {
            var typeOfMutation = (MutationType) evolutionParameters.TypeOfMutation;

            switch (typeOfMutation)
            {
                case MutationType.UncorrelatedOneStep:
                    return new OsmObjectMutator();
                case MutationType.UncorrelatedNSteps:
                    return new NsmObjectMutator();
                case MutationType.Correlated:
                    return new CmObjectMutator(evolutionParameters);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}