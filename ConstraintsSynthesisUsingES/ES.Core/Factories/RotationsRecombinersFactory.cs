using System;
using ES.Core.Enums;
using ES.Core.Models;
using ES.Core.Recombination;

namespace ES.Core.Factories
{
    public class RotationsRecombinersFactory : IGenericFactory<RecombinerBase>
    {
        public RecombinerBase Create(EvolutionParameters evolutionParameters)
        {
            var typeOfRotationsRecombination = (RecombinationType) evolutionParameters.TypeOfRotationsRecombination;

            switch (typeOfRotationsRecombination)
            {
                case RecombinationType.Discrete:
                    return new RotationsDiscreteRecombiner();
                case RecombinationType.Intermediate:
                    return new RotationsIntermediateRecombiner();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}