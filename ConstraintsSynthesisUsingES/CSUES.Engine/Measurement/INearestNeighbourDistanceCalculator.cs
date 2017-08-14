using CSUES.Engine.Models;

namespace CSUES.Engine.Measurement
{
    public interface INearestNeighbourDistanceCalculator
    {
        void CalculateNearestNeighbourDistances(Point[] points);
    }
}
