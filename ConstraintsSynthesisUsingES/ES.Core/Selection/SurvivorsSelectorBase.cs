using ES.Core.Models.Solutions;

namespace ES.Core.Selection
{
    public abstract class SurvivorsSelectorBase
    {
        public abstract Solution[] Select(Solution[] parentSolutions, Solution[] offspringSolutions);
    }
}