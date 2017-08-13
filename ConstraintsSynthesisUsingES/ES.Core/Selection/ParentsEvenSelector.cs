using System.Collections.Generic;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Selection
{
    public class ParentsEvenSelector : ParentsSelectorBase
    {
        private readonly MersenneTwister _randomGenerator;
        private readonly List<int> _parentIndexesReadonly;
        private List<int> _parentIndexes;

        public ParentsEvenSelector(EvolutionParameters evolutionParameters)
        {
            _randomGenerator = MersenneTwister.Instance;

            var basePopulationSize = evolutionParameters.BasePopulationSize;
            _parentIndexesReadonly = new List<int>(basePopulationSize);           

            for (var i = 0; i < basePopulationSize; i++)
                _parentIndexesReadonly.Add(i);

            _parentIndexes = _parentIndexesReadonly.DeepCopyByExpressionTree();
        }

        public override Solution Select(Solution[] parentSolutions)
        {
            var randomIndex = _randomGenerator.Next(_parentIndexes.Count);
            var randomParentIndex = _parentIndexes[randomIndex];
            _parentIndexes.RemoveAt(randomIndex);
            
            if (_parentIndexes.Count == 0)
                _parentIndexes = _parentIndexesReadonly.DeepCopyByExpressionTree();

            return parentSolutions[randomParentIndex].DeepCopyByExpressionTree();
        }
    }
}