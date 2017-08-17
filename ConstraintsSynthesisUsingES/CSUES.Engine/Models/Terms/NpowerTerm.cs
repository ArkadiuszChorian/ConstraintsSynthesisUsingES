using System;
using CSUES.Engine.Enums;

namespace CSUES.Engine.Models.Terms
{
    public class NpowerTerm : Term
    {
        private readonly double _power;
        public NpowerTerm(double coefficient, double power) : base(coefficient, null, TermType.Npower)
        {
            _power = power;
        }

        public override double Value(params double[] arguments)
        {
            return Math.Pow(arguments[0], _power);
        }
    }
}