using System;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Mutation
{
    public class RotationsMutator : MutatorBase
    {
        private readonly MersenneTwister _randomGenerator;
        private readonly double _rotationsAngle;

        public RotationsMutator(EvolutionParameters evolutionParameters)
        {           
            _rotationsAngle = evolutionParameters.RotationAngle;
            _randomGenerator = MersenneTwister.Instance;
        }

        public override Solution Mutate(Solution solution)
        {
            var vectorSize = solution.RotationsCoefficients.Length;

            for (var i = 0; i < vectorSize; i++)
            {
                var mutationValue = _rotationsAngle * _randomGenerator.NextDoublePositive();

                solution.RotationsCoefficients[i] += mutationValue;

                if (!(Math.Abs(solution.RotationsCoefficients[i]) > Math.PI)) continue;

                var reduction = 2 * Math.PI * Math.Sign(solution.RotationsCoefficients[i]);

                solution.RotationsCoefficients[i] -= reduction;
            }

            return solution;
        }
    }
}
