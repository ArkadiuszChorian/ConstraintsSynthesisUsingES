using System;
using ES.Core.Enums;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.Factories
{
    public class SolutionsFactory : IGenericFactory<Solution>
    {
        public Solution Create(EvolutionParameters evolutionParameters)
        {
            var mutationType = (MutationType) evolutionParameters.TypeOfMutation;
            var objectVectorSize = evolutionParameters.ObjectVectorSize;

            switch (mutationType)
            {
                case MutationType.UncorrelatedOneStep:
                    return new Solution(objectVectorSize);
                case MutationType.UncorrelatedNSteps:
                    return new NsmSolution(objectVectorSize);
                case MutationType.Correlated:
                    return new CmSolution(objectVectorSize);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}