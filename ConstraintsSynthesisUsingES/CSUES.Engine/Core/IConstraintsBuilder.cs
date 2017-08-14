using CSUES.Engine.Models;
using ES.Core.Models.Solutions;

namespace CSUES.Engine.Core
{
    public interface IConstraintsBuilder
    {
        Constraint[] BuildConstraints(Solution solution);
    }
}