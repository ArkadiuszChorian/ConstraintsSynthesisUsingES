using ES.Core.Models;

namespace ES.Core.DistanceMeasuring
{
    public interface INearestNeighbourDistanceCalculator
    {
        void CalculateNearestNeighbourDistances(Point[] points);
    }
}
