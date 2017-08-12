using System;
using System.Collections.Generic;
using ES.Core.Constraints;
using ES.Core.Models;

namespace ES.Core.Benchmarks
{
    public class SimplexnBenchmark : IBenchmark
    {
        public SimplexnBenchmark(ExperimentParameters experimentParameters)
        {
            var numberOfDimensions = experimentParameters.NumberOfDimensions;
            var simplexnBoundaryValue = experimentParameters.SimplexnBoundaryValue;
            var tanPi12 = Math.Tan(Math.PI / 12);
            var cotPi12 = 1 / tanPi12;
            var constraints = new List<Constraint>(numberOfDimensions * 2 - 1);
            var termsCoefficients = new double[numberOfDimensions];

            Domains = new Domain[numberOfDimensions];

            for (var i = 0; i < numberOfDimensions - 1; i++)
            {
                Domains[i] = new Domain(-1, 2 + simplexnBoundaryValue);

                var termsCoefficients1 = new double[numberOfDimensions];
                var termsCoefficients2 = new double[numberOfDimensions];
                termsCoefficients1[i] = -cotPi12;
                termsCoefficients1[i + 1] = tanPi12;
                termsCoefficients2[i + 1] = -cotPi12;
                termsCoefficients2[i] = tanPi12;

                constraints.Add(new LinearConstraint(termsCoefficients1, 0));
                constraints.Add(new LinearConstraint(termsCoefficients2, 0));

                termsCoefficients[i] = 1;
            }

            termsCoefficients[numberOfDimensions - 1] = 1;
            Domains[numberOfDimensions - 1] = new Domain(-1, 2 + simplexnBoundaryValue);
            constraints.Add(new LinearConstraint(termsCoefficients, simplexnBoundaryValue));

            Constraints = constraints.ToArray();
        }

        public Constraint[] Constraints { get; set; }
        public Domain[] Domains { get; set; }
    }
}
