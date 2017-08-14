using System.Collections.Generic;
using CSUES.Engine.Enums;
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
            int maximumNumberOfConstraints,

            int basePopulationSize,
            int offspringPopulationSize,
            int numberOfGenerations,
                       
            int seed = EvolutionDefaults.Seed,
            bool useRedundantConstraintsRemoving = Defaults.UseRedundantConstraintsRemoving,
            bool useDataStandardization = Defaults.UseDataStandardization,

            ISet<TermType> allowedTermsTypes = default(ISet<TermType>),
            BenchmarkType typeOfBenchmark = Defaults.TypeOfBenchmark,
            double ballnBoundaryValue = Defaults.BallnBoundaryValue,
            double cubenBoundaryValue = Defaults.CubenBoundaryValue,
            double simplexnBoundaryValue = Defaults.SimplexnBoundaryValue,
            IEnumerable<Constraint> referenceConstraints = default(IEnumerable<Constraint>),

            long numberOfDomainSamples = Defaults.NumberOfDomainSamples,
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
            AllowedTermsTypes = allowedTermsTypes == default(ISet<TermType>)
                ? Defaults.AllowedTermsTypes
                : allowedTermsTypes;

            NumberOfConstraintsCoefficients = numberOfDimensions * AllowedTermsTypes.Count + 1;

            var objectVectorSize = NumberOfConstraintsCoefficients * maximumNumberOfConstraints;          

            NumberOfDimensions = numberOfDimensions;
            MaximumNumberOfConstraints = maximumNumberOfConstraints;          
            EvolutionParameters = new EvolutionParameters(
                objectVectorSize, basePopulationSize, offspringPopulationSize, numberOfGenerations, seed,
                numberOfParentsSolutionsToSelect, (int)typeOfParentsSelection, (int)typeOfSurvivorsSelection,
                globalLearningRate, individualLearningRate, stepThreshold, rotationAngle, (int)typeOfMutation,
                useRecombination, (int)typeOfObjectsRecombination, (int)typeOfStdDeviationsRecombination, (int)typeOfRotationsRecombination);
            Seed = seed;
            UseRedundantConstraintsRemoving = useRedundantConstraintsRemoving;
            UseDataStandardization = useDataStandardization;

            TypeOfBenchmark = typeOfBenchmark;
            BallnBoundaryValue = ballnBoundaryValue;
            CubenBoundaryValue = cubenBoundaryValue;
            SimplexnBoundaryValue = simplexnBoundaryValue;
            ReferenceConstraints = referenceConstraints;

            NumberOfDomainSamples = numberOfDomainSamples;
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
        public bool UseRedundantConstraintsRemoving { get; set; }
        public bool UseDataStandardization { get; set; }


        //Benchmark parameters
        public ISet<TermType> AllowedTermsTypes { get; set; }
        public BenchmarkType TypeOfBenchmark { get; set; }
        public double BallnBoundaryValue { get; set; }
        public double CubenBoundaryValue { get; set; }
        public double SimplexnBoundaryValue { get; set; }
        public IEnumerable<Constraint> ReferenceConstraints { get; set; }

        //Points generation
        public long NumberOfDomainSamples { get; set; }
        public int NumberOfPositivePoints { get; set; }
        public int NumberOfNegativePoints { get; set; }
        public double DefaultDomainLowerLimit { get; set; }
        public double DefaultDomainUpperLimit { get; set; }
        public int MaxNumberOfPointsInSingleArray { get; set; }
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

//        GlobalLearningRate = Arguments.HasKey(nameof(Defaults.GlobalLerningRate)) ? Arguments.Get<double>(nameof(Defaults.GlobalLerningRate)) : Defaults.GlobalLerningRate(NumberOfDimensions);
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
