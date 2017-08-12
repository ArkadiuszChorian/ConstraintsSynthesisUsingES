using System.Collections.Generic;
using System.Linq;
using ES.Core.Constraints;
using ES.Core.Models;

namespace ES.Core.Benchmarks
{
    public class GeneralBenchmark : IBenchmark
    {
        public GeneralBenchmark(IEnumerable<Constraint> constraints, ExperimentParameters experimentParameters)
        {
            var numberOfDimensions = experimentParameters.NumberOfDimensions;
            var defaultLowerLimit = experimentParameters.DefaultDomainLowerLimit;
            var defaultUpperLimit = experimentParameters.DefaultDomainUpperLimit;

            Constraints = constraints.ToArray();
            Domains = new Domain[numberOfDimensions];

            for (var i = 0; i < numberOfDimensions; i++)
            {
                Domains[i] = new Domain(defaultLowerLimit, defaultUpperLimit);
            }
        }

        public GeneralBenchmark(IEnumerable<Constraint> constraints, IEnumerable<Domain> domains)
        {
            Constraints = constraints.ToArray();
            Domains = domains.ToArray();
        }

        public Constraint[] Constraints { get; set; }
        public Domain[] Domains { get; set; }
    }
}
