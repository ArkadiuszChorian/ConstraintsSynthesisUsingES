using System.Collections.Generic;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.MutationSupervison
{
    public class OsmOneFifthRuleSupervisor : IMutationRuleSupervisor
    {
        private const double OneFifthRatio = 0.2;

        public int SuccesfulMutationsNumber { get; set; }
        public int MutationsNumber { get; set; }
        public int StdDeviationsScalingFactor { get; set; }

        public IList<Solution> EnsureRuleFullfillment(IList<Solution> solutions)
        {
            var succesfulMutationsRatio = (double)SuccesfulMutationsNumber / MutationsNumber;

            if (succesfulMutationsRatio > OneFifthRatio)
            {
                foreach (var solution in solutions)
                {
                    solution.OneStepStdDeviation /= StdDeviationsScalingFactor;
                }
            }

            if (succesfulMutationsRatio < OneFifthRatio)
            {
                foreach (var solution in solutions)
                {
                    solution.OneStepStdDeviation *= StdDeviationsScalingFactor;
                }
            }

            return solutions;
        }

        public void RemeberSolutionParameters(Solution solution)
        {
            throw new System.NotImplementedException();
        }

        public void IncrementMutationsNumber()
        {
            throw new System.NotImplementedException();
        }

        public void IncrementGenerationNumber()
        {
            throw new System.NotImplementedException();
        }

        public void CompareNewSolutionParameters(Solution solution)
        {
            throw new System.NotImplementedException();
        }
    }
}
