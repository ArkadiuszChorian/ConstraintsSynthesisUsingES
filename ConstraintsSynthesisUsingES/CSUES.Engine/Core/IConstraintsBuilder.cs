using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using ES.Core.Models.Solutions;

namespace CSUES.Engine.Core
{
    public interface IConstraintsBuilder
    {
        Constraint[] BuildConstraints(Solution solution);
    }
}