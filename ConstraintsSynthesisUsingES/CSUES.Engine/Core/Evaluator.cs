using System;
using System.Runtime.CompilerServices;
using System.Threading;
using CSUES.Engine.Models;
using CSUES.Engine.Utils;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace CSUES.Engine.Core
{
    public class Evaluator : EvaluatorBase
    {
        private readonly Point[] _positivePoints;
        private readonly Point[] _negativePoints;
        private readonly ConstraintsBuilderBase _constraintsBuilder;
       
        public Evaluator(Point[] positivePoints, Point[] negativePoints, ConstraintsBuilderBase constraintsBuilder)
        {
            _positivePoints = positivePoints;
            _negativePoints = negativePoints;
            _constraintsBuilder = constraintsBuilder;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override double Evaluate(Solution solution)
        {
            var numberOfPositivePointsSatisfyingConstraints = 0;
            var numberOfNegativePointsSatisfyingConstraints = 0;
            var numberOfPositivePoints = _positivePoints.Length;
            var numberOfNegativePoints = _negativePoints.Length;
            var constraints = _constraintsBuilder.BuildConstraints(solution);

            //TODO
            //var mean = constraints.Sum(c => c.GetTermsCoefficients().Min(tc => Math.Abs(tc)));
            //var modifier = mean > 1 ? 1.0 / mean : 1;

            for (var i = 0; i < numberOfPositivePoints; i++)
            {
                if (constraints.IsSatisfyingConstraints(_positivePoints[i]))
                    numberOfPositivePointsSatisfyingConstraints++;
            }

            for (var i = 0; i < numberOfNegativePoints; i++)
            {
                if (constraints.IsSatisfyingConstraints(_negativePoints[i]))
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
