using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Mutation
{
    public class OsmObjectMutator : MutatorBase
    {
        private readonly MersenneTwister _randomGenerator;

        public OsmObjectMutator()
        {
            _randomGenerator = MersenneTwister.Instance;
        }

        public override Solution Mutate(Solution solution)
        {
            var numberOfCoefficients = solution.ObjectCoefficients.Length;

            for (var i = 0; i < numberOfCoefficients; i++)
            {
                //solution.ObjectCoefficients[i] += solution.OneStepStdDeviation * _randomGenerator.NextDoublePositive();
                solution.ObjectCoefficients[i] += solution.OneStepStdDeviation * _randomGenerator.NextGaussian();
            }

            return solution;
        }
    }
}
