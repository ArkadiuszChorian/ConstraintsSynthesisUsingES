using CSUES.Engine.Enums;
using CSUES.Engine.Factories;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using CSUES.Engine.Models.Terms;

namespace CSUES.Engine.Benchmarks
{
    public class BallnBenchmark : IBenchmark
    {
        public BallnBenchmark(ExperimentParameters experimentParameters, ITermsFactory termsFactory)
        {
            var numberOfDimensions = experimentParameters.NumberOfDimensions;
            var ballnBoundaryValue = experimentParameters.BallnBoundaryValue;
            var numberOfTerms = numberOfDimensions * 2;
            var terms = new Term[numberOfTerms];
            var limitingValue = ballnBoundaryValue * ballnBoundaryValue;

            Constraints = new Constraint[1];
            Domains = new Domain[numberOfDimensions];

            for (var i = 0; i < numberOfDimensions; i++)
            {
                var value = i + 1;              
                terms[i] = termsFactory.Create((int)TermType.Quadratic, 1);

                Domains[i] = new Domain(value - 2 * ballnBoundaryValue, value + 2 * ballnBoundaryValue);

                //HACK TODO
                Domains[i] = new Domain(value - 2 * ballnBoundaryValue - 100, value + 2 * ballnBoundaryValue + 100);

                limitingValue -= value * value;
            }

            for (var i = numberOfDimensions; i < numberOfTerms; i++)
            {
                var value = i + 1 - numberOfDimensions;
                terms[i] = termsFactory.Create((int) TermType.Linear, -2 * value);
            }

            Constraints[0] = new BallConstraint(terms, limitingValue);
        }

        public Constraint[] Constraints { get; set; }
        public Domain[] Domains { get; set; }
    }
}
