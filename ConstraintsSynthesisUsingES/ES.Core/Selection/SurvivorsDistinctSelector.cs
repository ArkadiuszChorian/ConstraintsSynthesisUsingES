using System;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.Selection
{
    public class SurvivorsDistinctSelector : SurvivorsSelectorBase
    {
        private readonly int _basePopulationSize;

        public SurvivorsDistinctSelector(EvolutionParameters evolutionParameters)
        {
            _basePopulationSize = evolutionParameters.BasePopulationSize;
        }

        public override Solution[] Select(Solution[] parentSolutions, Solution[] offspringSolutions)
        {
            var survivors = new Solution[_basePopulationSize];
            Array.Sort(offspringSolutions);
            Array.Copy(offspringSolutions, survivors, _basePopulationSize);

            return survivors;
        }
    }
}
