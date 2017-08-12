using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Mutation
{
    public class NsmObjectMutator : MutatorBase
    {
        private readonly MersenneTwister _randomGenerator;

        public NsmObjectMutator()
        {
            _randomGenerator = MersenneTwister.Instance;
        }

        public override Solution Mutate(Solution solution)
        {
            var numberOfCoefficients = solution.ObjectCoefficients.Length;

            for (var i = 0; i < numberOfCoefficients; i++)
            {
                solution.ObjectCoefficients[i] += solution.StdDeviationsCoefficients[i] * _randomGenerator.NextDoublePositive();
            }

            return solution;
        }
    }
}
