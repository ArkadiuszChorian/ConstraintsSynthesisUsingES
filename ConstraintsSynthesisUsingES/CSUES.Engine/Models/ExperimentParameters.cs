using System;
using System.Collections.Generic;
using ES.Core.Constraints;
using ES.Core.Utils;

namespace ES.Core.Models
{
    public class ExperimentParameters
    {         
        public ExperimentParameters()
        {
            if (Arguments.HasAnyKeys() == false)
                throw new ArgumentException(
                    "There was no command line arguments. Give command line arguments or use constructor with parameters.");
            try
            {
                NumberOfDimensions = Arguments.Get<int>(nameof(NumberOfDimensions));

                NumberOfConstraints = Arguments.Get<int>(nameof(NumberOfConstraints));

                //Optional parameters

                Seed = Arguments.HasKey(nameof(Defaults.Seed)) ? Arguments.Get<int>(nameof(Defaults.Seed)) : Defaults.Seed;

                DomainSamplingStep = Arguments.HasKey(nameof(Defaults.DomainSamplingStep)) ? Arguments.Get<double>(nameof(Defaults.DomainSamplingStep)) : Defaults.DomainSamplingStep;

                BallnBoundaryValue = Arguments.HasKey(nameof(Defaults.BallnBoundaryValue)) ? Arguments.Get<int>(nameof(Defaults.BallnBoundaryValue)) : Defaults.BallnBoundaryValue;
                CubenBoundaryValue = Arguments.HasKey(nameof(Defaults.CubenBoundaryValue)) ? Arguments.Get<int>(nameof(Defaults.CubenBoundaryValue)) : Defaults.CubenBoundaryValue;
                SimplexnBoundaryValue = Arguments.HasKey(nameof(Defaults.SimplexnBoundaryValue)) ? Arguments.Get<int>(nameof(Defaults.SimplexnBoundaryValue)) : Defaults.SimplexnBoundaryValue;

                GlobalLearningRate = Arguments.HasKey(nameof(Defaults.GlobalLerningRate)) ? Arguments.Get<double>(nameof(Defaults.GlobalLerningRate)) : Defaults.GlobalLerningRate(NumberOfDimensions);
                IndividualLearningRate = Arguments.HasKey(nameof(Defaults.IndividualLearningRate)) ? Arguments.Get<double>(nameof(Defaults.IndividualLearningRate)) : Defaults.IndividualLearningRate(NumberOfDimensions);
                StepThreshold = Arguments.HasKey(nameof(Defaults.StepThreshold)) ? Arguments.Get<double>(nameof(Defaults.StepThreshold)) : Defaults.StepThreshold;
                RotationAngle = Arguments.HasKey(nameof(Defaults.RotationAngle)) ? Arguments.Get<double>(nameof(Defaults.RotationAngle)) : Defaults.RotationAngle;
                TypeOfMutation = Arguments.HasKey(nameof(Defaults.TypeOfMutation)) ? Arguments.Get<MutationType>(nameof(Defaults.TypeOfMutation)) : Defaults.TypeOfMutation;

                NumberOfParentsSolutionsToSelect = Arguments.HasKey(nameof(Defaults.NumberOfParentsSolutionsToSelect)) ? Arguments.Get<int>(nameof(Defaults.NumberOfParentsSolutionsToSelect)) : Defaults.NumberOfParentsSolutionsToSelect;
                PartOfSurvivorsSolutionsToSelect = Arguments.HasKey(nameof(Defaults.PartOfSurvivorsSolutionsToSelect)) ? Arguments.Get<double>(nameof(Defaults.PartOfSurvivorsSolutionsToSelect)) : Defaults.PartOfSurvivorsSolutionsToSelect;
                TypeOfSurvivorsSelection = Arguments.HasKey(nameof(Defaults.TypeOfSurvivorsSelection)) ? Arguments.Get<SelectionType>(nameof(Defaults.TypeOfSurvivorsSelection)) : Defaults.TypeOfSurvivorsSelection;

                NumberOfPositivePoints = Arguments.HasKey(nameof(Defaults.NumberOfPositivePoints)) ? Arguments.Get<int>(nameof(Defaults.NumberOfPositivePoints)) : Defaults.NumberOfPositivePoints;
                NumberOfNegativePoints = Arguments.HasKey(nameof(Defaults.NumberOfNegativePoints)) ? Arguments.Get<int>(nameof(Defaults.NumberOfNegativePoints)) : Defaults.NumberOfNegativePoints;
                //DefaultDomainLimit = Tuple.Create(Arguments.HasKey(nameof(Defaults.DefaultDomainLowerLimit)) ? Arguments.Get<double>(nameof(Defaults.DefaultDomainLowerLimit)) : Defaults.DefaultDomainLowerLimit,
                //    Arguments.HasKey(nameof(Defaults.DefaultDomainUpperLimit)) ? Arguments.Get<double>(nameof(Defaults.DefaultDomainUpperLimit)) : Defaults.DefaultDomainUpperLimit);
                DefaultDomainLowerLimit = Arguments.HasKey(nameof(Defaults.DefaultDomainLowerLimit)) ? Arguments.Get<double>(nameof(Defaults.DefaultDomainLowerLimit)) : Defaults.DefaultDomainLowerLimit;
                DefaultDomainUpperLimit = Arguments.HasKey(nameof(Defaults.DefaultDomainUpperLimit)) ? Arguments.Get<double>(nameof(Defaults.DefaultDomainUpperLimit)) : Defaults.DefaultDomainUpperLimit;

                // ReSharper disable once SimplifyConditionalTernaryExpression === Reason: All defaults have to be in Defaults class.
                UsePointsGeneration = Arguments.HasKey(nameof(Defaults.UsePointsGeneration)) ? Arguments.Get<bool>(nameof(Defaults.UsePointsGeneration)) : Defaults.UsePointsGeneration;

                MaxNumberOfPointsInSingleArray = Arguments.HasKey(nameof(Defaults.MaxNumberOfPointsInSingleArray)) ? Arguments.Get<int>(nameof(Defaults.MaxNumberOfPointsInSingleArray)) : Defaults.MaxNumberOfPointsInSingleArray;

                BasePopulationSize = Arguments.HasKey(nameof(Defaults.BasePopulationSize)) ? Arguments.Get<int>(nameof(Defaults.BasePopulationSize)) : Defaults.BasePopulationSize;
                OffspringPopulationSize = Arguments.HasKey(nameof(Defaults.OffspringPopulationSize)) ? Arguments.Get<int>(nameof(Defaults.OffspringPopulationSize)) : Defaults.OffspringPopulationSize;
                NumberOfGenerations = Arguments.HasKey(nameof(Defaults.NumberOfGenerations)) ? Arguments.Get<int>(nameof(Defaults.NumberOfGenerations)) : Defaults.NumberOfGenerations;
                OneFifthRuleCheckInterval = Arguments.HasKey(nameof(Defaults.OneFifthRuleCheckInterval)) ? Arguments.Get<int>(nameof(Defaults.OneFifthRuleCheckInterval)) : Defaults.OneFifthRuleCheckInterval;
                OneFifthRuleScalingFactor = Arguments.HasKey(nameof(Defaults.OneFifthRuleScalingFactor)) ? Arguments.Get<int>(nameof(Defaults.OneFifthRuleScalingFactor)) : Defaults.OneFifthRuleScalingFactor;

                // ReSharper disable once SimplifyConditionalTernaryExpression === Reason: All defaults have to be in Defaults class.
                UseRecombination = Arguments.HasKey(nameof(Defaults.UseRecombination)) ? Arguments.Get<bool>(nameof(Defaults.UseRecombination)) : Defaults.UseRecombination;
                TypeOfObjectsRecombiner = Arguments.HasKey(nameof(Defaults.TypeOfObjectsRecombiner)) ? Arguments.Get<RecombinerType>(nameof(Defaults.TypeOfObjectsRecombiner)) : Defaults.TypeOfObjectsRecombiner;
                TypeOfStdDeviationsRecombiner = Arguments.HasKey(nameof(Defaults.TypeOfStdDeviationsRecombiner)) ? Arguments.Get<RecombinerType>(nameof(Defaults.TypeOfStdDeviationsRecombiner)) : Defaults.TypeOfStdDeviationsRecombiner;
                TypeOfRotationsRecombiner = Arguments.HasKey(nameof(Defaults.TypeOfRotationsRecombiner)) ? Arguments.Get<RecombinerType>(nameof(Defaults.TypeOfRotationsRecombiner)) : Defaults.TypeOfRotationsRecombiner;
                PartOfPopulationToRecombine = Arguments.HasKey(nameof(Defaults.PartOfPopulationToRecombine)) ? Arguments.Get<double>(nameof(Defaults.PartOfPopulationToRecombine)) : Defaults.PartOfPopulationToRecombine;

                ConstraintsToPointsGeneration = Arguments.HasKey(nameof(Defaults.ConstraintsToPointsGeneration)) ? Arguments.GetObject<List<Constraint>>(nameof(Defaults.ConstraintsToPointsGeneration)) : Defaults.ConstraintsToPointsGeneration;
                TypeOfBenchmark = Arguments.HasKey(nameof(Defaults.TypeOfBenchmark)) ? Arguments.Get<BenchmarkType>(nameof(Defaults.TypeOfBenchmark)) : Defaults.TypeOfBenchmark;
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException(
                    "Given command line arguments are incorrect. Give correct command line arguments or use constructor with parameters. You have to give at least NumberOfDimensions and NumberOfConstraints arguments.");
            }
        }

        public ExperimentParameters(
            int numberOfDimensions,

            int numberOfConstraints,          

            int seed = Defaults.Seed,

            double domainSamplingStep = Defaults.DomainSamplingStep,

            double ballnBoundaryValue = Defaults.BallnBoundaryValue,
            double cubenBoundaryValue = Defaults.CubenBoundaryValue,
            double simplexnBoundaryValue = Defaults.SimplexnBoundaryValue,

            double globalLerningRate = double.NaN,
            double individualLearningRate = double.NaN,
            double stepThreshold = Defaults.StepThreshold,
            double rotationAngle = Defaults.RotationAngle,
            MutationType typeOfMutation = Defaults.TypeOfMutation,

            int numberOfParentsSolutionsToSelect = Defaults.NumberOfParentsSolutionsToSelect,
            double partOfSurvivorsSolutionsToSelect = Defaults.PartOfSurvivorsSolutionsToSelect,
            SelectionType typeOfSurvivorsSelection = Defaults.TypeOfSurvivorsSelection,

            int numberOfPositivePoints = Defaults.NumberOfPositivePoints,
            int numberOfNegativePoints = Defaults.NumberOfNegativePoints,
            double defaultDomainLowerLimit = Defaults.DefaultDomainLowerLimit,
            double defaultDomainUpperLimit = Defaults.DefaultDomainUpperLimit,
            bool usePointsGeneration = Defaults.UsePointsGeneration,
            int maxNumberOfPointsInSingleArray = Defaults.MaxNumberOfPointsInSingleArray,

            int basePopulationSize = Defaults.BasePopulationSize,
            int offspringPopulationSize = Defaults.OffspringPopulationSize,
            int numberOfGenerations = Defaults.NumberOfGenerations,
            int oneFifthRuleCheckInterval = Defaults.OneFifthRuleCheckInterval,
            double oneFifthRuleScalingFactor = Defaults.OneFifthRuleScalingFactor,

            bool useRecombination = Defaults.UseRecombination,
            RecombinerType typeOfObjectsRecombiner = Defaults.TypeOfObjectsRecombiner,
            RecombinerType typeOfStdDeviationsRecombiner = Defaults.TypeOfStdDeviationsRecombiner,
            RecombinerType typeOfRotationsRecombiner = Defaults.TypeOfRotationsRecombiner,
            double partOfPopulationToRecombine = Defaults.PartOfPopulationToRecombine,

            BenchmarkType typeOfBenchmark = Defaults.TypeOfBenchmark,
            List<Constraint> constraintsToPointsGeneration = Defaults.ConstraintsToPointsGeneration)            
        {
            NumberOfDimensions = numberOfDimensions;

            NumberOfConstraints = numberOfConstraints;

            DomainSamplingStep = domainSamplingStep;

            Seed = seed;

            BallnBoundaryValue = ballnBoundaryValue;
            CubenBoundaryValue = cubenBoundaryValue;
            SimplexnBoundaryValue = simplexnBoundaryValue;

            GlobalLearningRate = double.IsNaN(globalLerningRate) ? Defaults.GlobalLerningRate(numberOfDimensions) : globalLerningRate;
            IndividualLearningRate = double.IsNaN(individualLearningRate) ? Defaults.IndividualLearningRate(numberOfDimensions) : individualLearningRate;
            StepThreshold = stepThreshold;
            RotationAngle = rotationAngle;
            TypeOfMutation = typeOfMutation;

            NumberOfParentsSolutionsToSelect = numberOfParentsSolutionsToSelect;
            PartOfSurvivorsSolutionsToSelect = partOfSurvivorsSolutionsToSelect;
            TypeOfSurvivorsSelection = typeOfSurvivorsSelection;

            NumberOfPositivePoints = numberOfPositivePoints;
            NumberOfNegativePoints = numberOfNegativePoints;
            //DefaultDomainLimit = Tuple.Create(defaultDomainLowerLimit, defaultDomainUpperLimit);
            DefaultDomainLowerLimit = defaultDomainLowerLimit;
            DefaultDomainUpperLimit = defaultDomainUpperLimit;
            UsePointsGeneration = usePointsGeneration;
            MaxNumberOfPointsInSingleArray = maxNumberOfPointsInSingleArray;

            BasePopulationSize = basePopulationSize;
            OffspringPopulationSize = offspringPopulationSize;
            NumberOfGenerations = numberOfGenerations;
            OneFifthRuleCheckInterval = oneFifthRuleCheckInterval;
            OneFifthRuleScalingFactor = oneFifthRuleScalingFactor;

            UseRecombination = useRecombination;
            TypeOfObjectsRecombiner = typeOfObjectsRecombiner;
            TypeOfStdDeviationsRecombiner = typeOfStdDeviationsRecombiner;
            TypeOfRotationsRecombiner = typeOfRotationsRecombiner;
            PartOfPopulationToRecombine = partOfPopulationToRecombine;

            ConstraintsToPointsGeneration = constraintsToPointsGeneration;
            TypeOfBenchmark = typeOfBenchmark;
        }

        public enum MutationType
        {
            UncorrelatedOneStep,
            UncorrelatedNSteps,
            Correlated
        }
        public enum SelectionType
        {
            Distinct,
            Union
        }

        public enum RecombinerType
        {
            Discrete,
            Intermediate
        }

        public enum BenchmarkType
        {
            Balln,
            Cuben,
            Simplexn,
            Other
        }

        //Global parameters
        public int NumberOfDimensions { get; set; }
        public int Seed { get; set; }

        //Benchmarks
        public double BallnBoundaryValue { get; set; }
        public double CubenBoundaryValue { get; set; }
        public double SimplexnBoundaryValue { get; set; }

        //Problem specific
        public int NumberOfConstraints { get; set; }

        //Sampling
        public double DomainSamplingStep { get; set; }

        //Mutation
        public double GlobalLearningRate { get; set; }
        public double IndividualLearningRate { get; set; }
        public double StepThreshold { get; set; }
        public double RotationAngle { get; set; }
        public MutationType TypeOfMutation { get; set; }

        //Selection
        public int NumberOfParentsSolutionsToSelect { get; set; }
        public double PartOfSurvivorsSolutionsToSelect { get; set; }
        public SelectionType TypeOfSurvivorsSelection { get; set; }

        //Points generation
        public int NumberOfPositivePoints { get; set; }
        public int NumberOfNegativePoints { get; set; }
        //public Tuple<double, double> DefaultDomainLimit { get; set; }
        public double DefaultDomainLowerLimit { get; set; }
        public double DefaultDomainUpperLimit { get; set; }
        public bool UsePointsGeneration { get; set; }
        public int MaxNumberOfPointsInSingleArray { get; set; }

        //Experiment execution
        public int BasePopulationSize { get; set; }
        public int OffspringPopulationSize { get; set; }
        public int NumberOfGenerations { get; set; }
        public int OneFifthRuleCheckInterval { get; set; }
        public double OneFifthRuleScalingFactor { get; set; }

        //Recombination
        public bool UseRecombination { get; set; }
        public RecombinerType TypeOfObjectsRecombiner { get; set; }
        public RecombinerType TypeOfStdDeviationsRecombiner { get; set; }
        public RecombinerType TypeOfRotationsRecombiner { get; set; }
        public double PartOfPopulationToRecombine { get; set; }

        //Constraints to generate points
        public List<Constraint> ConstraintsToPointsGeneration { get; set; }
        public BenchmarkType TypeOfBenchmark { get; set; }
    }
}
