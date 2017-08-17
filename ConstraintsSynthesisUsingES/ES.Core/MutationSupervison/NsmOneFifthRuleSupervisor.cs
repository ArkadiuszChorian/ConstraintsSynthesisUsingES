using ES.Core.Models.Solutions;

namespace ES.Core.MutationSupervison
{
    public class NsmOneFifthRuleSupervisor : MutationRuleSupervisorBase
    {
        public NsmOneFifthRuleSupervisor(int oneFifthRuleCheckInterval, double scalingFactor) : base(oneFifthRuleCheckInterval, scalingFactor)
        {
        }

        protected override void ScaleStandardDeviations(Solution solution, double scalingFactor)
        {
            var stdDeviationsCoefficientsVectorSize = solution.StdDeviationsCoefficients.Length;

            for (var i = 0; i < stdDeviationsCoefficientsVectorSize; i++)
            {
                solution.StdDeviationsCoefficients[i] *= scalingFactor;
            }
        }
    }
}
