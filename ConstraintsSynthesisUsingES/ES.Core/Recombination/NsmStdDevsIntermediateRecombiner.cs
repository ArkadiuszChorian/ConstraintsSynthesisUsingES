using ES.Core.Models.Solutions;

namespace ES.Core.Recombination
{
    public class NsmStdDevsIntermediateRecombiner : RecombinerBase
    {
        public override Solution Recombine(Solution[] parents, Solution child)
        {
            var vectorSize = parents[0].StdDeviationsCoefficients.Length;
            var numberOfParents = parents.Length;

            for (var i = 0; i < vectorSize; i++)
            {
                var sum = 0.0;

                for (var j = 0; j < numberOfParents; j++)
                    sum += parents[j].StdDeviationsCoefficients[i];

                child.StdDeviationsCoefficients[i] = sum / numberOfParents;
            }

            return child;
        }
    }
}
