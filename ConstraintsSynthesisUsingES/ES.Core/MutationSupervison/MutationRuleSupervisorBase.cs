using ES.Core.Models.Solutions;

namespace ES.Core.MutationSupervison
{
    public abstract class MutationRuleSupervisorBase
    {
        private const double OneFifthRatio = 0.2;

        private readonly int _oneFifthRuleCheckInterval;
        private readonly double _scalingFactor;

        private double _savedFitnessScore;
        private int _numberOfSuccessfulMutations;
        private int _numberOfGenerations;       

        protected MutationRuleSupervisorBase(int oneFifthRuleCheckInterval, double scalingFactor)
        {
            _oneFifthRuleCheckInterval = oneFifthRuleCheckInterval;
            _scalingFactor = scalingFactor;
        }
       
        public virtual void SaveBestFitness(Solution solution)
        {
            _savedFitnessScore = solution.FitnessScore;
        }

        public virtual Solution[] EnsureRuleFullfillment(Solution[] solutions)
        {
            _numberOfGenerations++;

            if (solutions[0].FitnessScore > _savedFitnessScore)
            {
                _savedFitnessScore = solutions[0].FitnessScore;
                _numberOfSuccessfulMutations++;               
            }

            if (_numberOfGenerations < _oneFifthRuleCheckInterval)
                return solutions;

            var succesfulMutationsRatio = (double)_numberOfSuccessfulMutations / _numberOfGenerations;
            //Original >
            if (succesfulMutationsRatio > OneFifthRatio)
            {
                var scalingFactor = 1.0 / _scalingFactor;

                foreach (var solution in solutions)
                    ScaleStandardDeviations(solution, scalingFactor);

                NumberOfHigheringScalings++;
            }//Original <
            else if (succesfulMutationsRatio < OneFifthRatio)
            {
                foreach (var solution in solutions)
                    ScaleStandardDeviations(solution, _scalingFactor);

                NumberOfLoweringScalings++;
            }

            _numberOfGenerations = 0;
            _numberOfSuccessfulMutations = 0;

            return solutions;
        }

        protected abstract void ScaleStandardDeviations(Solution solution, double scalingFactor);

        public int NumberOfLoweringScalings { get; set; }
        public int NumberOfHigheringScalings { get; set; }
    }
}