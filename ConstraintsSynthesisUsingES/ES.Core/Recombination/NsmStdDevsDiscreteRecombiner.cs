using ES.Core.Models.Solutions;

namespace ES.Core.Recombination
{
    public class NsmStdDevsDiscreteRecombiner : RecombinerBase
    {
        public override Solution Recombine(Solution[] parents, Solution child)
        {
            var vectorSize = parents[0].StdDeviationsCoefficients.Length;
            var numberOfParents = parents.Length;

            for (var i = 0; i < vectorSize; i++)
                child.StdDeviationsCoefficients[i] = parents[RandomGenerator.Next(numberOfParents)].StdDeviationsCoefficients[i];

            return child;
        }
    }
}
