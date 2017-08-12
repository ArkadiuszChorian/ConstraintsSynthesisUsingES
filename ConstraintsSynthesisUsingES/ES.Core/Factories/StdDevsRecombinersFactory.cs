using System;
using ES.Core.Enums;
using ES.Core.Models;
using ES.Core.Recombination;

namespace ES.Core.Factories
{
    public class StdDevsRecombinersFactory : IGenericFactory<RecombinerBase>
    {
        public RecombinerBase Create(EvolutionParameters evolutionParameters)
        {
            var typeOfMutation = (MutationType) evolutionParameters.TypeOfMutation;
            var typeOfStdDevsRecombination = (RecombinationType) evolutionParameters.TypeOfStdDeviationsRecombination;

            if (typeOfMutation == MutationType.UncorrelatedOneStep)
            {
                switch (typeOfStdDevsRecombination)
                {
                    case RecombinationType.Discrete:
                        return new OsmStdDevsDiscreteRecombiner();
                    case RecombinationType.Intermediate:
                        return new OsmStdDevsIntermediateRecombiner();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            switch (typeOfStdDevsRecombination)
            {
                case RecombinationType.Discrete:
                    return new NsmStdDevsDiscreteRecombiner();
                case RecombinationType.Intermediate:
                    return new NsmStdDevsIntermediateRecombiner();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}