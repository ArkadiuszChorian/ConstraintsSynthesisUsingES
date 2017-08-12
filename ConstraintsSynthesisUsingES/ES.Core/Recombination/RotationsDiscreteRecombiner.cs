using ES.Core.Models.Solutions;

namespace ES.Core.Recombination
{
    public class RotationsDiscreteRecombiner : RecombinerBase
    {
        public override Solution Recombine(Solution[] parents, Solution child)
        {
            var vectorSize = parents[0].RotationsCoefficients.Length;
            var numberOfParents = parents.Length;

            for (var i = 0; i < vectorSize; i++)
                child.RotationsCoefficients[i] = parents[RandomGenerator.Next(numberOfParents)].RotationsCoefficients[i];

            return child;
        }
    }
}
