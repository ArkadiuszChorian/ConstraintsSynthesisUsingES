using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.PopulationGeneration
{
    public interface IPopulationGenerator
    {
        Solution[] GeneratePopulation(EvolutionParameters evolutionParameters);
    }
}
