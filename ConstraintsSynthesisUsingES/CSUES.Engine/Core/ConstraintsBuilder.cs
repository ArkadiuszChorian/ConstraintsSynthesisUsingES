using CSUES.Engine.Models;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace CSUES.Engine.Core
{
    public class ConstraintsBuilder : IConstraintsBuilder
    {
        private readonly Constraint[] _referenceConstraints;

        public ConstraintsBuilder(Constraint[] referenceConstraints)
        {
            _referenceConstraints = referenceConstraints;
        }

        public Constraint[] BuildConstraints(Solution solution)
        {
            var constraints = _referenceConstraints.DeepCopyByExpressionTree();
            var coefficients = solution.ObjectCoefficients;
            var index = 0;

            foreach (var constraint in constraints)
            {
                var numberOfTerms = constraint.Terms.Length;
                var terms = constraint.Terms;

                for (var i = 0; i < numberOfTerms; i++)
                    terms[i].Coefficient = coefficients[index++];

                constraint.LimitingValue = coefficients[index++];
            }

            return constraints;
        }
    }
}