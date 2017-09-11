using System.Collections.Generic;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.Engine
{
    public interface IEngine
    {
        EvolutionParameters Parameters { get; set; }
        Statistics Statistics { get; }
        IList<Solution> EvolutionStepsSimple { get; }
        IDictionary<int, EvolutionStep> EvolutionSteps { get; }

        Solution RunEvolution(IEvaluator evaluator, ISeedingProcessor seedingProcessor);
        Solution RunEvolution(IEvaluator evaluator);
    }
}