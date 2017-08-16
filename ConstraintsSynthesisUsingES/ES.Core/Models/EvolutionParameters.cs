using ES.Core.Utils;

namespace ES.Core.Models
{
    public class EvolutionParameters
    {
        public EvolutionParameters(
            int objectVectorSize, 
            int basePopulationSize, 
            int offspringPopulationSize, 
            int numberOfGenerations, 
            
            int seed = Defaults.Seed, 
            bool trackEvolutionSteps = Defaults.TrackEvolutionSteps,
            
            int numberOfParentsSolutionsToSelect = Defaults.NumberOfParentsSolutionsToSelect, 
            int typeOfParentsSelection = (int) Defaults.TypeOfParentsSelection, 
            int typeOfSurvivorsSelection = (int) Defaults.TypeOfSurvivorsSelection, 
            
            double globalLearningRate = double.NaN, 
            double individualLearningRate = double.NaN, 
            double stepThreshold = Defaults.StepThreshold, 
            double rotationAngle = Defaults.RotationAngle,            
            int typeOfMutation = (int) Defaults.TypeOfMutation, 
            
            bool useRecombination = Defaults.UseRecombination, 
            int typeOfObjectsRecombination = (int) Defaults.TypeOfObjectsRecombination,
            int typeOfStdDeviationsRecombination = (int) Defaults.TypeOfStdDeviationsRecombination, 
            int typeOfRotationsRecombination = (int) Defaults.TypeOfRotationsRecombination)
        {
            ObjectVectorSize = objectVectorSize;
            BasePopulationSize = basePopulationSize;
            OffspringPopulationSize = offspringPopulationSize;
            NumberOfGenerations = numberOfGenerations;

            Seed = seed;
            TrackEvolutionSteps = trackEvolutionSteps;

            NumberOfParentsSolutionsToSelect = numberOfParentsSolutionsToSelect;
            TypeOfParentsSelection = typeOfParentsSelection;
            TypeOfSurvivorsSelection = typeOfSurvivorsSelection;

            GlobalLearningRate = double.IsNaN(globalLearningRate) ? Defaults.GlobalLerningRate(objectVectorSize) : globalLearningRate;
            IndividualLearningRate = double.IsNaN(individualLearningRate) ? Defaults.IndividualLearningRate(objectVectorSize) : individualLearningRate;
            StepThreshold = stepThreshold;
            RotationAngle = rotationAngle;
            TypeOfMutation = typeOfMutation;

            UseRecombination = useRecombination;
            TypeOfObjectsRecombination = typeOfObjectsRecombination;
            TypeOfStdDeviationsRecombination = typeOfStdDeviationsRecombination;
            TypeOfRotationsRecombination = typeOfRotationsRecombination;
        }

        //Basic evolution parameters         
        public int ObjectVectorSize { get; set; }
        public int BasePopulationSize { get; set; }
        public int OffspringPopulationSize { get; set; }
        public int NumberOfGenerations { get; set; }

        //TODO 
        //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        public int OneFifthRuleCheckInterval { get; set; }
        public double OneFifthRuleScalingFactor { get; set; }

        //Other
        public int Seed { get; set; }
        public bool TrackEvolutionSteps { get; set; }

        //Selection parameters
        public int NumberOfParentsSolutionsToSelect { get; set; }
        public int TypeOfParentsSelection { get; set; }
        public int TypeOfSurvivorsSelection { get; set; }

        //Mutation parameters
        public double GlobalLearningRate { get; set; }
        public double IndividualLearningRate { get; set; }
        public double StepThreshold { get; set; }
        public double RotationAngle { get; set; }
        public int TypeOfMutation { get; set; }

        //Recombination
        public bool UseRecombination { get; set; }
        public int TypeOfObjectsRecombination { get; set; }
        public int TypeOfStdDeviationsRecombination { get; set; }
        public int TypeOfRotationsRecombination { get; set; }
    }
}