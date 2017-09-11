using ES.Core.Factories;
using ES.Core.Models.Solutions;

namespace ES.Core.PopulationGeneration
{
    public class NsmPopulationRandomGenerator : OsmPopulationRandomGenerator
    {
        public NsmPopulationRandomGenerator(IGenericFactory<Solution> solutionsFactory) : base(solutionsFactory)
        {
        }

        protected override Solution GenerateCoefficients(Solution solution)
        {
            var lenght = solution.ObjectCoefficients.Length;

            for (var i = 0; i < lenght; i++)
            {
                solution.ObjectCoefficients[i] = RandomGenerator.NextDouble(-100, 100);
                solution.StdDeviationsCoefficients[i] = RandomGenerator.NextDouble();
            }

            return solution;
        }
    }
}
