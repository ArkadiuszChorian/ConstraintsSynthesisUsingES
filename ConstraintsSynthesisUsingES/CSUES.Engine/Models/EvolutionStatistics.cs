using System;

namespace ES.Core.Models
{
    public class EvolutionStatistics
    {
        public TimeSpan TotalEvolutionTime { get; set; }
        public TimeSpan MeanSingleGenerationEvolutionTime { get; set; }
    }
}