using System.Collections.Generic;
using CSUES.Engine.Benchmarks;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using ES.Core.Models;
using Statistics = CSUES.Engine.Models.Statistics;

namespace CSUES.Engine.Core
{
    public interface IEngine
    {
        IBenchmark Benchmark { get; set; }
        ExperimentParameters Parameters { get; set; }      
        Statistics Statistics { get; }
        MathModel MathModel { get; }
        IList<IList<Constraint>> EvolutionSteps { get; }       
        IList<Point> NormalizedTrainingPoints { get; }
        IList<Constraint> NormalizedSynthesizedConstraints { get; }
        IList<IList<Constraint>> NormalizedEvolutionSteps { get; }
        IDictionary<int, EvolutionStep> CoreEvolutionSteps { get; set; }

        MathModel SynthesizeModel(Point[] trainingPoints);
        Statistics EvaluateModel(Point[] testPoints);
    }
}