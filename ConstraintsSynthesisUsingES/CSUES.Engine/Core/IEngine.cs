using System.Collections.Generic;
using CSUES.Engine.Benchmarks;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
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

        MathModel SynthesizeModel(Point[] trainingPoints);
        Statistics EvaluateModel(Point[] testPoints);
    }
}