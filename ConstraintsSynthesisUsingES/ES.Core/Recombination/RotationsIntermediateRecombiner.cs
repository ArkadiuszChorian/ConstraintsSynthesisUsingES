using ES.Core.Models.Solutions;

namespace ES.Core.Recombination
{
    public class RotationsIntermediateRecombiner : RecombinerBase
    {
        public override Solution Recombine(Solution[] parents, Solution child)
        {
            var vectorSize = parents[0].RotationsCoefficients.Length;
            var numberOfParents = parents.Length;

            for (var i = 0; i < vectorSize; i++)
            {
                var sum = 0.0;

                for (var j = 0; j < numberOfParents; j++)
                    sum += parents[j].RotationsCoefficients[i];

                child.RotationsCoefficients[i] = sum / numberOfParents;
            }

            return child;
        }
    }
}
