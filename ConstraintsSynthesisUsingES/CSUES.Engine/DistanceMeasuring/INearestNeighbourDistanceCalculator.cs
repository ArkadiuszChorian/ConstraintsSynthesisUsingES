using CSUES.Engine.Models;

namespace CSUES.Engine.DistanceMeasuring
{
    public interface INearestNeighbourDistanceCalculator
    {
        void CalculateNearestNeighbourDistances(Point[] points);
    }
}
