using System;
using ES.Core.Enums;

namespace ES.Core.Utils
{
    public static class Defaults
    {   
        public static double GlobalLerningRate(int objectVectorSize)
        {
            return 1 / Math.Sqrt(2 * objectVectorSize);
        }

        public static double IndividualLearningRate(int objectVectorSize)
        {
            return 1 / Math.Sqrt(2 * Math.Sqrt(objectVectorSize));
        }

        public const int Seed = 1;
        public const bool TrackEvolutionSteps = false;

        public const double StepThreshold = 0.1;
        public const double RotationAngle = 5 * Math.PI / 180;
        public const MutationType TypeOfMutation = MutationType.Correlated;

        public const int NumberOfParentsSolutionsToSelect = 1;
        public const ParentsSelectionType TypeOfParentsSelection = ParentsSelectionType.Random;
        public const SurvivorsSelectionType TypeOfSurvivorsSelection = SurvivorsSelectionType.Distinct;
        
        public const int OneFifthRuleCheckInterval = 5;
        public const double OneFifthRuleScalingFactor = 0.9;

        public const bool UseRecombination = false;
        public const RecombinationType TypeOfObjectsRecombination = RecombinationType.Discrete;
        public const RecombinationType TypeOfStdDeviationsRecombination = RecombinationType.Discrete;
        public const RecombinationType TypeOfRotationsRecombination = RecombinationType.Discrete;
    }
}
