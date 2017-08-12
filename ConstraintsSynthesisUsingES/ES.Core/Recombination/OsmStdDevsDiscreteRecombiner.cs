using ES.Core.Models.Solutions;

namespace ES.Core.Recombination
{
    public class OsmStdDevsDiscreteRecombiner : RecombinerBase
    {
        public override Solution Recombine(Solution[] parents, Solution child)
        {
            child.OneStepStdDeviation = parents[RandomGenerator.Next(parents.Length)].OneStepStdDeviation;

            return child;
        }
    }
}
