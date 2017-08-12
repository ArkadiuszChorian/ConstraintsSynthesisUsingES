using System.Collections.Generic;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.MutationSupervison
{
    public interface IMutationRuleSupervisor
    {
        void RemeberSolutionParameters(Solution solution);
        void IncrementMutationsNumber();
        void IncrementGenerationNumber();
        void CompareNewSolutionParameters(Solution solution);
        IList<Solution> EnsureRuleFullfillment(IList<Solution> solutions);
    }
}