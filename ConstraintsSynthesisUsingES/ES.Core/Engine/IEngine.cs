using System.Collections.Generic;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.Engine
{
    public interface IEngine
    {
        EvolutionParameters Parameters { get; set; }
        Statistics Statistics { get; }
        IList<Solution> EvolutionSteps { get; }

        Solution RunEvolution(IEvaluator evaluator);
    }
}