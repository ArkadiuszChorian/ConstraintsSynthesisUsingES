using Accord.Statistics.Distributions.Univariate;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Mutation
{
    public class NsmObjectMutator : MutatorBase
    {
        private readonly MersenneTwister _randomGenerator;
        private readonly NormalDistribution _randomGenerator2 = new NormalDistribution();

        public NsmObjectMutator()
        {
            _randomGenerator = MersenneTwister.Instance;
        }

        public override Solution Mutate(Solution solution)
        {
            var numberOfCoefficients = solution.ObjectCoefficients.Length;

            for (var i = 0; i < numberOfCoefficients; i++)
            {
                //solution.ObjectCoefficients[i] += solution.StdDeviationsCoefficients[i] * _randomGenerator.NextDoublePositive();
                //solution.ObjectCoefficients[i] += solution.StdDeviationsCoefficients[i] * _randomGenerator.NextDouble(-1, 1);
                solution.ObjectCoefficients[i] += solution.StdDeviationsCoefficients[i] * _randomGenerator2.Generate();
            }

            return solution;
        }
    }
}
