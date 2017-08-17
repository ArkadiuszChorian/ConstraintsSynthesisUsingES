using ES.Core.Models.Solutions;

namespace ES.Core.MutationSupervison
{
    public class OsmOneFifthRuleSupervisor : MutationRuleSupervisorBase
    {
        public OsmOneFifthRuleSupervisor(int oneFifthRuleCheckInterval, double scalingFactor) : base(oneFifthRuleCheckInterval, scalingFactor)
        {
        }

        protected override void ScaleStandardDeviations(Solution solution, double scalingFactor)
        {
            solution.OneStepStdDeviation *= scalingFactor;
        }
    }
}
