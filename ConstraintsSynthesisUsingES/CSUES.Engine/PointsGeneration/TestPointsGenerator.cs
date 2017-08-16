using CSUES.Engine.Enums;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using CSUES.Engine.Utils;

namespace CSUES.Engine.PointsGeneration
{
    public class TestPointsGenerator : PointsGenerator
    {
        protected override Point GetAllowedPoint(Domain[] domains, Constraint[] constraints)
        {
            var point = base.GetAllowedPoint(domains, constraints);

            point.ClassificationType = constraints.IsSatisfyingConstraints(point)
                ? ClassificationType.Positive
                : ClassificationType.Negative;

            return point;
        }
    }
}