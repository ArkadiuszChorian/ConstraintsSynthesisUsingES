using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Recombination
{
    public abstract class RecombinerBase
    {
        protected readonly MersenneTwister RandomGenerator;

        protected RecombinerBase()
        {
            RandomGenerator = MersenneTwister.Instance;
        }

        public abstract Solution Recombine(Solution[] parents, Solution child);
    }
}