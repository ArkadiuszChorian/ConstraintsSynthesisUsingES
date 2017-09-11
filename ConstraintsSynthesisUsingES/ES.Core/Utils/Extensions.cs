using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.Utils
{
    public static class Extensions
    {      
        public static string ToPrintableString<T>(this T objectToPrint)
        {
            var stringBuilder = new StringBuilder();

            var propertyInfos = GetPrintableProperties(objectToPrint);

            foreach (var propertyInfo in propertyInfos)
            {
                var valueToPrint = propertyInfo.GetValue(objectToPrint, null);

                if (propertyInfo.PropertyType.IsEnum)
                {
                    var numericValue = propertyInfo.GetValue(objectToPrint, null);
                    var stringValue = Enum.GetName(propertyInfo.PropertyType, numericValue);

                    valueToPrint = stringValue;
                }

                if (propertyInfo.PropertyType == typeof(TimeSpan))
                {
                    var timeSpan = propertyInfo.GetValue(objectToPrint, null);

                    valueToPrint = ((TimeSpan)timeSpan).Milliseconds + " [ms]";
                }

                stringBuilder.AppendFormat("{0} = {1}\n", propertyInfo.Name, valueToPrint);
            }

            return stringBuilder.ToString();
        }

        public static readonly Type[] TypesToPrint = {
            typeof(bool).BaseType,
            typeof(int).BaseType,
            typeof(long).BaseType,
            typeof(double).BaseType,
            typeof(decimal).BaseType,
            typeof(float).BaseType,
            typeof(Enum),
            typeof(TimeSpan)
        };

        public static IEnumerable<PropertyInfo> GetPrintableProperties<T>(T obj)
        {
            return obj.GetType().GetProperties().Where(pi => TypesToPrint.Contains(pi.PropertyType.BaseType));
        }

        public static string ToExponential(this double value)
        {
            return value.ToString("E1", CultureInfo.InvariantCulture);
        }

        public static StringBuilder AppendPrintable<T>(this StringBuilder stringBuilder, T objectToAppend)
        {
            var propertyInfos = GetPrintableProperties(objectToAppend);

            foreach (var propertyInfo in propertyInfos)
            {
                var valueToPrint = propertyInfo.GetValue(objectToAppend, null);

                if (propertyInfo.PropertyType.IsEnum)
                {
                    var numericValue = propertyInfo.GetValue(objectToAppend, null);
                    var stringValue = Enum.GetName(propertyInfo.PropertyType, numericValue);

                    valueToPrint = stringValue;
                }

                if (propertyInfo.PropertyType == typeof(TimeSpan))
                {
                    var timeSpan = propertyInfo.GetValue(objectToAppend, null);

                    valueToPrint = ((TimeSpan)timeSpan).Milliseconds + " [ms]";
                }

                stringBuilder.AppendFormat("{0} = {1}\n", propertyInfo.Name, valueToPrint);
            }

            return stringBuilder;
        }

        private const string GenerationsSeparator =      "===========================================";
        private const string SectionsSeparator =         "-------------------------------------------";
        private const string MutationStepsSeparator =    "###########################################";
        private const string CollectionsSeparator =      ":::::::::::::::::::::::::::::::::::::::::::";
        private const string IndexSeparator =            "#######################";

        public static StringBuilder AppendEvolutionSteps(this StringBuilder stringBuilder, IDictionary<int, EvolutionStep> evolutionSteps)
        {
            foreach (var evolutionStep in evolutionSteps)
            {
                stringBuilder.AppendLine(GenerationsSeparator);
                stringBuilder.AppendFormat("============ Generation {0:D4} ============\n", evolutionStep.Key);
                stringBuilder.AppendLine(SectionsSeparator);
                stringBuilder.AppendSolutions(evolutionStep.Value.BasePopulation, nameof(evolutionStep.Value.BasePopulation));
                stringBuilder.AppendLine(SectionsSeparator);
                stringBuilder.AppendMutationSteps(evolutionStep.Value.Mutations);
                stringBuilder.AppendLine(SectionsSeparator);
                stringBuilder.AppendSolutions(evolutionStep.Value.NewPopulation, nameof(evolutionStep.Value.NewPopulation));
            }

            return stringBuilder;
        }

        public static StringBuilder AppendMutationSteps(this StringBuilder stringBuilder, IList<MutationStep> mutationSteps)
        {
            var count = mutationSteps.Count;

            stringBuilder.AppendLine("--------- MutationSteps ---------");

            for (var i = 0; i < count; i++)
            {
                stringBuilder.AppendLine(MutationStepsSeparator);
                stringBuilder.AppendMutationStep(mutationSteps[i], i + 1);
            }
                
            return stringBuilder;
        }

        public static StringBuilder AppendMutationStep(this StringBuilder stringBuilder, MutationStep mutationStep, int? identifier = null)
        {
            if (identifier.HasValue)
                stringBuilder.AppendFormat("###### Mutation {0:D4} ######\n", identifier);
            
            stringBuilder.AppendSolution(mutationStep.Parent, $"Parent {mutationStep.ParentIndex:D4}");
            stringBuilder.AppendSolution(mutationStep.Parent, $"Offspring {identifier:D4}");

            return stringBuilder;
        }

        public static StringBuilder AppendSolutions(this StringBuilder stringBuilder, IList<Solution> solutions, string collectionName = null)
        {
            var count = solutions.Count;

            if (collectionName != null)
            {
                stringBuilder.AppendLine(CollectionsSeparator);
                stringBuilder.AppendLine($":::::: {collectionName} ::::::");
            }

            for (var i = 0; i < count; i++)
            {
                stringBuilder.AppendLine(IndexSeparator);
                stringBuilder.AppendSolution(solutions[i], i.ToString("D4"));
            }
                
            return stringBuilder;
        }

        public static StringBuilder AppendSolution(this StringBuilder stringBuilder, Solution solution, string identifier = null)
        {
            if (identifier != null)
                stringBuilder.AppendFormat("### {0} ###\n", identifier);

            stringBuilder.Append($"<{nameof(solution.FitnessScore)}>\n{solution.FitnessScore}\n");            
            stringBuilder.AppendArray(solution.ObjectCoefficients, nameof(solution.ObjectCoefficients));
            stringBuilder.AppendArray(solution.StdDeviationsCoefficients, nameof(solution.StdDeviationsCoefficients));
            stringBuilder.AppendArray(solution.RotationsCoefficients, nameof(solution.RotationsCoefficients));

            return stringBuilder;
        }       

        public static StringBuilder AppendArray(this StringBuilder stringBuilder, double[] coefficients, string coefficientsName = null)
        {
            var length = coefficients.Length;

            if (coefficientsName != null)
                stringBuilder.AppendLine($"<{coefficientsName}>");

            stringBuilder.Append("[");

            for (var i = 0; i < length; i++)
            {
                stringBuilder.Append(coefficients[i].ToExponential());
                stringBuilder.Append(i != length - 1 ? ";" : "]\n");
            }

            return stringBuilder;
        }
    }
}