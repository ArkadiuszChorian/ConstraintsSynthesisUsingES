using System;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Mutation
{
    public class OsmStdDevsMutator : MutatorBase
    {
        private readonly MersenneTwister _randomGenerator;
        private readonly double _individualLearningRate;
        private readonly double _stepThreshold;

        public OsmStdDevsMutator(EvolutionParameters evolutionParameters)
        {
            _individualLearningRate = evolutionParameters.IndividualLearningRate;
            _stepThreshold = evolutionParameters.StepThreshold;
            _randomGenerator = MersenneTwister.Instance;
        }

        public override Solution Mutate(Solution solution)
        {
            solution.OneStepStdDeviation *= Math.Exp(_individualLearningRate * _randomGenerator.NextDoublePositive());
            solution.OneStepStdDeviation = solution.OneStepStdDeviation < _stepThreshold ? _stepThreshold : solution.OneStepStdDeviation;

            return solution; 
        }
    }
}
