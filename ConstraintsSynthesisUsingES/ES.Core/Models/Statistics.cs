using System;

namespace ES.Core.Models
{
    public class Statistics
    {
        public double BestFitnessScore { get; set; }

        public TimeSpan TotalEvolutionTime { get; set; }
        public TimeSpan MeanSingleGenerationEvolutionTime { get; set; }
    }
}