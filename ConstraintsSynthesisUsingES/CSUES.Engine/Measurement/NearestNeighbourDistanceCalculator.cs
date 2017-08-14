using CSUES.Engine.Models;

namespace CSUES.Engine.Measurement
{
    public class NearestNeighbourDistanceCalculator : INearestNeighbourDistanceCalculator
    {
        private readonly IDistanceCalculator _distanceCalculator;

        public NearestNeighbourDistanceCalculator(IDistanceCalculator distanceCalculator)
        {
            _distanceCalculator = distanceCalculator;
        }

        public void CalculateNearestNeighbourDistances(Point[] points)
        {
            var numberOfPositiveMeasurePoints = points.Length;

            for (var i = 0; i < numberOfPositiveMeasurePoints; i++)
            {
                points[i].DistanceToNearestNeighbour = int.MaxValue;

                for (var j = 0; j < numberOfPositiveMeasurePoints; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var distanceBetweenPoints = _distanceCalculator.Calculate(points[i].Coordinates, points[j].Coordinates);

                    if (distanceBetweenPoints < points[i].DistanceToNearestNeighbour)
                    {
                        points[i].DistanceToNearestNeighbour = distanceBetweenPoints;
                    }
                }
            }
        }
    }
}
