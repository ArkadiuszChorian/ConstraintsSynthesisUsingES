using CSUES.Engine.Models;

namespace CSUES.ResultsAnalyzer
{
    public class Experiment
    {
        public ExperimentParameters ExperimentParameters { get; set; }
        public IStatistics Statistics { get; set; }
        public MathModel MathModel { get; set; }
    }
}
