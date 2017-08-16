using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;

namespace CSUES.Engine.Benchmarks
{
    public interface IBenchmark
    {
        Constraint[] Constraints { get; set; }
        Domain[] Domains { get; set; }
    }
}
