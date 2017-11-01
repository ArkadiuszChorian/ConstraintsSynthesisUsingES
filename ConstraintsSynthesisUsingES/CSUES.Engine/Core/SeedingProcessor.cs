using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSUES.Engine.Models;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace CSUES.Engine.Core
{
    public class SeedingProcessor : ISeedingProcessor
    {
        private readonly EvaluatorBase _evaluator;
        private readonly ConstraintsBuilderBase _constraintsBuilder;
        private readonly Point[] _positivePoints;

        public SeedingProcessor(EvaluatorBase evaluator, ConstraintsBuilderBase constraintsBuilder, IList<Point> positivePoints)
        {
            _evaluator = evaluator;
            _constraintsBuilder = constraintsBuilder;
            _positivePoints = positivePoints.ToArray();
        }

        public Solution[] Seed(Solution[] solutions)
        {
            //Console.WriteLine($"Solution obj coeffs len input = {solutions[0].ObjectCoefficients.Length}"); 
            var numberOfPoints = _positivePoints.Length;
                      
            foreach (var solution in solutions)
            {
                //Console.ReadKey();
                //Console.WriteLine("Solution");
                //Console.WriteLine(new StringBuilder().AppendArray(solution.ObjectCoefficients).ToString());
                var newObjectCoefficients = new List<double>(solution.ObjectCoefficients.Length);
                var constraints = _constraintsBuilder.BuildConstraints(solution);

                foreach (var constraint in constraints)
                {
                    //Console.WriteLine("Constraint");
                    var bestFitnessScore = 0.0;
                    var bestLimitingValue = 0.0;

                    for (var i = 0; i < numberOfPoints; i++)
                    {
                        var leftSideValue = constraint.GetLeftSideValue(_positivePoints[i]);
                        constraint.LimitingValue = leftSideValue;

                        var singleConstraintAsSolution = new Solution(constraint.Terms.Length + 1)
                        {
                            ObjectCoefficients = constraint.GetAllCoefficients()
                        };

                        var newFitnessScore = _evaluator.Evaluate(singleConstraintAsSolution);

                        if (newFitnessScore > bestFitnessScore)
                        {
                            bestFitnessScore = newFitnessScore;
                            bestLimitingValue = leftSideValue;
                        }
                    }

                    constraint.LimitingValue = bestLimitingValue;
                    newObjectCoefficients.AddRange(constraint.GetAllCoefficients());
                }

                solution.ObjectCoefficients = newObjectCoefficients.ToArray();
                //Console.WriteLine(new StringBuilder().AppendArray(solution.ObjectCoefficients).ToString());
            }
            //Console.WriteLine($"Solution obj coeffs len output = {solutions[0].ObjectCoefficients.Length}");
            //Console.WriteLine("#####################");
            //Console.WriteLine("Seeding finished!");

            return solutions;
        }
    }
}