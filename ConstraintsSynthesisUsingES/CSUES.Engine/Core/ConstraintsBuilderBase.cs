using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using ES.Core.Models.Solutions;

namespace CSUES.Engine.Core
{
    public abstract class ConstraintsBuilderBase
    {
        public abstract Constraint[] BuildConstraints(Solution solution);
    }
}