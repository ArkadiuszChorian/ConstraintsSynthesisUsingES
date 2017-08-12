using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ES.Core.Constraints;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.Utils
{
    public static class Extensions
    {
        public static Constraint[] GetConstraints(this Solution solution, ExperimentParameters experimentParameters)
        {
            var numberOfConstraintCoefficients = experimentParameters.NumberOfDimensions + 1;
            var numberOfConstraints = experimentParameters.NumberOfConstraints;
            var limiter = numberOfConstraintCoefficients * numberOfConstraints;
            var constraints = new Constraint[numberOfConstraints];            
            var j = 0;

            for (var i = 0; i < limiter; i += numberOfConstraintCoefficients)
            {
                var constraintLimitingValue = solution.ObjectCoefficients[i + numberOfConstraintCoefficients - 1];
                var constraintCoefficients = new double[numberOfConstraintCoefficients - 1];

                Array.Copy(solution.ObjectCoefficients, i, constraintCoefficients, 0, numberOfConstraintCoefficients - 1);
                
                constraints[j++] = new LinearConstraint(constraintCoefficients, constraintLimitingValue);
            }

            return constraints;
        }

        public static bool IsSatisfyingConstraints(this IList<Constraint> constraints, Point point)
        {
            var length = constraints.Count;

            for (var i = 0; i < length; i++)
            {
                if (!constraints[i].IsSatisfyingConstraint(point))
                    return false;
            }

            return true;
        }

        public static string GetHashString(this ExperimentParameters experimentParameters)
        {
            var hashStringBuilder = new StringBuilder();
            var propertyInfos = experimentParameters.GetDbSerializableProperties().OrderBy(pi => pi.Name);
            
            foreach (var propertyInfo in propertyInfos)
            {
                hashStringBuilder.Append(propertyInfo.GetValue(experimentParameters, null));
            }

            return hashStringBuilder.ToString();
        }

        public static IEnumerable<PropertyInfo> GetDbSerializableProperties<T>(this T obj)
        {
            return obj.GetType().GetProperties().Where(pi => DatabaseContext.SerializableTypes.Contains(pi.PropertyType.BaseType));
        }

        public static string ToLpFormat(this IList<Constraint> constraints, IList<Domain> domains)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Subject To");

            for (var i = 0; i < constraints.Count; i++)
            {
                sb.Append("\t");
                sb.AppendFormat("c{0}: ", i);

                for (var j = 0; j < constraints[i].TermsCoefficients.Length; j++)
                {
                    sb.AppendFormat("{0} x{1}", constraints[i].TermsCoefficients[j], j);
                    sb.Append(j == constraints[i].TermsCoefficients.Length - 1 ? " <= " : " + ");
                }

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

            sb.AppendLine("Generals");
            sb.Append("\t");

            for (var i = 0; i < domains.Count; i++)
            {
                sb.AppendFormat("x{0}", i);
                sb.Append(i != domains.Count - 1 ? " " : string.Empty);
            }

            sb.Append("\n");
            sb.Append("End");

            return sb.ToString();
        }

        public static string ToSimpleFormat(this IList<Constraint> constraints, IList<Domain> domains)
        {
            var sb = new StringBuilder();



            return sb.ToString();
        }

        public static string ToReadableString(this Statistics statistics)
        {
            var sb = new StringBuilder();
            var propertyInfos = statistics.GetDbSerializableProperties();

            foreach (var propertyInfo in propertyInfos)
            {
                sb.Append(propertyInfo.Name);
                sb.Append(" = ");
                
                if (propertyInfo.PropertyType == typeof(TimeSpan))
                {
                    var timeSpan = (TimeSpan) propertyInfo.GetValue(statistics, null);
                    sb.Append(timeSpan.TotalMilliseconds);
                    sb.Append(" [ms]");
                }
                else
                {
                    sb.Append(propertyInfo.GetValue(statistics, null));
                }

                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}
