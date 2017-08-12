using ES.Core.Models;
using ES.Core.Mutation;

namespace ES.Core.Factories
{
    public class RotationsMutatorsFactory : IGenericFactory<MutatorBase>
    {
        public MutatorBase Create(EvolutionParameters evolutionParameters)
        {
            return new RotationsMutator(evolutionParameters);
        }
    }
}