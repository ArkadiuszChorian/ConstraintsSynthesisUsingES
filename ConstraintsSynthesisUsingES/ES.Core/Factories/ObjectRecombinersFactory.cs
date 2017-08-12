using System;
using ES.Core.Enums;
using ES.Core.Models;
using ES.Core.Recombination;

namespace ES.Core.Factories
{
    public class ObjectRecombinersFactory : IGenericFactory<RecombinerBase>
    {
        public RecombinerBase Create(EvolutionParameters evolutionParameters)
        {
            var typeOfObjectsRecombination = (RecombinationType) evolutionParameters.TypeOfObjectsRecombination;

            switch (typeOfObjectsRecombination)
            {
                case RecombinationType.Discrete:
                    return new ObjectDiscreteRecombiner();
                case RecombinationType.Intermediate:
                    return new ObjectIntermediateRecombiner();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}