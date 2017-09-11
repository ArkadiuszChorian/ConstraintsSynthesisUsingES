using System;
using System.Threading;
using CSUES.Engine.Models;
using CSUES.Engine.Utils;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace CSUES.Engine.Core
{
    public class Evaluator : IEvaluator
    {
        private readonly Point[] _positivePoints;
        private readonly Point[] _negativePoints;
        private readonly IConstraintsBuilder _constraintsBuilder;
       
        public Evaluator(Point[] positivePoints, Point[] negativePoints, IConstraintsBuilder constraintsBuilder)
        {
            _positivePoints = positivePoints;
            _negativePoints = negativePoints;
            _constraintsBuilder = constraintsBuilder;
        }
        
        public double Evaluate(Solution solution)
        {
            var numberOfPositivePointsSatisfyingConstraints = 0;
            var numberOfNegativePointsSatisfyingConstraints = 0;
            var constraints = _constraintsBuilder.BuildConstraints(solution);

            //TODO
            //var mean = constraints.Sum(c => c.GetTermsCoefficients().Min(tc => Math.Abs(tc)));
            //var modifier = mean > 1 ? 1.0 / mean : 1;

            foreach (var positivePoint in _positivePoints)
            {
                if (constraints.IsSatisfyingConstraints(positivePoint))
                    numberOfPositivePointsSatisfyingConstraints++;
            }

            foreach (var negativePoint in _negativePoints)
            {
                if (constraints.IsSatisfyingConstraints(negativePoint))
                    numberOfNegativePointsSatisfyingConstraints++;
            }

            //Console.Write($"Positive satisfying = {numberOfPositivePointsSatisfyingConstraints} ### ");
            //Console.Write($"Negative satisfying = {numberOfNegativePointsSatisfyingConstraints}\n");
            //Thread.Sleep(100);

            //TODO
            return (double)numberOfPositivePointsSatisfyingConstraints / (_positivePoints.Length + numberOfNegativePointsSatisfyingConstraints);
            //return ((double)numberOfPositivePointsSatisfyingConstraints / (_positivePoints.Length + numberOfNegativePointsSatisfyingConstraints)) * modifier;
        }
    }
}
