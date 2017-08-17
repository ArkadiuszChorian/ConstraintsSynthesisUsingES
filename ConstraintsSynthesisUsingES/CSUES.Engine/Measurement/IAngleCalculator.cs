using System.Collections.Generic;
using CSUES.Engine.Models.Constraints;

namespace CSUES.Engine.Measurement
{
    public interface IAngleCalculator
    {
        double Calculate(IList<Constraint> constraints1, IList<Constraint> constraints2);
    }
}