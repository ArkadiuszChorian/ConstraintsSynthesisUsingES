using System;
using System.Collections.Generic;
using ES.Core.Constraints;
using ES.Core.Models;

namespace ES.Core.Utils
{
    public static class Defaults
    {   
        public static double GlobalLerningRate(int numberOfDimensions)
        {
            return 1 / Math.Sqrt(2 * numberOfDimensions);
        }

        public static double IndividualLearningRate(int numberOfDimensions)
        {
            return 1 / Math.Sqrt(2 * Math.Sqrt(numberOfDimensions));
        }

        public const int Seed = 1;

        public const double DomainSamplingStep = 0.1;

        public const double BallnBoundaryValue = 100;
        public const double CubenBoundaryValue = 100;
        public const double SimplexnBoundaryValue = 100;

        public const double StepThreshold = 0.1;
        public const double RotationAngle = 5 * Math.PI / 180;
        public const ExperimentParameters.MutationType TypeOfMutation = ExperimentParameters.MutationType.Correlated;

        public const int NumberOfParentsSolutionsToSelect = 1;
        public const double PartOfSurvivorsSolutionsToSelect = (double)1 / 7;
        public const ExperimentParameters.SelectionType TypeOfSurvivorsSelection = ExperimentParameters.SelectionType.Distinct;

        public const int NumberOfPositivePoints = 100;
        public const int NumberOfNegativePoints = 100;
        public const double DefaultDomainLowerLimit = -100;
        public const double DefaultDomainUpperLimit = 100;
        public const bool UsePointsGeneration = true;
        public const int MaxNumberOfPointsInSingleArray = 800000;

        public const int BasePopulationSize = 15;
        public const int OffspringPopulationSize = 100;
        public const int NumberOfGenerations = 100;
        public const int OneFifthRuleCheckInterval = 5;
        public const double OneFifthRuleScalingFactor = 0.9;

        public const bool UseRecombination = false;
        public const ExperimentParameters.RecombinerType TypeOfObjectsRecombiner = ExperimentParameters.RecombinerType.Discrete;
        public const ExperimentParameters.RecombinerType TypeOfStdDeviationsRecombiner = ExperimentParameters.RecombinerType.Discrete;
        public const ExperimentParameters.RecombinerType TypeOfRotationsRecombiner = ExperimentParameters.RecombinerType.Discrete;
        public const double PartOfPopulationToRecombine = (double)2 / 100;

        public const List<Constraint> ConstraintsToPointsGeneration = default(List<Constraint>);
        public const ExperimentParameters.BenchmarkType TypeOfBenchmark = ExperimentParameters.BenchmarkType.Other;
    }
}
