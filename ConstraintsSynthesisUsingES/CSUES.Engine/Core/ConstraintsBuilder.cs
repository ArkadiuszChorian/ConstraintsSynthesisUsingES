using System.Linq;
using CSUES.Engine.Enums;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace CSUES.Engine.Core
{
    public class ConstraintsBuilder : IConstraintsBuilder
    {
        private readonly Constraint[] _constraintsModel;

        public ConstraintsBuilder(Constraint[] referenceConstraints, ExperimentParameters experimentParameters)
        {
            if (experimentParameters.TypeOfBenchmark == BenchmarkType.Balln && experimentParameters.AllowQuadraticTerms
                || experimentParameters.TypeOfBenchmark != BenchmarkType.Balln)
            {
                _constraintsModel = referenceConstraints;
            }
            else
            {
                _constraintsModel = new Constraint[experimentParameters.MaximumNumberOfConstraints];
                var terms = referenceConstraints.First().Terms.Where(term => term.Type == TermType.Linear).ToArray();

                for (var i = 0; i < _constraintsModel.Length; i++)
                    _constraintsModel[i] = new LinearConstraint(terms.DeepCopyByExpressionTree(), 0);
            }            
        }

        public Constraint[] BuildConstraints(Solution solution)
        {
            var constraints = _constraintsModel.DeepCopyByExpressionTree();
            var coefficients = solution.ObjectCoefficients;
            var index = 0;

            foreach (var constraint in constraints)
            {
                var numberOfTerms = constraint.Terms.Length;
                //var terms = constraint.Terms;

                for (var j = 0; j < numberOfTerms; j++)
                    constraint.Terms[j].Coefficient = coefficients[index++];

                constraint.LimitingValue = coefficients[index++];
            }

            return constraints;
        }
    }
}