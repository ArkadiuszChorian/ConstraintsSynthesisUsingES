using ES.Core.Models.Solutions;

namespace ES.Core.Recombination
{
    public class ObjectDiscreteRecombiner : RecombinerBase
    {
        public override Solution Recombine(Solution[] parents, Solution child)
        {
            var vectorSize = parents[0].ObjectCoefficients.Length;
            var numberOfParents = parents.Length;

            for (var i = 0; i < vectorSize; i++)
                child.ObjectCoefficients[i] = parents[RandomGenerator.Next(numberOfParents)].ObjectCoefficients[i];

            return child;
        }
    }
}
