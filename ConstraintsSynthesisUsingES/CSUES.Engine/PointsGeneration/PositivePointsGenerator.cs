using CSUES.Engine.Enums;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using ES.Core.Utils;

namespace CSUES.Engine.PointsGeneration
{
    public class PositivePointsGenerator : PointsGenerator
    {
        private readonly MersenneTwister _randomGenerator;

        public PositivePointsGenerator()
        {
            _randomGenerator = MersenneTwister.Instance;
        }

        protected override Point GetAllowedPoint(Domain[] domains, Constraint[] constraints)
        {
            var numberOfDimensions = domains.Length;
            var numberOfConstraints = constraints.Length;
            var point = new Point(numberOfDimensions, ClassificationType.Positive);
            
            var isSatsfyngConstraints = false;

            while (isSatsfyngConstraints == false)
            {
                isSatsfyngConstraints = true;

                for (var i = 0; i < numberOfDimensions; i++)
                    point.Coordinates[i] = _randomGenerator.NextDouble(domains[i].LowerLimit, domains[i].UpperLimit);

                //HACK

                //point.Coordinates[0] = 1;
                //point.Coordinates[1] = -0.5;

                //

                for (var i = 0; i < numberOfConstraints; i++)
                {
                    if (constraints[i].IsSatisfyingConstraint(point)) continue;
                    isSatsfyngConstraints = false;
                    break;
                }
            }

            return point;
        }
    }
}
