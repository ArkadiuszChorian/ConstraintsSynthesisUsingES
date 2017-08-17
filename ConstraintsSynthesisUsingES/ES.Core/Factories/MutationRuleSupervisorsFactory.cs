using System;
using ES.Core.Enums;
using ES.Core.Models;
using ES.Core.MutationSupervison;

namespace ES.Core.Factories
{
    public class MutationRuleSupervisorsFactory : IGenericFactory<MutationRuleSupervisorBase>
    {
        public MutationRuleSupervisorBase Create(EvolutionParameters evolutionParameters)
        {
            var typeOfMutation = (MutationType)evolutionParameters.TypeOfMutation;

            switch (typeOfMutation)
            {
                case MutationType.UncorrelatedOneStep:
                    return new OsmOneFifthRuleSupervisor(evolutionParameters.OneFifthRuleCheckInterval, evolutionParameters.OneFifthRuleScalingFactor);
                case MutationType.UncorrelatedNSteps:
                    return new NsmOneFifthRuleSupervisor(evolutionParameters.OneFifthRuleCheckInterval, evolutionParameters.OneFifthRuleScalingFactor);
                case MutationType.Correlated:
                    return new NsmOneFifthRuleSupervisor(evolutionParameters.OneFifthRuleCheckInterval, evolutionParameters.OneFifthRuleScalingFactor);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}