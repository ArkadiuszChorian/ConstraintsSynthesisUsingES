using System.Collections.Generic;
using System.Linq;
using CSUES.Engine.Models;

namespace CSUES.Engine.Benchmarks
{
    public class GenericBenchmark : IBenchmark
    {
        public GenericBenchmark(ExperimentParameters experimentParameters)
        {
            var numberOfDimensions = experimentParameters.NumberOfDimensions;
            var defaultLowerLimit = experimentParameters.DefaultDomainLowerLimit;
            var defaultUpperLimit = experimentParameters.DefaultDomainUpperLimit;

            Constraints = experimentParameters.ReferenceConstraints.ToArray();
            Domains = new Domain[numberOfDimensions];

            for (var i = 0; i < numberOfDimensions; i++)
            {
                Domains[i] = new Domain(defaultLowerLimit, defaultUpperLimit);
            }
        }

        public GenericBenchmark(IEnumerable<Constraint> constraints, IEnumerable<Domain> domains)
        {
            Constraints = constraints.ToArray();
            Domains = domains.ToArray();
        }

        public Constraint[] Constraints { get; set; }
        public Domain[] Domains { get; set; }
    }
}
