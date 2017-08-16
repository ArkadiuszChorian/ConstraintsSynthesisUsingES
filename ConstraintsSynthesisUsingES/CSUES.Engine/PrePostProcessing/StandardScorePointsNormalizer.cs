using CSUES.Engine.Models;
using CSUES.Engine.Utils;

namespace CSUES.Engine.PrePostProcessing
{
    public class StandardScorePointsNormalizer : StandardScorePointsNormalizationProcessor
    {
        public override Point[] ApplyProcessing(Point[] points)
        {
            var normalizedPoints = points.DeepCopyByExpressionTree();
            var means = Means(points);
            var standardDeviations = StandardDeviations(points, means);
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