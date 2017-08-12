using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Selection
{
    public class ParentsRandomSelector : ParentsSelectorBase
    {
        private readonly MersenneTwister _randomGenerator;
        private readonly int _numberOfSolutionsToSelect;

        public ParentsRandomSelector(EvolutionParameters evolutionParameters)
        {
            _numberOfSolutionsToSelect = evolutionParameters.NumberOfParentsSolutionsToSelect;
            _randomGenerator = MersenneTwister.Instance;         
        }

        public override Solution[] Select(Solution[] parentSolutions)
        {
            var selectedSolutions = new Solution[_numberOfSolutionsToSelect];

            for (var i = 0; i < _numberOfSolutionsToSelect; i++)
            {
                selectedSolutions[i] = parentSolutions[_randomGenerator.Next(parentSolutions.Length)].DeepCopyByExpressionTree();
            }

            return selectedSolutions;
        }
    }
}
