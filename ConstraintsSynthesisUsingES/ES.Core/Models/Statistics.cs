using System;

namespace ES.Core.Models
{
    public class Statistics
    {
        public double BestFitnessScore { get; set; }
        public double LastFitnessScore { get; set; }
        public int NumberOfGenerationBestSolutionTakenFrom { get; set; }

        public TimeSpan TotalEvolutionTime { get; set; }
        public TimeSpan MeanSingleGenerationEvolutionTime { get; set; }
        //public TimeSpan MeanStdDevsMutationTime { get; set; }
        //public TimeSpan MeanRotationsMutationTime { get; set; }
        //public TimeSpan MeanObjectMutationTime { get; set; }
        //public TimeSpan MeanEvaluationTime { get; set; }
        //public TimeSpan MeanSurvivorsSelectionTime { get; set; }
        public TimeSpan SeedingTime { get; set; }
    }
}