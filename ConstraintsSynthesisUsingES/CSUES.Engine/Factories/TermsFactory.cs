using System;
using CSUES.Engine.Enums;
using CSUES.Engine.Models.Terms;

namespace CSUES.Engine.Factories
{
    public class TermsFactory : ITermsFactory
    {
        public Term Create(int type, double coefficient, double power = 1)
        {
            var termType = (TermType) type;

            switch (termType)
            {
                case TermType.Linear:
                    return new LinearTerm(coefficient);
                case TermType.Quadratic:
                    return new QuadraticTerm(coefficient);
                case TermType.Cubic:
                    return new CubicTerm(coefficient);
                case TermType.Npower:
                    return new NpowerTerm(coefficient, power);
                //case TermType.SquareRoot:
                //    break;
                //case TermType.Exponential:
                //    break;
                //case TermType.NaturalLogarithm:
                //    break;
                //case TermType.BaseTenLogarithm:
                //    break;
                //case TermType.AbsoluteValue:
                //    break;
                //case TermType.Sine:
                //    break;
                //case TermType.Cosine:
                //    break;
                //case TermType.Tangent:
                //    break;
                //case TermType.ArcSine:
                //    break;
                //case TermType.ArcCosine:
                //    break;
                //case TermType.ArcTangent:
                //    break;
                //case TermType.HiperbolicSine:
                //    break;
                //case TermType.HiperbolicCosine:
                //    break;
                //case TermType.HiperbolicTangent:
                //    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(termType), termType, null);
            }
        }
    }
}