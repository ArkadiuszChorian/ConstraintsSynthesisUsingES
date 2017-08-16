using System;
using System.Linq;
using CSUES.Engine.Models;

namespace CSUES.Engine.PrePostProcessing
{
    public abstract class StandardScorePointsNormalizationProcessor : IProcessor<Point[]>
    {
        public abstract Point[] ApplyProcessing(Point[] points);

        public virtual double[] Means(Point[] points)
        {
            var numberOfDimensions = points.First().Coordinates.Length;
            var numberOfPoints = points.Length;
            var means = new double[numberOfDimensions];

            foreach (var point in points)
            {
                for (var i = 0; i < numberOfDimensions; i++)
                    means[i] += point.Coordinates[i] / numberOfPoints;
            }

            return means;
        }

        public virtual double[] StandardDeviations(Point[] points, double[] means)
        {
            var numberOfDimensions = means.Length;
            var variances = Variances(points, means);
            var standardDeviations = new double[numberOfDimensions];

            for (var i = 0; i < numberOfDimensions; i++)
                standardDeviations[i] = Math.Sqrt(variances[i]);

            return standardDeviations;
        }

        public virtual double[] StandardDeviations(Point[] points)
        {
            return StandardDeviations(points, Means(points));
        }

        public virtual double[] Variances(Point[] points, double[] means)
        {
            var numberOfDimensions = means.Length;
            var numberOfPoints = points.Length;
            var variances = new double[numberOfDimensions];

            for (var i = 0; i < numberOfDimensions; i++)
            {
                for (var j = 0; j < numberOfPoints; j++)
                    variances[i] += Math.Pow(points[j].Coordinates[i] - means[i], 2) / numberOfPoints;
            }

            return variances;
        }

        public virtual double[] Variances(Point[] points)
        {
            return Variances(points, Means(points));
        }
    }
}