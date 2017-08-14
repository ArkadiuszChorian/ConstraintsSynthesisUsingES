﻿using CSUES.Engine.Models;
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

            return (double)numberOfPositivePointsSatisfyingConstraints / (_positivePoints.Length + numberOfNegativePointsSatisfyingConstraints);
        }
    }
}