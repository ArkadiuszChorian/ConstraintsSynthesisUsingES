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
        public const bool UseDataStandardization = true;

        public const long NumberOfDomainSamples = 100000;

        public const double BallnBoundaryValue = 100;
        public const double CubenBoundaryValue = 100;
        public const double SimplexnBoundaryValue = 100;

        public const int NumberOfPositivePoints = 100;
        public const int NumberOfNegativePoints = 100;
        public const double DefaultDomainLowerLimit = -100;
        public const double DefaultDomainUpperLimit = 100;
        public const int MaxNumberOfPointsInSingleArray = 800000;

        public const BenchmarkType TypeOfBenchmark = BenchmarkType.Other;
    }
}
