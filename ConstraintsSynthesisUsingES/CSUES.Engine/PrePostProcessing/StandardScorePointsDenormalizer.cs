using CSUES.Engine.Models;
using CSUES.Engine.Utils;
using ES.Core.Utils;

namespace CSUES.Engine.PrePostProcessing
{
    public class StandardScorePointsDenormalizer : IProcessor<Point[]>
    {
        private readonly double[] _means;
        private readonly double[] _standardDeviations;

        public StandardScorePointsDenormalizer(double[] means, double[] standardDeviations)
        {
            _means = means;
            _standardDeviations = standardDeviations;
        }

        public Point[] ApplyProcessing(Point[] points)
        {
            var denormalizedPoints = points.DeepCopyByExpressionTree();
            var numberOfDimensions = _means.Length;

            foreach (var point in denormalizedPoints)
            {
                for (var i = 0; i < numberOfDimensions; i++)
                {
                    point.Coordinates[i] = point.Coordinates[i] * _standardDeviations[i] + _means[i];
                }
            }

            return denormalizedPoints;
        }
    }
}