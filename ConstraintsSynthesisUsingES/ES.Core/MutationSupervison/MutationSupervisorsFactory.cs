using System;
using ES.Core.Enums;
using ES.Core.Models;

namespace ES.Core.MutationSupervison
{
    public static class MutationSupervisorsFactory
    {
        public static IMutationRuleSupervisor GetMutationRuleSupervisor(EvolutionParameters evolutionParameters)
        {
            var typeOfMutation = (MutationType) evolutionParameters.TypeOfMutation;

            switch (typeOfMutation)
            {
                case MutationType.UncorrelatedOneStep:
                    return new OsmOneFifthRuleSupervisor();
                //case MutationType.UncorrelatedNSteps:
                //    return new NsmOneFifthRuleSupervisor(evolutionParameters);
                //case MutationType.Correlated:
                //    return new NsmOneFifthRuleSupervisor(evolutionParameters);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
