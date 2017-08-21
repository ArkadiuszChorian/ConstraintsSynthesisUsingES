using CSUES.Engine.Enums;
using CSUES.Engine.Measurement;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using CSUES.Engine.Utils;
using ES.Core.Utils;

namespace CSUES.Engine.PointsGeneration
{
    public class NegativePointsGenerator : PointsGenerator
    {
        private readonly MersenneTwister _randomGenerator;
        private readonly Point[] _positiveMeasurePoints;
        private readonly IDistanceCalculator _distanceCalculator;      

        public NegativePointsGenerator(Point[] positiveMeasurePoints, IDistanceCalculator distanceCalculator, INearestNeighbourDistanceCalculator nearestNeighbourDistanceCalculator)
        {
            _randomGenerator = MersenneTwister.Instance;
            _positiveMeasurePoints = positiveMeasurePoints;
            _distanceCalculator = distanceCalculator;

            nearestNeighbourDistanceCalculator.CalculateNearestNeighbourDistances(positiveMeasurePoints);
        }

        protected override Point GetAllowedPoint(Domain[] domains, Constraint[] constraints)
        {
            var numberOfDimensions = domains.Length;
            var point = new Point(numberOfDimensions, ClassificationType.Negative);

            var isSatisfyingNearestNeighbourConstraints = false;

            while (isSatisfyingNearestNeighbourConstraints == false)
            {
                isSatisfyingNearestNeighbourConstraints = true;

                for (var j = 0; j < numberOfDimensions; j++)
                    point.Coordinates[j] = _randomGenerator.NextDouble(domains[j].LowerLimit, domains[j].UpperLimit);

                for (var j = 0; j < _positiveMeasurePoints.Length; j++)
                {
                    if (IsOutsideNeighbourhood(point, _positiveMeasurePoints[j])) continue;
                    isSatisfyingNearestNeighbourConstraints = false;
                    break;
                }
            }

            return point;
        }

        //protected override Point GetAllowedPoint(Domain[] domains, Constraint[] constraints)
        //{
        //    var numberOfDimensions = domains.Length;
        //    var numberOfConstraints = constraints.Length;
        //    var point = new Point(numberOfDimensions, ClassificationType.Negative);

        //    var isSatisfyingConstraints = true;

        //    while (isSatisfyingConstraints == true)
        //    {
        //        isSatisfyingConstraints = false;

        //        for (var i = 0; i < numberOfDimensions; i++)
        //            point.Coordinates[i] = _randomGenerator.NextDouble(domains[i].LowerLimit, domains[i].UpperLimit);

        //        if (constraints.IsSatisfyingConstraints(point))
        //            isSatisfyingConstraints = true;

        //        //for (var i = 0; i < numberOfConstraints; i++)
        //        //{
        //        //    if (constraints[i].IsSatisfyingConstraint(point) == false) break;
        //        //    isSatsfyngConstraints = true;
        //        //    break;
        //        //}
        //    }

        //    return point;
        //}

        private bool IsOutsideNeighbourhood(Point pointToCheck, Point centerPoint)
        {
            return _distanceCalculator.Calculate(pointToCheck.Coordinates, centerPoint.Coordinates) >
                   centerPoint.DistanceToNearestNeighbour;
        }
    }
}
