using System;
using ES.Core.Factories;
using ES.Core.Models.Solutions;

namespace ES.Core.PopulationGeneration
{
    public class CmPopulationRandomGenerator : NsmPopulationRandomGenerator
    {
        public CmPopulationRandomGenerator(IGenericFactory<Solution> solutionsFactory) : base(solutionsFactory)
        {
        }

        //HACK TODO
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

        //HACK TODO
        //protected override Solution GenerateCoefficients(Solution solution)
        //{
        //    var lenght = solution.ObjectCoefficients.Length;

        //    for (var i = 0; i < lenght; i++)
        //    {
        //        //solution.StdDeviationsCoefficients[i] = RandomGenerator.NextDouble();
        //        solution.StdDeviationsCoefficients[i] = double.Epsilon;
        //    }

        //    lenght = solution.RotationsCoefficients.Length;

        //    for (var i = 0; i < lenght; i++)
        //    {
        //        //solution.RotationsCoefficients[i] = RandomGenerator.NextDoublePositive();
        //        solution.RotationsCoefficients[i] = double.Epsilon;
        //    }

        //    solution.ObjectCoefficients = ObjectCoefficients;

        //    return solution;
        //}
    }
}
