using CSUES.Engine.Benchmarks;
using CSUES.Engine.Models;
using Statistics = CSUES.Engine.Models.Statistics;

namespace CSUES.Engine.Engine
{
    public interface IEngine
    {
        IBenchmark Benchmark { get; set; }
        ExperimentParameters Parameters { get; set; }      
        Statistics Statistics { get; }
        MathModel MathModel { get; }

        MathModel SynthesizeModel(Point[] trainingPoints);
        Statistics EvaluateModel(Point[] testPoints);
    }
}