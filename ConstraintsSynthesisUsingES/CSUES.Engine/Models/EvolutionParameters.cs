namespace ES.Core.Models
{
    public class EvolutionParameters
    {
        //Basic evolution parameters
        public int Seed { get; set; }
        public int ObjectVectorSize { get; set; }
        public int BasePopulationSize { get; set; }
        public int OffspringPopulationSize { get; set; }
        public int NumberOfGenerations { get; set; }
        
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
        public double PartOfPopulationToRecombine { get; set; }
    }
}