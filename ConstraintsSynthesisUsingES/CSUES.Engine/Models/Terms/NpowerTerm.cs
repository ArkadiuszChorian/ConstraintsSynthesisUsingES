using System;
using CSUES.Engine.Enums;

namespace CSUES.Engine.Models.Terms
{
    public class NpowerTerm : Term
    {
        public NpowerTerm(double coefficient, double power) : base(coefficient, power, TermType.Npower)
        {
        }

        public override double Value(double argument)
        {
            return Math.Pow(argument, Power);
        }
    }
}