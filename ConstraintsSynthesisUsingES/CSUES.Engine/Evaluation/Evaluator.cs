using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Evaluation
{
    public class Evaluator : IEvaluator
    {
        public Evaluator(ExperimentParameters experimentParameters)
        {
            ExperimentParameters = experimentParameters;
            NumberOfConstraints = experimentParameters.NumberOfConstraints;
            NumberOfConstraintCoefficients = experimentParameters.NumberOfDimensions + 1;
        }
        public Evaluator(ExperimentParameters experimentParameters, Point[] positivePoints)
        {
            ExperimentParameters = experimentParameters;
            PositivePoints = positivePoints;
            NumberOfConstraints = experimentParameters.NumberOfConstraints;
            NumberOfConstraintCoefficients = experimentParameters.NumberOfDimensions + 1;
        }
        public Evaluator(ExperimentParameters experimentParameters, Point[] positivePoints, Point[] negativePoints)
        {
            ExperimentParameters = experimentParameters;
            PositivePoints = positivePoints;
            NegativePoints = negativePoints;
            NumberOfConstraints = experimentParameters.NumberOfConstraints;
            NumberOfConstraintCoefficients = experimentParameters.NumberOfDimensions + 1;
        }

        public Point[] PositivePoints { get; set; }
        public Point[] NegativePoints { get; set; }

        //Experiment parameters
        public int NumberOfConstraints { get; set; }
        public int NumberOfConstraintCoefficients { get; set; }
        public ExperimentParameters ExperimentParameters { get; set; }

        //public double Evaluate2(Solution solution)
        //{
        //    var constraints = solution.GetConstraints(ExperimentParameters);
        //    var numberOfPositivePointsSatisfyingConstraints = PositivePoints.Count(point => IsSatisfyingConstraints(constraints, point));
        //    var numberOfNegativePointsSatisfyingConstraints = NegativePoints.Count(point => IsSatisfyingConstraints(constraints, point));

        //    return (double)numberOfPositivePointsSatisfyingConstraints / (PositivePoints.Length + numberOfNegativePointsSatisfyingConstraints);
        //    //return (double)numberOfPositivePointsSatisfyingConstraints / (PositivePoints.Length + NegativePoints.Length);
        //    //return (double)numberOfPositivePointsSatisfyingConstraints;
        //}

        public double Evaluate(Solution solution)
        {
            var numberOfPositivePointsSatisfyingConstraints = 0;
            var numberOfNegativePointsSatisfyingConstraints = 0;
            var numberOfPositivePoints = PositivePoints.Length;
            var numberOfNegativePoints = NegativePoints.Length;
            var constraints = solution.GetConstraints(ExperimentParameters);

            for (var i = 0; i < numberOfPositivePoints; i++)
            {
                //if (IsSatisfyingConstraints(constraints, PositivePoints[i]))
                if (constraints.IsSatisfyingConstraints(PositivePoints[i]))
                    numberOfPositivePointsSatisfyingConstraints++;
            }

            for (var i = 0; i < numberOfNegativePoints; i++)
            {
                //if (IsSatisfyingConstraints(constraints, NegativePoints[i]))
                if (constraints.IsSatisfyingConstraints(NegativePoints[i]))
                    numberOfNegativePointsSatisfyingConstraints++;
            }

            //if (numberOfNegativePointsSatisfyingConstraints > 0 || numberOfPositivePointsSatisfyingConstraints > 0)
            //{
            //    Debugger.Break();
            //}

            return (double)numberOfPositivePointsSatisfyingConstraints / (numberOfPositivePoints + numberOfNegativePointsSatisfyingConstraints);
        }

        //private bool IsSatisfyingConstraints2(Solution solution, Point point)
        //{
        //    for (var i = 0; i < NumberOfConstraints; i += NumberOfConstraintCoefficients)
        //    {
        //        var constraintComputedValue = 0.0;
        //        var constraintLimitingValue = solution.ObjectCoefficients[i + NumberOfConstraintCoefficients - 1];

        //        for (var j = i; j < NumberOfConstraintCoefficients - 1; j++)
        //        {
        //            constraintComputedValue += solution.ObjectCoefficients[j] * point.Coordinates[j - i];
        //        }

        //        if (constraintComputedValue > constraintLimitingValue)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //public double Evaluate(Solution solution)
        //{
        //    var numberOfPositivePointsSatisfyingConstraints = 0;
        //    var numberOfNegativePointsSatisfyingConstraints = 0;
        //    //var numberOfRestrictions = Arguments.Get<int>("NumberOfRestrictions");
        //    //var numberOfConstraints = Arguments.Get<int>("NumberOfRestrictions");
        //    //var numberOfConstraintCoefficients = solution.ObjectCoefficients.Length / numberOfConstraints;

        //    for (var i = 0; i < PositivePoints.Length; i++)
        //    {
        //        if (IsSatisfyingConstraints(solution, PositivePoints[i]))
        //        {
        //            numberOfPositivePointsSatisfyingConstraints++;
        //        }
        //    }

        //    for (var i = 0; i < NegativePoints.Length; i++)
        //    {
        //        if (IsSatisfyingConstraints(solution, NegativePoints[i]))
        //        {
        //            numberOfNegativePointsSatisfyingConstraints++;
        //        }
        //    }

        //    return (double)numberOfPositivePointsSatisfyingConstraints / (PositivePoints.Length + numberOfNegativePointsSatisfyingConstraints);
        //    //return (double)numberOfPositivePointsSatisfyingConstraints / (PositivePoints.Length + NegativePoints.Length);
        //    //return (double)numberOfPositivePointsSatisfyingConstraints;
        //}

        //private bool IsSatisfyingConstraints2(List<Constraint> constraints, Point point)
        //{
        //    return constraints.All(constraint => constraint.IsSatisfyingConstraint(point));
        //}

        //private static bool IsSatisfyingConstraints(Constraint[] constraints, Point point)
        //{
        //    var length = constraints.Length;

        //    for (var i = 0; i < length; i++)
        //    {
        //        if (!constraints[i].IsSatisfyingConstraint(point))
        //            return false;
        //    }

        //    return true;
        //}
    }
}
