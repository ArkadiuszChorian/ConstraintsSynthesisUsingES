using System;
using Accord.Statistics.Distributions.Univariate;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Mutation
{
    public class NsmStdDevsMutator : MutatorBase
    {
        private readonly MersenneTwister _randomGenerator;
        private readonly double _globalLearningRate;
        private readonly double _individualLearningRate;
        private readonly double _stepThreshold;
        private readonly NormalDistribution _randomGenerator2 = new NormalDistribution();

        public NsmStdDevsMutator(EvolutionParameters evolutionParameters)
        {
            _globalLearningRate = evolutionParameters.GlobalLearningRate;
            _individualLearningRate = evolutionParameters.IndividualLearningRate;
            _stepThreshold = evolutionParameters.StepThreshold;
            _randomGenerator = MersenneTwister.Instance;
        }

        public override Solution Mutate(Solution solution)
        {
            var numberOfCoefficients = solution.StdDeviationsCoefficients.Length;

            for (var j = 0; j < numberOfCoefficients; j++)
            {
                //solution.StdDeviationsCoefficients[j] *= Math.Exp(_individualLearningRate * _randomGenerator.NextDoublePositive() + _globalLearningRate * _randomGenerator.NextDoublePositive());
                //solution.StdDeviationsCoefficients[j] *= Math.Exp(_individualLearningRate * _randomGenerator.NextDouble(-1, 1) + _globalLearningRate * _randomGenerator.NextDouble(-1, 1));
                solution.StdDeviationsCoefficients[j] *= Math.Exp(_individualLearningRate * _randomGenerator2.Generate() + _globalLearningRate * _randomGenerator2.Generate());
                solution.StdDeviationsCoefficients[j] = solution.StdDeviationsCoefficients[j] < _stepThreshold ? _stepThreshold : solution.StdDeviationsCoefficients[j];
            }

            return solution;
        }
    }
}
