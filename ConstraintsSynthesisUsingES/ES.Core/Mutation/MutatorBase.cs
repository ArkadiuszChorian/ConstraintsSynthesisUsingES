using ES.Core.Models.Solutions;

namespace ES.Core.Mutation
{
    public abstract class MutatorBase
    {
        public abstract Solution Mutate(Solution solution);
    }
}