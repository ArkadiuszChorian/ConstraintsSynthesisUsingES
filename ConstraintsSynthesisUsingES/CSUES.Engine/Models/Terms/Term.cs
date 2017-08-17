using System;
using CSUES.Engine.Enums;

namespace CSUES.Engine.Models.Terms
{
    public class Term
    {
        protected readonly Func<double[], double> Function;

        public Term(double coefficient, Func<double[], double> function, TermType termType)
        {
            Coefficient = coefficient;
            Function = function;
            Type = termType;
        }

        public virtual double Value(params double[] arguments)
        {
            return Function.Invoke(arguments);
        }

        public double Coefficient { get; set; }
        public TermType Type { get; private set; }
    }
}