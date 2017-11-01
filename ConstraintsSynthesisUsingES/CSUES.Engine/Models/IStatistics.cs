using System;
using ES.Core.Models;

namespace CSUES.Engine.Models
{
    public interface IStatistics
    {
        double Accuracy { get; }
        ES.Core.Models.Statistics EvolutionStatistics { get; set; }
        double F1Score { get; }
        double FallOut { get; }
        double FalseDiscoveryRate { get; }
        int FalseNegatives { get; set; }
        double FalseOmissionRate { get; }
        int FalsePositives { get; set; }
        double JaccardIndex { get; }
        double MeanAngle { get; set; }
        double MissRate { get; }
        TimeSpan ModelEvaluationTime { get; set; }
        double NegativePredictiveValue { get; }
        TimeSpan NegativeTrainingPointsGenerationTime { get; set; }
        int NumberOfConstraints { get; set; }
        TimeSpan PositiveTrainingPointsGenerationTime { get; set; }
        double Precision { get; }
        double Recall { get; }
        TimeSpan RedundantConstraintsRemovingTime { get; set; }
        double Specificity { get; }
        TimeSpan TestPointsGenerationTime { get; set; }
        TimeSpan TotalSynthesisTime { get; set; }
        int TrueNegatives { get; set; }
        int TruePositives { get; set; }
    }
}