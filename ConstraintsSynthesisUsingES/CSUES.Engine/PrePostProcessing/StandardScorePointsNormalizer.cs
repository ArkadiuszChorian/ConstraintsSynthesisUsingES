using CSUES.Engine.Models;
using CSUES.Engine.Utils;
using ES.Core.Utils;

namespace CSUES.Engine.PrePostProcessing
{
    public class StandardScorePointsNormalizer : IProcessor<Point[]>
    {
        public Point[] ApplyProcessing(Point[] points)
        {
            var normalizedPoints = points.DeepCopyByExpressionTree();
            var means = points.Means();
            var standardDeviations = points.StandardDeviations(means);
            var numberOfDimensions = means.Length;

            foreach (var point in normalizedPoints)
            {
                for (var i = 0; i < numberOfDimensions; i++)
                {
                    point.Coordinates[i] = (point.Coordinates[i] - means[i]) / standardDeviations[i];
                }
            }

            return normalizedPoints;
        }
    }
}