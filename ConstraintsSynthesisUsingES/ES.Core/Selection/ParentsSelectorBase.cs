using ES.Core.Models.Solutions;

namespace ES.Core.Selection
{
    public abstract class ParentsSelectorBase
    {
        public abstract Solution Select(Solution[] parentSolutions);
    }
}