using System;
using System.Collections.Generic;
using System.Text;
using CSUES.Engine.Models;
using ES.Core.Models;
using ES.Core.Utils;
using EvolutionStatistics = ES.Core.Models.Statistics;
using Statistics = CSUES.Engine.Models.Statistics;

namespace CSUES.Common
{
    public class Logger
    {
        private readonly StringBuilder _logBuilder;

        private static readonly Lazy<Logger> LazyInstance = new Lazy<Logger>(() => new Logger());
        public static Logger Instance => LazyInstance.Value;

        private Logger()
        {
            _logBuilder = new StringBuilder();
        }

        public string GetLogAsString()
        {
            return _logBuilder.ToString();
        }
        
        public void Log(EvolutionParameters evolutionParameters)
        {
            _logBuilder.AppendLine("=============== EVOLUTION PARAMETERS ===============");
            _logBuilder.AppendPrintable(evolutionParameters);
            _logBuilder.AppendLine("====================================================");
        }

        public void Log(ExperimentParameters experimentParameters, bool withEvolutionParameters = true)
        {
            if (withEvolutionParameters)
                Log(experimentParameters.EvolutionParameters);

            _logBuilder.AppendLine("============== EXPERIMENT PARAMETERS ===============");
            _logBuilder.AppendPrintable(experimentParameters);
            _logBuilder.AppendLine("====================================================");     
        }

        public void Log(IDictionary<int, EvolutionStep> evolutionSteps, bool withHeader = false)
        {    
            if (withHeader)        
                _logBuilder.AppendLine("================= EVOLUTION STEPS ==================");

            _logBuilder.AppendEvolutionSteps(evolutionSteps);

            if (withHeader)
                _logBuilder.AppendLine("====================================================");
        }

        public void Log(MathModel mathModel)
        {
            _logBuilder.AppendLine("================= REFERENCE MODEL ==================");
            _logBuilder.AppendLine(mathModel.ReferenceModelInLpFormat);
            _logBuilder.AppendLine("====================================================");
            _logBuilder.AppendLine("================ SYNTHESIZED MODEL =================");
            _logBuilder.AppendLine(mathModel.SynthesizedModelInLpFormat);
            _logBuilder.AppendLine("====================================================");
        }

        public void Log(Statistics statistics)
        {
            _logBuilder.AppendLine("=================== STATISTICS =====================");
            _logBuilder.AppendPrintable(statistics.EvolutionStatistics);
            _logBuilder.AppendPrintable(statistics);
            _logBuilder.AppendLine("====================================================");
        }
    }
}
