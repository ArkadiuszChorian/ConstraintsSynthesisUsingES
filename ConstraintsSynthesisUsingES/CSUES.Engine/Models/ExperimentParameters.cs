using System;
using System.Collections.Generic;
using CSUES.Engine.Enums;
using CSUES.Engine.Models.Constraints;
using ES.Core.Enums;
using ES.Core.Models;
using EvolutionDefaults = ES.Core.Utils.Defaults;
using Defaults = CSUES.Engine.Utils.Defaults;

namespace CSUES.Engine.Models
{
    public class ExperimentParameters
    {                
        public ExperimentParameters(
            int numberOfDimensions,
            
            int basePopulationSize,
            int offspringPopulationSize,
            int numberOfGenerations,
                        
            int seed = EvolutionDefaults.Seed,
            bool trackEvolutionSteps = EvolutionDefaults.TrackEvolutionSteps,
            bool useRedundantConstraintsRemoving = Defaults.UseRedundantConstraintsRemoving,
            bool useDataNormalization = Defaults.UseDataNormalization,
            bool allowQuadraticTerms = Defaults.AllowQuadraticTerms,

            ISet<TermType> allowedTermsTypes = default(ISet<TermType>),
            BenchmarkType typeOfBenchmark = Defaults.TypeOfBenchmark,
            double ballnBoundaryValue = Defaults.BallnBoundaryValue,
            double cubenBoundaryValue = Defaults.CubenBoundaryValue,
            double simplexnBoundaryValue = Defaults.SimplexnBoundaryValue,
            IList<Constraint> referenceConstraints = default(IList<Constraint>),

            long numberOfDomainSamples = Defaults.NumberOfDomainSamples,
            int numberOfTestPoints = Defaults.NumberOfTestPoints,
            int numberOfPositivePoints = Defaults.NumberOfPositivePoints,
            int numberOfNegativePoints = Defaults.NumberOfNegativePoints,
            double defaultDomainLowerLimit = Defaults.DefaultDomainLowerLimit,
            double defaultDomainUpperLimit = Defaults.DefaultDomainUpperLimit,
            int maxNumberOfPointsInSingleArray = Defaults.MaxNumberOfPointsInSingleArray,

            double globalLearningRate = double.NaN,
            double individualLearningRate = double.NaN,
            double stepThreshold = EvolutionDefaults.StepThreshold,
            double rotationAngle = EvolutionDefaults.RotationAngle,
            MutationType typeOfMutation = EvolutionDefaults.TypeOfMutation,

            int numberOfParentsSolutionsToSelect = EvolutionDefaults.NumberOfParentsSolutionsToSelect,
            ParentsSelectionType typeOfParentsSelection = EvolutionDefaults.TypeOfParentsSelection,
            SurvivorsSelectionType typeOfSurvivorsSelection = EvolutionDefaults.TypeOfSurvivorsSelection,
                   
            int oneFifthRuleCheckInterval = EvolutionDefaults.OneFifthRuleCheckInterval,
            double oneFifthRuleScalingFactor = EvolutionDefaults.OneFifthRuleScalingFactor,

            bool useRecombination = EvolutionDefaults.UseRecombination,
            RecombinationType typeOfObjectsRecombination = EvolutionDefaults.TypeOfObjectsRecombination,
            RecombinationType typeOfStdDeviationsRecombination = EvolutionDefaults.TypeOfStdDeviationsRecombination,
            RecombinationType typeOfRotationsRecombination = EvolutionDefaults.TypeOfRotationsRecombination)                       
        {
            if (typeOfBenchmark == BenchmarkType.Other && referenceConstraints == default(IList<Constraint>))
                throw new ArgumentException("In case of choosing BenchmarkType = Other, it is obligatory to provide reference constraints.");

            AllowedTermsTypes = allowedTermsTypes == default(ISet<TermType>)
                ? Defaults.AllowedTermsTypes
                : allowedTermsTypes;

            //HACK
            //AllowedTermsTypes = new HashSet<TermType>
            //{
            //    TermType.Linear,
            //    TermType.Quadratic
            //};

            //NumberOfConstraintsCoefficients = numberOfDimensions * AllowedTermsTypes.Count + 1;

            NumberOfConstraintsCoefficients = allowQuadraticTerms ? numberOfDimensions * 2 + 1 : numberOfDimensions + 1;

            MaximumNumberOfConstraints = typeOfBenchmark == BenchmarkType.Other
                // ReSharper disable once PossibleNullReferenceException : It is checked before
                ? referenceConstraints.Count
                : GetMaximumNumberOfConstraints(numberOfDimensions, typeOfBenchmark, allowQuadraticTerms);

            var objectVectorSize = NumberOfConstraintsCoefficients * MaximumNumberOfConstraints;          

            NumberOfDimensions = numberOfDimensions;        
                  
            EvolutionParameters = new EvolutionParameters(
                objectVectorSize, basePopulationSize, offspringPopulationSize, numberOfGenerations, seed, trackEvolutionSteps,
                oneFifthRuleCheckInterval, oneFifthRuleScalingFactor, numberOfParentsSolutionsToSelect, (int)typeOfParentsSelection, (int)typeOfSurvivorsSelection,
                globalLearningRate, individualLearningRate, stepThreshold, rotationAngle, (int)typeOfMutation,
                useRecombination, (int)typeOfObjectsRecombination, (int)typeOfStdDeviationsRecombination, (int)typeOfRotationsRecombination);
            Seed = seed;
            TrackEvolutionSteps = trackEvolutionSteps;
            UseRedundantConstraintsRemoving = useRedundantConstraintsRemoving;
            UseDataNormalization = useDataNormalization;
            AllowQuadraticTerms = allowQuadraticTerms;

            TypeOfBenchmark = typeOfBenchmark;
            BallnBoundaryValue = ballnBoundaryValue;
            CubenBoundaryValue = cubenBoundaryValue;
            SimplexnBoundaryValue = simplexnBoundaryValue;
            ReferenceConstraints = referenceConstraints;

            NumberOfDomainSamples = numberOfDomainSamples;
            NumberOfTestPoints = numberOfTestPoints;
            NumberOfPositivePoints = numberOfPositivePoints;
            NumberOfNegativePoints = numberOfNegativePoints;
            DefaultDomainLowerLimit = defaultDomainLowerLimit;
            DefaultDomainUpperLimit = defaultDomainUpperLimit;
            MaxNumberOfPointsInSingleArray = maxNumberOfPointsInSingleArray;          
        }        

        //Basic parameters
        public int NumberOfDimensions { get; set; }
        public int MaximumNumberOfConstraints { get; set; }    
        public int NumberOfConstraintsCoefficients { get; set; }    
        public EvolutionParameters EvolutionParameters { get; set; }
        public int Seed { get; set; }
        public bool TrackEvolutionSteps { get; set; }
        public bool UseRedundantConstraintsRemoving { get; set; }
        public bool UseDataNormalization { get; set; }
        public bool AllowQuadraticTerms { get; set; }


        //Benchmark parameters        
        public ISet<TermType> AllowedTermsTypes { get; set; }
        public BenchmarkType TypeOfBenchmark { get; set; }
        public double BallnBoundaryValue { get; set; }
        public double CubenBoundaryValue { get; set; }
        public double SimplexnBoundaryValue { get; set; }
        public IList<Constraint> ReferenceConstraints { get; set; }

        //Points generation
        public long NumberOfDomainSamples { get; set; }
        public int NumberOfTestPoints { get; set; }
        public int NumberOfPositivePoints { get; set; }
        public int NumberOfNegativePoints { get; set; }
        public double DefaultDomainLowerLimit { get; set; }
        public double DefaultDomainUpperLimit { get; set; }
        public int MaxNumberOfPointsInSingleArray { get; set; }

        private static int GetMaximumNumberOfConstraints(int numberOfDimensions, BenchmarkType benchmarkType, bool allowQuadraticTerms)
        {
            switch (benchmarkType)
            {
                case BenchmarkType.Balln:
                    return allowQuadraticTerms ? 1 : numberOfDimensions * numberOfDimensions;
                case BenchmarkType.Cuben:
                    return numberOfDimensions * 2;
                case BenchmarkType.Simplexn:
                    return 4 * numberOfDimensions - 6 + 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(benchmarkType), benchmarkType, null);
            }
        }
    }
}


//public ExperimentParameters()
//{
//    if (Arguments.HasAnyKeys() == false)
//        throw new ArgumentException(
//            "There was no command line arguments. Give command line arguments or use constructor with parameters.");
//    try
//    {
//        NumberOfDimensions = Arguments.Get<int>(nameof(NumberOfDimensions));

//        MaximumNumberOfConstraints = Arguments.Get<int>(nameof(MaximumNumberOfConstraints));

//        //Optional parameters

//        Seed = Arguments.HasKey(nameof(Defaults.Seed)) ? Arguments.Get<int>(nameof(Defaults.Seed)) : Defaults.Seed;

//        DomainSamplingStep = Arguments.HasKey(nameof(Defaults.DomainSamplingStep)) ? Arguments.Get<double>(nameof(Defaults.DomainSamplingStep)) : Defaults.DomainSamplingStep;

//        BallnBoundaryValue = Arguments.HasKey(nameof(Defaults.BallnBoundaryValue)) ? Arguments.Get<int>(nameof(Defaults.BallnBoundaryValue)) : Defaults.BallnBoundaryValue;
//        CubenBoundaryValue = Arguments.HasKey(nameof(Defaults.CubenBoundaryValue)) ? Arguments.Get<int>(nameof(Defaults.CubenBoundaryValue)) : Defaults.CubenBoundaryValue;
//        SimplexnBoundaryValue = Arguments.HasKey(nameof(Defaults.SimplexnBoundaryValue)) ? Arguments.Get<int>(nameof(Defaults.SimplexnBoundaryValue)) : Defaults.SimplexnBoundaryValue;

//        GlobalLearningRate = Arguments.HasKey(nameof(Defaults.GlobalLearningRate)) ? Arguments.Get<double>(nameof(Defaults.GlobalLearningRate)) : Defaults.GlobalLearningRate(NumberOfDimensions);
//        IndividualLearningRate = Arguments.HasKey(nameof(Defaults.IndividualLearningRate)) ? Arguments.Get<double>(nameof(Defaults.IndividualLearningRate)) : Defaults.IndividualLearningRate(NumberOfDimensions);
//        StepThreshold = Arguments.HasKey(nameof(Defaults.StepThreshold)) ? Arguments.Get<double>(nameof(Defaults.StepThreshold)) : Defaults.StepThreshold;
//        RotationAngle = Arguments.HasKey(nameof(Defaults.RotationAngle)) ? Arguments.Get<double>(nameof(Defaults.RotationAngle)) : Defaults.RotationAngle;
//        TypeOfMutation = Arguments.HasKey(nameof(Defaults.TypeOfMutation)) ? Arguments.Get<ExperimentParameters.MutationType>(nameof(Defaults.TypeOfMutation)) : Defaults.TypeOfMutation;

//        NumberOfParentsSolutionsToSelect = Arguments.HasKey(nameof(Defaults.NumberOfParentsSolutionsToSelect)) ? Arguments.Get<int>(nameof(Defaults.NumberOfParentsSolutionsToSelect)) : Defaults.NumberOfParentsSolutionsToSelect;
//        PartOfSurvivorsSolutionsToSelect = Arguments.HasKey(nameof(Defaults.PartOfSurvivorsSolutionsToSelect)) ? Arguments.Get<double>(nameof(Defaults.PartOfSurvivorsSolutionsToSelect)) : Defaults.PartOfSurvivorsSolutionsToSelect;
//        TypeOfSurvivorsSelection = Arguments.HasKey(nameof(Defaults.TypeOfSurvivorsSelection)) ? Arguments.Get<ExperimentParameters.SelectionType>(nameof(Defaults.TypeOfSurvivorsSelection)) : Defaults.TypeOfSurvivorsSelection;

//        NumberOfPositivePoints = Arguments.HasKey(nameof(Defaults.NumberOfPositivePoints)) ? Arguments.Get<int>(nameof(Defaults.NumberOfPositivePoints)) : Defaults.NumberOfPositivePoints;
//        NumberOfNegativePoints = Arguments.HasKey(nameof(Defaults.NumberOfNegativePoints)) ? Arguments.Get<int>(nameof(Defaults.NumberOfNegativePoints)) : Defaults.NumberOfNegativePoints;
//        //DefaultDomainLimit = Tuple.Create(Arguments.HasKey(nameof(Defaults.DefaultDomainLowerLimit)) ? Arguments.Get<double>(nameof(Defaults.DefaultDomainLowerLimit)) : Defaults.DefaultDomainLowerLimit,
//        //    Arguments.HasKey(nameof(Defaults.DefaultDomainUpperLimit)) ? Arguments.Get<double>(nameof(Defaults.DefaultDomainUpperLimit)) : Defaults.DefaultDomainUpperLimit);
//        DefaultDomainLowerLimit = Arguments.HasKey(nameof(Defaults.DefaultDomainLowerLimit)) ? Arguments.Get<double>(nameof(Defaults.DefaultDomainLowerLimit)) : Defaults.DefaultDomainLowerLimit;
//        DefaultDomainUpperLimit = Arguments.HasKey(nameof(Defaults.DefaultDomainUpperLimit)) ? Arguments.Get<double>(nameof(Defaults.DefaultDomainUpperLimit)) : Defaults.DefaultDomainUpperLimit;

//        // ReSharper disable once SimplifyConditionalTernaryExpression === Reason: All defaults have to be in Defaults class.
//        UsePointsGeneration = Arguments.HasKey(nameof(Defaults.UsePointsGeneration)) ? Arguments.Get<bool>(nameof(Defaults.UsePointsGeneration)) : Defaults.UsePointsGeneration;

//        MaxNumberOfPointsInSingleArray = Arguments.HasKey(nameof(Defaults.MaxNumberOfPointsInSingleArray)) ? Arguments.Get<int>(nameof(Defaults.MaxNumberOfPointsInSingleArray)) : Defaults.MaxNumberOfPointsInSingleArray;

//        BasePopulationSize = Arguments.HasKey(nameof(Defaults.BasePopulationSize)) ? Arguments.Get<int>(nameof(Defaults.BasePopulationSize)) : Defaults.BasePopulationSize;
//        OffspringPopulationSize = Arguments.HasKey(nameof(Defaults.OffspringPopulationSize)) ? Arguments.Get<int>(nameof(Defaults.OffspringPopulationSize)) : Defaults.OffspringPopulationSize;
//        NumberOfGenerations = Arguments.HasKey(nameof(Defaults.NumberOfGenerations)) ? Arguments.Get<int>(nameof(Defaults.NumberOfGenerations)) : Defaults.NumberOfGenerations;
//        OneFifthRuleCheckInterval = Arguments.HasKey(nameof(Defaults.OneFifthRuleCheckInterval)) ? Arguments.Get<int>(nameof(Defaults.OneFifthRuleCheckInterval)) : Defaults.OneFifthRuleCheckInterval;
//        OneFifthRuleScalingFactor = Arguments.HasKey(nameof(Defaults.OneFifthRuleScalingFactor)) ? Arguments.Get<int>(nameof(Defaults.OneFifthRuleScalingFactor)) : Defaults.OneFifthRuleScalingFactor;

//        // ReSharper disable once SimplifyConditionalTernaryExpression === Reason: All defaults have to be in Defaults class.
//        UseRecombination = Arguments.HasKey(nameof(Defaults.UseRecombination)) ? Arguments.Get<bool>(nameof(Defaults.UseRecombination)) : Defaults.UseRecombination;
//        TypeOfObjectsRecombiner = Arguments.HasKey(nameof(Defaults.TypeOfObjectsRecombiner)) ? Arguments.Get<ExperimentParameters.RecombinerType>(nameof(Defaults.TypeOfObjectsRecombiner)) : Defaults.TypeOfObjectsRecombiner;
//        TypeOfStdDeviationsRecombiner = Arguments.HasKey(nameof(Defaults.TypeOfStdDeviationsRecombiner)) ? Arguments.Get<ExperimentParameters.RecombinerType>(nameof(Defaults.TypeOfStdDeviationsRecombiner)) : Defaults.TypeOfStdDeviationsRecombiner;
//        TypeOfRotationsRecombiner = Arguments.HasKey(nameof(Defaults.TypeOfRotationsRecombiner)) ? Arguments.Get<ExperimentParameters.RecombinerType>(nameof(Defaults.TypeOfRotationsRecombiner)) : Defaults.TypeOfRotationsRecombiner;
//        PartOfPopulationToRecombine = Arguments.HasKey(nameof(Defaults.PartOfPopulationToRecombine)) ? Arguments.Get<double>(nameof(Defaults.PartOfPopulationToRecombine)) : Defaults.PartOfPopulationToRecombine;

//        ConstraintsToPointsGeneration = Arguments.HasKey(nameof(Defaults.ConstraintsToPointsGeneration)) ? Arguments.GetObject<List<Constraint>>(nameof(Defaults.ConstraintsToPointsGeneration)) : Defaults.ConstraintsToPointsGeneration;
//        TypeOfBenchmark = Arguments.HasKey(nameof(Defaults.TypeOfBenchmark)) ? Arguments.Get<ExperimentParameters.BenchmarkType>(nameof(Defaults.TypeOfBenchmark)) : Defaults.TypeOfBenchmark;
//    }
//    catch (KeyNotFoundException)
//    {
//        throw new ArgumentException(
//            "Given command line arguments are incorrect. Give correct command line arguments or use constructor with parameters. You have to give at least NumberOfDimensions and MaximumNumberOfConstraints arguments.");
//    }
//}
