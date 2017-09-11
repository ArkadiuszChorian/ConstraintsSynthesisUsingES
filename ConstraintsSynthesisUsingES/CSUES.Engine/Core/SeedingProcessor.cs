using System;
using System.Collections.Generic;
using System.Text;
using CSUES.Engine.Models;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace CSUES.Engine.Core
{
    public class SeedingProcessor : ISeedingProcessor
    {
        private readonly IEvaluator _evaluator;
        private readonly IConstraintsBuilder _constraintsBuilder;
        private readonly IList<Point> _positivePoints;

        public SeedingProcessor(IEvaluator evaluator, IConstraintsBuilder constraintsBuilder, IList<Point> positivePoints)
        {
            _evaluator = evaluator;
            _constraintsBuilder = constraintsBuilder;
            _positivePoints = positivePoints;
        }

        public Solution[] Seed(Solution[] solutions)
        {
            //Console.WriteLine($"Solution obj coeffs len input = {solutions[0].ObjectCoefficients.Length}");           
            foreach (var solution in solutions)
            {
                //Console.ReadKey();
                //Console.WriteLine("Solution");
                //Console.WriteLine(new StringBuilder().AppendArray(solution.ObjectCoefficients).ToString());
                var newObjectCoefficients = new List<double>();
                var constraints = _constraintsBuilder.BuildConstraints(solution);

                foreach (var constraint in constraints)
                {
                    //Console.WriteLine("Constraint");
                    var bestFitnessScore = 0.0;
                    var bestLimitingValue = 0.0;

                    foreach (var positivePoint in _positivePoints)
                    {
                        var leftSideValue = constraint.GetLeftSideValue(positivePoint);
                        constraint.LimitingValue = leftSideValue;

                        var singleConstraintAsSolution = new Solution(constraint.Terms.Length + 1)
                        {
                            ObjectCoefficients = constraint.GetAllCoefficients()
                        };

                        var newFitnessScore = _evaluator.Evaluate(singleConstraintAsSolution);

                        if (!(newFitnessScore > bestFitnessScore)) continue;

                        bestFitnessScore = newFitnessScore;
                        bestLimitingValue = leftSideValue;
                    }

                    constraint.LimitingValue = bestLimitingValue;
                    newObjectCoefficients.AddRange(constraint.GetAllCoefficients());
                }

                solution.ObjectCoefficients = newObjectCoefficients.ToArray();
                //Console.WriteLine(new StringBuilder().AppendArray(solution.ObjectCoefficients).ToString());
            }
            //Console.WriteLine($"Solution obj coeffs len output = {solutions[0].ObjectCoefficients.Length}");
            //Console.WriteLine("#####################");
            Console.WriteLine("Seeding finished!");

            return solutions;
        }
    }
}