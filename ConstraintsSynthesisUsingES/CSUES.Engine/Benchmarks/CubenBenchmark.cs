using System.Collections.Generic;
using CSUES.Engine.Enums;
using CSUES.Engine.Factories;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Terms;

namespace CSUES.Engine.Benchmarks
{
    public class CubenBenchmark : IBenchmark
    {
        public CubenBenchmark(ExperimentParameters experimentParameters, ITermsFactory termsFactory)
        {
            var numberOfDimensions = experimentParameters.NumberOfDimensions;
            var cubenBoundaryValue = experimentParameters.CubenBoundaryValue;   
            var constraints = new List<Constraint>(numberOfDimensions * 2);
            
            Domains = new Domain[numberOfDimensions];

            for (var i = 0; i < numberOfDimensions; i++)
            {
                var value = i + 1;

                var terms1 = new Term[numberOfDimensions];
                var terms2 = new Term[numberOfDimensions];

                for (var j = 0; j < numberOfDimensions; j++)
                {
                    terms1[j] = termsFactory.Create((int)TermType.Linear, 0);
                    terms2[j] = termsFactory.Create((int)TermType.Linear, 0);
                }

                terms1[i].Coefficient = -1;
                terms2[i].Coefficient = 1;

                constraints.Add(new Constraint(terms1, -value));
                constraints.Add(new Constraint(terms2, value + value * cubenBoundaryValue));

                Domains[i] = new Domain(value - value * cubenBoundaryValue, value + 2 * value * cubenBoundaryValue);
            }

            Constraints = constraints.ToArray();
        }

        public Constraint[] Constraints { get; set; }
        public Domain[] Domains { get; set; }
    }
}
