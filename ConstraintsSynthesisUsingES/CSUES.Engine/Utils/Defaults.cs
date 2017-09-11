using System.Collections.Generic;
using CSUES.Engine.Enums;

namespace CSUES.Engine.Utils
{
    public static class Defaults
    {   
        public static ISet<TermType> AllowedTermsTypes = new HashSet<TermType>
        {
            TermType.Linear
        };

        public const bool UseRedundantConstraintsRemoving = true;
        public const bool UseDataNormalization = true;       
        public const bool AllowQuadraticTerms = false;       
        public const bool UseSeeding = true;       
        
        public const double BallnBoundaryValue = 2.7;
        public const double CubenBoundaryValue = 2.7;
        public const double SimplexnBoundaryValue = 2.7;

        public const long NumberOfDomainSamples = 100000;
        public const int NumberOfTestPoints = 100000;
        public const int NumberOfPositivePoints = 500;
        public const int NumberOfNegativePoints = 500;
        public const double DefaultDomainLowerLimit = -100;
        public const double DefaultDomainUpperLimit = 100;
        public const int MaxNumberOfPointsInSingleArray = 800000;

        public const BenchmarkType TypeOfBenchmark = BenchmarkType.Other;
    }
}
