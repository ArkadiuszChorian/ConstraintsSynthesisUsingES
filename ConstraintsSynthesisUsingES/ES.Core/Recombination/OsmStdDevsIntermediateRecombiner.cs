using ES.Core.Models.Solutions;

namespace ES.Core.Recombination
{
    public class OsmStdDevsIntermediateRecombiner : RecombinerBase
    {
        public override Solution Recombine(Solution[] parents, Solution child)
        {
            var numberOfParents = parents.Length;
            var sum = 0.0;

            for (var j = 0; j < numberOfParents; j++)
                sum += parents[j].OneStepStdDeviation;

            child.OneStepStdDeviation = sum / numberOfParents;           

            return child;
        }
    }
}
