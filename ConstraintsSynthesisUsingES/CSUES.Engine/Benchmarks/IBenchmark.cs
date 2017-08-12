using ES.Core.Constraints;
using ES.Core.Models;

namespace ES.Core.Benchmarks
{
    public interface IBenchmark
    {
        Constraint[] Constraints { get; set; }
        Domain[] Domains { get; set; }
    }
}
