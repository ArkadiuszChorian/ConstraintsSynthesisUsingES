using ES.Core.Constraints;
using ES.Core.Models;

namespace ES.Core.Benchmarks
{
    public class BallnBenchmark : IBenchmark
    {
        public BallnBenchmark(ExperimentParameters experimentParameters)
        {
            var numberOfDimensions = experimentParameters.NumberOfDimensions;
            var ballnBoundaryValue = experimentParameters.BallnBoundaryValue;
            var termsCoefficients = new double[numberOfDimensions];

            Constraints = new Constraint[1];
            Domains = new Domain[numberOfDimensions];

            for (var i = 0; i < numberOfDimensions; i++)
            {
                termsCoefficients[i] = i + 1;
                Domains[i] = new Domain(i + 1 - 2 * ballnBoundaryValue, i + 1 + 2 * ballnBoundaryValue);
            }

            Constraints[0] = new BallConstraint(termsCoefficients, ballnBoundaryValue * ballnBoundaryValue);
        }

        public Constraint[] Constraints { get; set; }
        public Domain[] Domains { get; set; }
    }
}
