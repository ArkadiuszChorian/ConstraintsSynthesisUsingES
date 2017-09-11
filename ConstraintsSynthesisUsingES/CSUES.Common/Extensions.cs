using System.Text;
using CSUES.Engine.Models;

namespace CSUES.Common
{
    public static class Extensions
    {
        public static string GetFileName(this ExperimentParameters experimentParameters, string prefix = null, string extension = null)
        {
            var sb = new StringBuilder();

            if (prefix != null)
                sb.Append(prefix).Append("_");

            sb.Append(experimentParameters.TypeOfBenchmark).Append("_");
            sb.Append(experimentParameters.Seed).Append("_");
            sb.Append(experimentParameters.NumberOfDimensions).Append("_");
            sb.Append(experimentParameters.EvolutionParameters.NumberOfGenerations).Append("_");
            sb.Append(experimentParameters.EvolutionParameters.OffspringPopulationSize).Append("_");
            sb.Append(experimentParameters.NumberOfPositivePoints).Append("_");
            sb.Append(experimentParameters.NumberOfNegativePoints).Append("_");
            sb.Append(experimentParameters.AllowQuadraticTerms).Append("_");
            sb.Append(experimentParameters.UseDataNormalization).Append("_");
            sb.Append(experimentParameters.UseSeeding);

            if (extension != null)
                sb.Append(extension);

            return sb.ToString();
        }
    }
}