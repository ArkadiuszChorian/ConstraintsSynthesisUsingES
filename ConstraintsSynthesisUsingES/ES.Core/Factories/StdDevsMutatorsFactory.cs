using System;
using ES.Core.Enums;
using ES.Core.Models;
using ES.Core.Mutation;

namespace ES.Core.Factories
{
    public class StdDevsMutatorsFactory : IGenericFactory<MutatorBase>
    {
        public MutatorBase Create(EvolutionParameters evolutionParameters)
        {
            var typeOfMutation = (MutationType)evolutionParameters.TypeOfMutation;

            switch (typeOfMutation)
            {
                case MutationType.UncorrelatedOneStep:
                    return new OsmStdDevsMutator(evolutionParameters);
                case MutationType.UncorrelatedNSteps:
                    return new NsmStdDevsMutator(evolutionParameters);
                case MutationType.Correlated:
                    return new NsmStdDevsMutator(evolutionParameters);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}