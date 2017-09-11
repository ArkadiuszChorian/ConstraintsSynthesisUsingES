using System.Collections.Generic;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Models
{
    public class EvolutionStep
    {
        public EvolutionStep(IList<Solution> basePopulation, int offspringPopulationSize)
        {
            BasePopulation = basePopulation.DeepCopyByExpressionTree();
            Mutations = new List<MutationStep>(offspringPopulationSize);
        }

        public IList<Solution> BasePopulation { get; set; }
        public IList<MutationStep> Mutations { get; set; }
        public IList<Solution> NewPopulation { get; set; }
    }
}