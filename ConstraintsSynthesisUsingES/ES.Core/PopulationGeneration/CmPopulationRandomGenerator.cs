using ES.Core.Factories;
using ES.Core.Models.Solutions;

namespace ES.Core.PopulationGeneration
{
    public class CmPopulationRandomGenerator : NsmPopulationRandomGenerator
    {
        public CmPopulationRandomGenerator(IGenericFactory<Solution> solutionsFactory) : base(solutionsFactory)
        {
        }

        protected override Solution GenerateCoefficients(Solution solution)
        {
            var lenght = solution.ObjectCoefficients.Length;

            for (var i = 0; i < lenght; i++)
            {
                solution.ObjectCoefficients[i] = RandomGenerator.NextDouble();
                solution.StdDeviationsCoefficients[i] = RandomGenerator.NextDouble();
            }

            lenght = solution.RotationsCoefficients.Length;

            for (var i = 0; i < lenght; i++)
            {
                solution.RotationsCoefficients[i] = RandomGenerator.NextDoublePositive();
            }

            return solution;
        }
    }
}
