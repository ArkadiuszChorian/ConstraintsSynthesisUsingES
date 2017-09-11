using System;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.Selection
{
    public class SurvivorsUnionSelector : SurvivorsSelectorBase
    {
        private readonly int _basePopulationSize;

        public SurvivorsUnionSelector(EvolutionParameters evolutionParameters)
        {
            _basePopulationSize = evolutionParameters.BasePopulationSize;
        }

        public override Solution[] Select(Solution[] parentSolutions, Solution[] offspringSolutions)
        {
            var parentsLength = parentSolutions.Length;
            var offspringLength = offspringSolutions.Length;
            var union = new Solution[parentsLength + offspringLength];
            var survivors = new Solution[_basePopulationSize];

            Array.Copy(parentSolutions, union, parentsLength);
            Array.Copy(offspringSolutions, 0, union, parentsLength, offspringLength);
        
            Array.Sort(union);
            Array.Copy(union, survivors, _basePopulationSize);

            return survivors;
        }
    }
}
