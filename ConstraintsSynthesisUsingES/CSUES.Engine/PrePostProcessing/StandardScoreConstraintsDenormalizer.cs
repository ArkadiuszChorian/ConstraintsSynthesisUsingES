using System;
using CSUES.Engine.Enums;
using CSUES.Engine.Models.Constraints;

namespace CSUES.Engine.PrePostProcessing
{
    public class StandardScoreConstraintsDenormalizer : IProcessor<Constraint[]>
    {
        private readonly double[] _means;
        private readonly double[] _standardDeviations;

        public StandardScoreConstraintsDenormalizer(double[] means, double[] standardDeviations)
        {
            _means = means;
            _standardDeviations = standardDeviations;
        }

        public Constraint[] ApplyProcessing(Constraint[] constraints)
        {
            foreach (var constraint in constraints)
            {
                var numberOfTerms = constraint.Terms.Length;
                var limitingValueModifier = 0.0;

                switch (constraint.Type)
                {
                    case ConstraintType.Linear:                       
                        for (var i = 0; i < numberOfTerms; i++)
                        {
                            limitingValueModifier += constraint.Terms[i].Coefficient * _means[i] / _standardDeviations[i];
                            constraint.Terms[i].Coefficient /= _standardDeviations[i];
                        }

                        constraint.LimitingValue += limitingValueModifier;                                                  
                        break;
                    case ConstraintType.Quadratic:
                        var numberOfDimensions = numberOfTerms / 2;

                        for (var i = 0; i < numberOfDimensions; i++)
                        {
                            var value = _means[i] / _standardDeviations[i];
                            limitingValueModifier -= constraint.Terms[i].Coefficient * value * value;
                            constraint.Terms[i].Coefficient /= _standardDeviations[i] * _standardDeviations[i];
                        }

                        for (var i = numberOfDimensions; i < numberOfTerms; i++)
                        {
                            var index = i - numberOfDimensions;
                            limitingValueModifier += constraint.Terms[i].Coefficient * _means[index] / _standardDeviations[index];
                            constraint.Terms[i].Coefficient = constraint.Terms[i].Coefficient / _standardDeviations[index] 
                                - 2 * constraint.Terms[index].Coefficient * _means[index] / (_standardDeviations[index] * _standardDeviations[index]);
                        }

                        constraint.LimitingValue += limitingValueModifier;

                        break;
                    default:
                        throw new NotImplementedException("Normalization is not implemented for constraints other than linear and quadratic.");
                }
            }

            return constraints;
        }
    }
}