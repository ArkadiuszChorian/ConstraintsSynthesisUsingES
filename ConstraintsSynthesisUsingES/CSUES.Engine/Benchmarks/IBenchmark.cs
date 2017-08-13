using CSUES.Engine.Models;

namespace CSUES.Engine.Benchmarks
{
    public interface IBenchmark
    {
        Constraint[] Constraints { get; set; }
        Domain[] Domains { get; set; }
    }
}
