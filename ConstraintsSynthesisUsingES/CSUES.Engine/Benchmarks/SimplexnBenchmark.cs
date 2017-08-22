using System;
using System.Collections.Generic;
using CSUES.Engine.Enums;
using CSUES.Engine.Factories;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using CSUES.Engine.Models.Terms;
using CSUES.Engine.Utils;
using ES.Core.Utils;

namespace CSUES.Engine.Benchmarks
{
    public class SimplexnBenchmark : IBenchmark
    {
        public SimplexnBenchmark(ExperimentParameters experimentParameters, ITermsFactory termsFactory)
        {
            var numberOfDimensions = experimentParameters.NumberOfDimensions;
            var simplexnBoundaryValue = experimentParameters.SimplexnBoundaryValue;
            var tanPi12 = Math.Tan(Math.PI / 12);
            var cotPi12 = 1 / tanPi12;
            var constraints = new List<Constraint>(4 * numberOfDimensions - 6 + 1);
            var terms1 = new Term[numberOfDimensions];
            var terms2 = new Term[numberOfDimensions];

            Domains = new Domain[numberOfDimensions];

            for (var i = 0; i < numberOfDimensions; i++)
                Domains[i] = new Domain(-1, 2 + simplexnBoundaryValue);

            for (var i = 0; i < numberOfDimensions; i++)
            {                          
                for (var j = i + 1; j < numberOfDimensions; j++)
                {
                    for (var k = 0; k < numberOfDimensions; k++)
                    {
                        terms1[k] = termsFactory.Create((int)TermType.Linear, 0);
                        terms2[k] = termsFactory.Create((int)TermType.Linear, 0);
                    }

                    terms1[i].Coefficient = -cotPi12;
                    terms1[j].Coefficient = tanPi12;
                    terms2[j].Coefficient = -cotPi12;
                    terms2[i].Coefficient = tanPi12;

                    constraints.Add(new LinearConstraint(terms1.DeepCopyByExpressionTree(), 0));
                    constraints.Add(new LinearConstraint(terms2.DeepCopyByExpressionTree(), 0));
                }               
            }

            for (var i = 0; i < numberOfDimensions; i++)
                terms1[i] = termsFactory.Create((int)TermType.Linear, 1);

            constraints.Add(new LinearConstraint(terms1.DeepCopyByExpressionTree(), simplexnBoundaryValue));

            Constraints = constraints.ToArray();
        }

        public Constraint[] Constraints { get; set; }
        public Domain[] Domains { get; set; }
    }
}
