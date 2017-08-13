using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.PopulationGeneration
{
    public abstract class PopulationGeneratorBase
    {
        public abstract Solution[] GeneratePopulation(EvolutionParameters evolutionParameters);
    }
}
