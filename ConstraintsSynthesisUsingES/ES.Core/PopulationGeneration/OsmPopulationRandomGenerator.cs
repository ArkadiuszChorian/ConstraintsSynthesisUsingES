using ES.Core.Factories;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.PopulationGeneration
{
    public class OsmPopulationRandomGenerator : IPopulationGenerator
    {       
        private readonly IGenericFactory<Solution> _solutionsFactory;
        protected readonly MersenneTwister RandomGenerator;

        public OsmPopulationRandomGenerator(IGenericFactory<Solution> solutionsFactory)
        {            
            _solutionsFactory = solutionsFactory;
            RandomGenerator = MersenneTwister.Instance;
        }

        public Solution[] GeneratePopulation(EvolutionParameters evolutionParameters)
        {
            var basePopulationSize = evolutionParameters.BasePopulationSize;
            var population = new Solution[basePopulationSize];

            for (var i = 0; i < basePopulationSize; i++)
            {
                var solution = _solutionsFactory.Create(evolutionParameters);                
                population[i] = GenerateCoefficients(solution);
            }

            return population;
        }

        protected virtual Solution GenerateCoefficients(Solution solution)
        {
            var lenght = solution.ObjectCoefficients.Length;

            for (var i = 0; i < lenght; i++)
                solution.ObjectCoefficients[i] = RandomGenerator.NextDouble();

            solution.OneStepStdDeviation = RandomGenerator.NextDoublePositive();

            return solution;
        }
    }
}
