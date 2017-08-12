using System.Collections.Generic;
using ES.Core.Constraints;
using ES.Core.Models;

namespace ES.Core.Benchmarks
{
    public class CubenBenchmark : IBenchmark
    {
        public CubenBenchmark(ExperimentParameters experimentParameters)
        {
            var numberOfDimensions = experimentParameters.NumberOfDimensions;
            var cubenBoundaryValue = experimentParameters.CubenBoundaryValue;   
            var constraints = new List<Constraint>(numberOfDimensions * 2);
            
            Domains = new Domain[numberOfDimensions];

            for (var i = 0; i < numberOfDimensions; i++)
            {
                var value = i + 1;
                var termsCoefficients1 = new double[numberOfDimensions];
                var termsCoefficients2 = new double[numberOfDimensions];
                termsCoefficients1[i] = -1;
                termsCoefficients2[i] = 1;

                constraints.Add(new LinearConstraint(termsCoefficients1, -value));
                constraints.Add(new LinearConstraint(termsCoefficients2, value + value * cubenBoundaryValue));

                Domains[i] = new Domain(value - value * cubenBoundaryValue, value + 2 * value * cubenBoundaryValue);
            }

            Constraints = constraints.ToArray();
        }

        public Constraint[] Constraints { get; set; }
        public Domain[] Domains { get; set; }
    }
}
