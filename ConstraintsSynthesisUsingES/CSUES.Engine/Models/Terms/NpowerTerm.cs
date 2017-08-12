using System;
using ES.Core.Enums;

namespace ES.Core.Models.Terms
{
    public class NpowerTerm : Term
    {
        private readonly double _power;

        public NpowerTerm(double coefficient, double power) : base(coefficient)
        {
            _power = power;
        }

        public NpowerTerm(double coefficient, TermType termType, double power) : base(coefficient, termType)
        {
            _power = power;
        }

        public override double Value(double argument)
        {
            return Math.Pow(argument, _power);
        }
    }
}