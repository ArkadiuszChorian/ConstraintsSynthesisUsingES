using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Selection
{
    public class ParentsRandomSelector : ParentsSelectorBase
    {
        private readonly MersenneTwister _randomGenerator;

        public ParentsRandomSelector()
        {
            _randomGenerator = MersenneTwister.Instance;         
        }

        public override Solution Select(Solution[] parentSolutions)
        {
            return parentSolutions[_randomGenerator.Next(parentSolutions.Length)].DeepCopyByExpressionTree();
        }
    }
}
