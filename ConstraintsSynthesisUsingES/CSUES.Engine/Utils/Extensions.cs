using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CSUES.Engine.Enums;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using CSUES.Engine.Models.Terms;
using ES.Core.Models;
using ES.Core.Utils;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace CSUES.Engine.Utils
{
    public static class Extensions
    {     
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSatisfyingConstraints(this Constraint[] constraints, Point point)
        {
            var length = constraints.Length;

            for (var i = 0; i < length; i++)
            {
                if (!constraints[i].IsSatisfyingConstraint(point))
                    return false;
            }

            //HACK TODO
            //var partOfConstraintsCanBeNotSatisfied = 0.05;
            //var maximumNumberOfNotSatisfiedConstraints = point.ClassificationType == ClassificationType.Positive 
            //    ? (int)(partOfConstraintsCanBeNotSatisfied * constraints.Count) : 0;
            //var numberOfNotSatisfiedConstraints = 0;

            //for (var i = 0; i < length; i++)
            //{
            //    if (!constraints[i].IsSatisfyingConstraint(point))
            //        numberOfNotSatisfiedConstraints++;

            //    if (numberOfNotSatisfiedConstraints <= maximumNumberOfNotSatisfiedConstraints) continue;
            //    //Console.WriteLine($"Not satisfied more than {MaximumNumberOfNotSatisfiedConstraints}");
            //    return false;
            //}
            //
            //Console.WriteLine($"Not satisfied = {numberOfNotSatisfiedConstraints}");

            return true;
        }
        
        public static string ToLpFormat(this IList<Constraint> constraints, IList<Domain> domains)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Subject To");

            for (var i = 0; i < constraints.Count; i++)
            {
                sb.Append("\t");
                sb.AppendFormat("c{0}: ", i);

                var quadraticTerms = constraints[i].Terms.Where(t => t.Type == TermType.Quadratic).ToArray();
                var linearTerms = constraints[i].Terms.Where(t => t.Type == TermType.Linear).ToArray();
                var isFirst = true;

                for (var j = 0; j < quadraticTerms.Length; j++)
                {
                    if (!isFirst && quadraticTerms[j].Coefficient != 0.0)
                        sb.Append(" + ");

                    if (quadraticTerms[j].Coefficient != 0.0)
                    {
                        sb.AppendFormat("{0} x{1} ^ {2}", quadraticTerms[j].Coefficient, j, 2);
                        isFirst = false;
                    }                       
                }

                for (var j = 0; j < linearTerms.Length; j++)
                {
                    if (!isFirst && linearTerms[j].Coefficient != 0.0)
                        sb.Append(" + ");

                    if (linearTerms[j].Coefficient != 0.0)
                    {
                        sb.AppendFormat("{0} x{1}", linearTerms[j].Coefficient, j);
                        isFirst = false;
                    }                      
                }

                sb.Append(" <= ");
                sb.Append(constraints[i].LimitingValue);
                sb.Append("\n");
            }

            sb.AppendLine("Bounds");

            for (var i = 0; i < domains.Count; i++)
            {
                sb.Append("\t");
                sb.AppendFormat("{0} <= x{1} <= {2}", domains[i].LowerLimit, i, domains[i].UpperLimit);
                sb.Append("\n");
            }

            //sb.AppendLine("Generals");
            //sb.Append("\t");

            //for (var i = 0; i < domains.Count; i++)
            //{
            //    sb.AppendFormat("x{0}", i);
            //    sb.Append(i != domains.Count - 1 ? " " : string.Empty);
            //}

            //sb.Append("\n");
            sb.Append("End");

            return sb.ToString();
        }

        public static string ToLpFormatSimplified(this IList<Constraint> constraints, IList<Domain> domains)
        {
            var simplifiedConstraints = constraints.DeepCopyByExpressionTree().SimplifyCoefficients();

            var sb = new StringBuilder();

            sb.AppendLine("Subject To");

            for (var i = 0; i < simplifiedConstraints.Count; i++)
            {
                sb.Append("\t");
                sb.AppendFormat("c{0}: ", i);

                var quadraticTerms = simplifiedConstraints[i].Terms.Where(t => t.Type == TermType.Quadratic).ToArray();
                var linearTerms = simplifiedConstraints[i].Terms.Where(t => t.Type == TermType.Linear).ToArray();
                var isFirst = true;

                for (var j = 0; j < quadraticTerms.Length; j++)
                {
                    if (!isFirst && quadraticTerms[j].Coefficient != 0.0)
                        sb.Append(" + ");

                    if (quadraticTerms[j].Coefficient != 0.0)
                    {
                        sb.AppendFormat("{0:G5} x{1} ^ {2}", quadraticTerms[j].Coefficient, j, 2);
                        isFirst = false;
                    }
                }

                for (var j = 0; j < linearTerms.Length; j++)
                {
                    if (!isFirst && linearTerms[j].Coefficient != 0.0)
                        sb.Append(" + ");

                    if (linearTerms[j].Coefficient != 0.0)
                    {
                        sb.AppendFormat("{0:G5} x{1}", linearTerms[j].Coefficient, j);
                        isFirst = false;
                    }
                }

                sb.Append(" <= ");
                sb.AppendFormat("{0:G5}", simplifiedConstraints[i].LimitingValue);
                sb.Append("\n");
            }

            sb.AppendLine("Bounds");

            for (var i = 0; i < domains.Count; i++)
            {
                sb.Append("\t");
                sb.AppendFormat("{0} <= x{1} <= {2}", domains[i].LowerLimit, i, domains[i].UpperLimit);
                sb.Append("\n");
            }

            //sb.AppendLine("Generals");
            //sb.Append("\t");

            //for (var i = 0; i < domains.Count; i++)
            //{
            //    sb.AppendFormat("x{0}", i);
            //    sb.Append(i != domains.Count - 1 ? " " : string.Empty);
            //}

            //sb.Append("\n");
            sb.Append("End");

            return sb.ToString();
        }

        private static IList<Constraint> SimplifyCoefficients(this IList<Constraint> constraints)
        {
            foreach (var constraint in constraints)
            {
                var minimalCoefficient = constraint
                    .GetAllCoefficients()                    
                    .Select(Math.Abs)
                    .Where(c => c > 0)
                    .Min();

                foreach (var term in constraint.Terms)
                    term.Coefficient /= minimalCoefficient;

                constraint.LimitingValue /= minimalCoefficient;
            }

            return constraints;
        }

        public static double[] Means(this Point[] points)
        {
            var numberOfDimensions = points.First().Coordinates.Length;
            var numberOfPoints = points.Length;
            var means = new double[numberOfDimensions];

            foreach (var point in points)
            {
                for (var i = 0; i < numberOfDimensions; i++)
                    means[i] += point.Coordinates[i] / numberOfPoints;
            }

            return means;
        }

        public static double[] StandardDeviations(this Point[] points, double[] means)
        {
            var numberOfDimensions = means.Length;
            var variances = Variances(points, means);
            var standardDeviations = new double[numberOfDimensions];

            for (var i = 0; i < numberOfDimensions; i++)
                standardDeviations[i] = Math.Sqrt(variances[i]);

            return standardDeviations;
        }

        public static double[] StandardDeviations(this Point[] points)
        {
            return StandardDeviations(points, Means(points));
        }

        public static double[] Variances(this Point[] points, double[] means)
        {
            var numberOfDimensions = means.Length;
            var numberOfPoints = points.Length;
            var variances = new double[numberOfDimensions];

            for (var i = 0; i < numberOfDimensions; i++)
            {
                for (var j = 0; j < numberOfPoints; j++)
                    variances[i] += Math.Pow(points[j].Coordinates[i] - means[i], 2) / numberOfPoints;
            }

            return variances;
        }

        public static double[] Variances(this Point[] points)
        {
            return Variances(points, Means(points));
        }        
    }
}
