using System;
using CSUES.Engine.Models;

namespace CSUES.ResultsAnalyzer
{
    public class Statistics : IStatistics
    {
        public double Accuracy { get; set; }
        public ES.Core.Models.Statistics EvolutionStatistics { get; set; }
        public double F1Score { get; set; }
        public double FallOut { get; set; }
        public double FalseDiscoveryRate { get; set; }
        public int FalseNegatives { get; set; }
        public double FalseOmissionRate { get; set; }
        public int FalsePositives { get; set; }
        public double JaccardIndex { get; set; }
        public double MeanAngle { get; set; }
        public double MissRate { get; set; }
        public TimeSpan ModelEvaluationTime { get; set; }
        public double NegativePredictiveValue { get; set; }
        public TimeSpan NegativeTrainingPointsGenerationTime { get; set; }
        public int NumberOfConstraints { get; set; }
        public TimeSpan PositiveTrainingPointsGenerationTime { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public TimeSpan RedundantConstraintsRemovingTime { get; set; }
        public double Specificity { get; set; }
        public TimeSpan TestPointsGenerationTime { get; set; }
        public TimeSpan TotalSynthesisTime { get; set; }
        public int TrueNegatives { get; set; }
        public int TruePositives { get; set; }
    }
}
