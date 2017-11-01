using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CSUES.Common;
using CSUES.Engine.Enums;
using CSUES.Engine.Models;
using ES.Core.Models;
using ExperimentDatabase;
using Microsoft.Data.Sqlite;
using EvolutionStatistics = ES.Core.Models.Statistics;
using CSUSESStatistics = CSUES.Engine.Models.Statistics;

namespace CSUES.ResultsAnalyzer
{
    class Program
    {
        private static readonly string DatabaseDirPath = $"../../../Database/{DatabaseContext.DbFilename}";
        private static readonly string DatabaseFullPath = Path.GetFullPath(DatabaseDirPath);
        private static SqliteDataReader Reader;
        private static List<Experiment> Experiments = new List<Experiment>();

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            using (var database = new DatabaseEngine(DatabaseFullPath))
            {
                using (var reader = database.PrepareStatement(GetExperimentsData).ExecuteReader())
                {
                    Reader = reader;

                    while (reader.Read())
                    {
                        var evolutionParameters = GetEvolutionParameters();
                        var experimentParameters = GetExperimentParameters(evolutionParameters);
                        var evolutionStatistics = GetEvolutionStatistics();
                        var statistics = GetStatistics(evolutionStatistics);
                        var mathModel = GetMathModel();

                        var experiment = new Experiment()
                        {
                            ExperimentParameters = experimentParameters,
                            Statistics = statistics,
                            MathModel = mathModel
                        };

                        Experiments.Add(experiment);
                    }
                }               
            }

            TuningTable();
            //AnalysisTable();

            Console.WriteLine("DONE!");
            Console.ReadKey();
        }

        private static void AnalysisTable()
        {
            var groupedExperimentsCollection = Experiments
                .Where(e => e.ExperimentParameters.EvolutionParameters.NumberOfGenerations == 100)
                .Where(e => e.ExperimentParameters.EvolutionParameters.OffspringPopulationSize == 1000)
                .Where(e => (e.ExperimentParameters.AllowQuadraticTerms && e.ExperimentParameters.UseSeeding && !e.ExperimentParameters.UseDataNormalization)
                            || (!e.ExperimentParameters.AllowQuadraticTerms && !e.ExperimentParameters.UseSeeding && e.ExperimentParameters.UseDataNormalization))
                .Where(e => e.ExperimentParameters.NumberOfPositivePoints == 500)
                .GroupBy(e => new
                {
                    e.ExperimentParameters.NumberOfDimensions,                   
                    e.ExperimentParameters.AllowQuadraticTerms,                 
                    e.ExperimentParameters.TypeOfBenchmark,

                    e.ExperimentParameters.EvolutionParameters.NumberOfGenerations,
                    e.ExperimentParameters.EvolutionParameters.OffspringPopulationSize,
                    e.ExperimentParameters.UseSeeding,
                    e.ExperimentParameters.UseDataNormalization,

                    e.ExperimentParameters.NumberOfPositivePoints,
                });

            var averagedExperiments = GetAveragedExperiments(groupedExperimentsCollection);

            SaveToFile("analysisTable.tex", CreateAnalysisTable(averagedExperiments));
        }
        
        private static void TuningTable()
        {
            var groupedExperimentsCollection = Experiments
                .GroupBy(e => new
                {
                    e.ExperimentParameters.NumberOfDimensions,
                    e.ExperimentParameters.UseRedundantConstraintsRemoving,
                    e.ExperimentParameters.UseDataNormalization,
                    e.ExperimentParameters.AllowQuadraticTerms,
                    e.ExperimentParameters.UseSeeding,
                    e.ExperimentParameters.TypeOfBenchmark,

                    e.ExperimentParameters.EvolutionParameters.BasePopulationSize,
                    e.ExperimentParameters.EvolutionParameters.OffspringPopulationSize,
                    e.ExperimentParameters.EvolutionParameters.NumberOfGenerations,

                    //e.ExperimentParameters.MaximumNumberOfConstraints,
                    //e.ExperimentParameters.NumberOfConstraintsCoefficients,
                    //e.ExperimentParameters.TrackEvolutionSteps,
                    //e.ExperimentParameters.BallnBoundaryValue,
                    //e.ExperimentParameters.CubenBoundaryValue,
                    //e.ExperimentParameters.SimplexnBoundaryValue,
                    //e.ExperimentParameters.NumberOfDomainSamples,
                    //e.ExperimentParameters.NumberOfTestPoints,
                    //e.ExperimentParameters.NumberOfPositivePoints,
                    //e.ExperimentParameters.NumberOfNegativePoints,
                    //e.ExperimentParameters.DefaultDomainLowerLimit,
                    //e.ExperimentParameters.DefaultDomainUpperLimit,
                    //e.ExperimentParameters.MaxNumberOfPointsInSingleArray,

                    //e.ExperimentParameters.EvolutionParameters.ObjectVectorSize,
                    //e.ExperimentParameters.EvolutionParameters.OneFifthRuleCheckInterval,
                    //e.ExperimentParameters.EvolutionParameters.OneFifthRuleScalingFactor,
                    //e.ExperimentParameters.EvolutionParameters.NumberOfParentsSolutionsToSelect,
                    //e.ExperimentParameters.EvolutionParameters.TypeOfParentsSelection,
                    //e.ExperimentParameters.EvolutionParameters.TypeOfSurvivorsSelection,
                    //e.ExperimentParameters.EvolutionParameters.GlobalLearningRate,
                    //e.ExperimentParameters.EvolutionParameters.IndividualLearningRate,
                    //e.ExperimentParameters.EvolutionParameters.StepThreshold,
                    //e.ExperimentParameters.EvolutionParameters.RotationAngle,
                    //e.ExperimentParameters.EvolutionParameters.TypeOfMutation,
                    //e.ExperimentParameters.EvolutionParameters.UseRecombination,
                    //e.ExperimentParameters.EvolutionParameters.TypeOfObjectsRecombination,
                    //e.ExperimentParameters.EvolutionParameters.TypeOfStdDeviationsRecombination,
                    //e.ExperimentParameters.EvolutionParameters.TypeOfRotationsRecombination,
                });

            var averagedExperiments = new List<Experiment>();

            foreach (var groupedExperiments in groupedExperimentsCollection)
            {
                var parameters = groupedExperiments.First().ExperimentParameters;
                var evolutionStatistics = new EvolutionStatistics
                {
                    BestFitnessScore = groupedExperiments.Average(g => g.Statistics.EvolutionStatistics.BestFitnessScore),
                    LastFitnessScore = groupedExperiments.Average(g => g.Statistics.EvolutionStatistics.LastFitnessScore),
                    NumberOfGenerationBestSolutionTakenFrom = (int)Math.Round(groupedExperiments.Average(g => g.Statistics.EvolutionStatistics.NumberOfGenerationBestSolutionTakenFrom))
                };
                var statistics = new Statistics()
                {
                    Accuracy = groupedExperiments.Average(g => g.Statistics.Accuracy),
                    EvolutionStatistics = evolutionStatistics,
                    F1Score = groupedExperiments.Average(g => g.Statistics.F1Score),
                    FallOut = groupedExperiments.Average(g => g.Statistics.FallOut),
                    FalseDiscoveryRate = groupedExperiments.Average(g => g.Statistics.FalseDiscoveryRate),
                    FalseNegatives = (int)Math.Round(groupedExperiments.Average(g => g.Statistics.FalseNegatives)),
                    FalseOmissionRate = groupedExperiments.Average(g => g.Statistics.FalseOmissionRate),
                    FalsePositives = (int)Math.Round(groupedExperiments.Average(g => g.Statistics.FalsePositives)),
                    JaccardIndex = groupedExperiments.Average(g => g.Statistics.JaccardIndex),
                    MeanAngle = groupedExperiments.Average(g => g.Statistics.MeanAngle),
                    MissRate = groupedExperiments.Average(g => g.Statistics.MissRate),
                    //ModelEvaluationTime = modelEvaluationTime,
                    NegativePredictiveValue = groupedExperiments.Average(g => g.Statistics.NegativePredictiveValue),
                    //NegativeTrainingPointsGenerationTime = negativeTrainingPointsGenerationTime,
                    NumberOfConstraints = (int)Math.Round(groupedExperiments.Average(g => g.Statistics.NumberOfConstraints)),
                    //PositiveTrainingPointsGenerationTime = positiveTrainingPointsGenerationTime,
                    Precision = groupedExperiments.Average(g => g.Statistics.Precision),
                    Recall = groupedExperiments.Average(g => g.Statistics.Recall),
                    //RedundantConstraintsRemovingTime = redundantConstraintsRemovingTime,
                    Specificity = groupedExperiments.Average(g => g.Statistics.Specificity),
                    //TestPointsGenerationTime = testPointsGenerationTime,
                    //TotalSynthesisTime = totalSynthesisTime,
                    TrueNegatives = (int)Math.Round(groupedExperiments.Average(g => g.Statistics.TrueNegatives)),
                    TruePositives = (int)Math.Round(groupedExperiments.Average(g => g.Statistics.TruePositives)),
                };

                averagedExperiments.Add(new Experiment
                {
                    ExperimentParameters = parameters,
                    Statistics = statistics
                });
            }

            SaveToFile("tuningTable.tex", CreateTuningTable(averagedExperiments));
        }

        private static List<Experiment> GetAveragedExperiments(IEnumerable<IGrouping<dynamic, Experiment>> groupedExperimentsCollection)
        {
            var averagedExperiments = new List<Experiment>();

            foreach (var groupedExperiments in groupedExperimentsCollection)
            {
                var parameters = groupedExperiments.First().ExperimentParameters;
                var evolutionStatistics = new EvolutionStatistics
                {
                    BestFitnessScore = groupedExperiments.Average(g => g.Statistics.EvolutionStatistics.BestFitnessScore),
                    LastFitnessScore = groupedExperiments.Average(g => g.Statistics.EvolutionStatistics.LastFitnessScore),
                    NumberOfGenerationBestSolutionTakenFrom = (int)Math.Round(groupedExperiments.Average(g => g.Statistics.EvolutionStatistics.NumberOfGenerationBestSolutionTakenFrom))
                };
                var statistics = new Statistics()
                {
                    Accuracy = groupedExperiments.Average(g => g.Statistics.Accuracy),
                    EvolutionStatistics = evolutionStatistics,
                    F1Score = groupedExperiments.Average(g => g.Statistics.F1Score),
                    FallOut = groupedExperiments.Average(g => g.Statistics.FallOut),
                    FalseDiscoveryRate = groupedExperiments.Average(g => g.Statistics.FalseDiscoveryRate),
                    FalseNegatives = (int)Math.Round(groupedExperiments.Average(g => g.Statistics.FalseNegatives)),
                    FalseOmissionRate = groupedExperiments.Average(g => g.Statistics.FalseOmissionRate),
                    FalsePositives = (int)Math.Round(groupedExperiments.Average(g => g.Statistics.FalsePositives)),
                    JaccardIndex = groupedExperiments.Average(g => g.Statistics.JaccardIndex),
                    MeanAngle = groupedExperiments.Average(g => g.Statistics.MeanAngle),
                    MissRate = groupedExperiments.Average(g => g.Statistics.MissRate),
                    //ModelEvaluationTime = modelEvaluationTime,
                    NegativePredictiveValue = groupedExperiments.Average(g => g.Statistics.NegativePredictiveValue),
                    //NegativeTrainingPointsGenerationTime = negativeTrainingPointsGenerationTime,
                    NumberOfConstraints = (int)Math.Round(groupedExperiments.Average(g => g.Statistics.NumberOfConstraints)),
                    //PositiveTrainingPointsGenerationTime = positiveTrainingPointsGenerationTime,
                    Precision = groupedExperiments.Average(g => g.Statistics.Precision),
                    Recall = groupedExperiments.Average(g => g.Statistics.Recall),
                    //RedundantConstraintsRemovingTime = redundantConstraintsRemovingTime,
                    Specificity = groupedExperiments.Average(g => g.Statistics.Specificity),
                    //TestPointsGenerationTime = testPointsGenerationTime,
                    //TotalSynthesisTime = totalSynthesisTime,
                    TrueNegatives = (int)Math.Round(groupedExperiments.Average(g => g.Statistics.TrueNegatives)),
                    TruePositives = (int)Math.Round(groupedExperiments.Average(g => g.Statistics.TruePositives)),
                };

                averagedExperiments.Add(new Experiment
                {
                    ExperimentParameters = parameters,
                    Statistics = statistics
                });
            }

            return averagedExperiments;
        }

        private static string CreateAnalysisTable(List<Experiment> experiments)
        {
            var groupedOrderedExperiments = experiments
                .OrderBy(e => e.ExperimentParameters.NumberOfDimensions)
                .GroupBy(e => new
                {
                    e.ExperimentParameters.TypeOfBenchmark,
                    e.ExperimentParameters.AllowQuadraticTerms
                })
                .OrderBy(g => g.Key.TypeOfBenchmark)
                .ThenBy(g => g.Key.AllowQuadraticTerms)
                .ToList();

            var sb = new StringBuilder();

            sb.Append("\\begin{table}[h]\n" +
                      "\\centering\n" +
                      "\\footnotesize\n" +
                      "\\label{tableAnalysis}\n" +
                      "\\caption{Analiza statystyczna jakości syntezy " +
                      "dla najlepszej konfiguracji parametrów dla danego problemu}\n" +
                      "\\makebox[\\textwidth][c]{\n" +
                      "\\begin{tabular}{|c|c|}\n");

            sb.Append("\\hline \n" +
                      "Simplex$n$ -- modele \\emph{LP} & Cube$n$ -- modele \\emph{LP} \\\\ \n" +
                      "\\Xhline{3\\arrayrulewidth}\n");

            sb.Append("\\begin{tabular}{c|cccc} \n" +
                      "$n$ & 2 & 3 & 4 & 5 \\\\ " +
                      "\\hline \n" +
                      "\\hline \n" +
                      GetTabularContent(groupedOrderedExperiments[0].ToList()) +
                      "\\end{tabular} \n");

            sb.Append("& \n");

            sb.Append("\\begin{tabular}{c|cccc} \n" +
                      "$n$ & 2 & 3 & 4 & 5 \\\\ " +
                      "\\hline \n" +
                      "\\hline \n" +
                      GetTabularContent(groupedOrderedExperiments[1].ToList()) +
                      "\\end{tabular} \n");

            sb.Append("\\\\ \n");

            sb.Append("\\hline \n" +
                      "\\hline \n" +
                      "Ball$n$ -- modele \\emph{LP} & Ball$n$ -- modele \\emph{QCQP} \\\\ \n" +
                      "\\Xhline{3\\arrayrulewidth}\n");

            sb.Append("\\begin{tabular}{c|cccc} \n" +
                      "$n$ & 2 & 3 & 4 & 5 \\\\ " +
                      "\\hline \n" +
                      "\\hline \n" +
                      GetTabularContent(groupedOrderedExperiments[2].ToList()) +
                      "\\end{tabular} \n");

            sb.Append("& \n");

            sb.Append("\\begin{tabular}{c|cccc} \n" +
                      "$n$ & 2 & 3 & 4 & 5 \\\\ " +
                      "\\hline \n" +
                      "\\hline \n" +
                      GetTabularContent(groupedOrderedExperiments[3].ToList()) +
                      "\\end{tabular}");

            sb.Append(" \\\\ \\hline \n");
            sb.Append("\\end{tabular} \n");
            sb.Append("} \n");
            sb.Append("\\end{table} \n");

            return sb.ToString();
        }

        private static string GetTabularContent(List<Experiment> experiments)
        {
            var sb = new StringBuilder();
            var count = experiments.Count;
            var last = count - 1;

            sb.Append("$J_{TR}$ & ");
            for (var i = 0; i < count; i++)
                sb.Append(GetCell(experiments[i].Statistics.EvolutionStatistics.BestFitnessScore, true, i == last));

            sb.Append("$J_{TS}$ & ");
            for (var i = 0; i < count; i++)
                sb.Append(GetCell(experiments[i].Statistics.JaccardIndex, true, i == last));

            sb.Append("$p$ & ");
            for (var i = 0; i < count; i++)
                sb.Append(GetCell(experiments[i].Statistics.Precision, true, i == last));

            sb.Append("$r$ & ");
            for (var i = 0; i < count; i++)
                sb.Append(GetCell(experiments[i].Statistics.Recall, true, i == last));

            sb.Append("$F_1$ & ");
            for (var i = 0; i < count; i++)
                sb.Append(GetCell(experiments[i].Statistics.F1Score, true, i == last, 2));

            if (experiments.First().ExperimentParameters.TypeOfBenchmark != BenchmarkType.Balln)
            {
                sb.Append("$\\bar{\\gamma}$ & ");
                for (var i = 0; i < count; i++)
                    sb.Append(GetCell(experiments[i].Statistics.MeanAngle, false, i == last));
            }
         
            sb.Append("$|C|$ & ");
            for (var i = 0; i < count; i++)
                sb.Append(GetCell(experiments[i].Statistics.NumberOfConstraints, false, i == last, numberOfHorizontalLines: 0, withDecimalPlaces: false));        

            return sb.ToString();
        }

        private static string GetCell(double value, bool withColour, bool withRowEnding, int numberOfHorizontalLines = 1, bool withDecimalPlaces = true)
        {
            var sb = new StringBuilder();
            var rounded = Math.Round(value, 2);
            var cellValue = withDecimalPlaces ? rounded.ToString("F2") : ((int)rounded).ToString();
            var colorValue = double.IsNaN(rounded) ? 0 : (int) (rounded * 100);

            sb.Append(withColour
                    ? @"\cca{" + $"{colorValue}" + "}{" + $"{cellValue}" + "}"
                    : $"{cellValue}");

            if (withRowEnding)
            {
                sb.Append(" \\\\");

                for (var i = 0; i < numberOfHorizontalLines; i++)
                    sb.Append(" \\hline");

                sb.Append(" \n");
            }
            else
            {
                sb.Append(" & ");
            }

            return sb.ToString();
        }

        private static string CreateTuningTable(List<Experiment> experiments)
        {
            var groupedOrderedExperiments = experiments
                .OrderBy(e => e.ExperimentParameters.TypeOfBenchmark)
                .ThenBy(e => e.ExperimentParameters.AllowQuadraticTerms)
                .ThenBy(e => e.ExperimentParameters.NumberOfDimensions)
                .ThenByDescending(e => e.Statistics.JaccardIndex)
                .GroupBy(e => new
                {
                    e.ExperimentParameters.EvolutionParameters.OffspringPopulationSize,
                    e.ExperimentParameters.EvolutionParameters.NumberOfGenerations,
                    e.ExperimentParameters.UseSeeding,
                    e.ExperimentParameters.UseDataNormalization
                })
                .OrderByDescending(g => g.Key.OffspringPopulationSize)
                .ThenBy(g => g.Key.NumberOfGenerations)
                .ThenBy(g => g.Key.UseSeeding)
                .ThenByDescending(g => g.Key.UseDataNormalization);

            var maxJaccards = experiments
                .OrderBy(e => e.ExperimentParameters.TypeOfBenchmark)
                .ThenBy(e => e.ExperimentParameters.AllowQuadraticTerms)
                .ThenBy(e => e.ExperimentParameters.NumberOfDimensions)
                .ThenByDescending(e => e.Statistics.JaccardIndex)
                .GroupBy(e => new
                {
                    e.ExperimentParameters.TypeOfBenchmark,
                    e.ExperimentParameters.AllowQuadraticTerms,
                    e.ExperimentParameters.NumberOfDimensions
                })
                .Select(g => g.First().Statistics.JaccardIndex)
                .ToList();

            var sb = new StringBuilder();

            sb.Append("\\begin{table}\n" +
                      "\\centering\n" +
                      "\\footnotesize\n" +
                      "\\label{tableSetup}\n" +
                      "\\caption{Wyniki strojenia algorytmu}\n" +
                      "\\makebox[\\textwidth][c]{\n" +
                      "\\setlength{\\tabcolsep}{2.75pt}\n" +
                      "\\begin{tabular}{|cccc||cccc|cccc|cccc||cccc|}\n" +
                      "\\cline{5-20}\n");

            sb.Append("\\multicolumn{4}{c|}{} & \n" +
                      "\\multicolumn{12}{c||}{Modele \\emph{LP}} &\n" +
                      "\\multicolumn{4}{c|}{Modele \\emph{QCQP}} \\\\ \n" +
                      "\\hline\n");

            sb.Append("\\multicolumn{4}{|c||}{Parametry} & \n" +
                      "\\multicolumn{4}{c|}{S$n$ -- Simplex$n$} &\n" +
                      "\\multicolumn{4}{c|}{C$n$ -- Cube$n$} &\n" +
                      "\\multicolumn{4}{c||}{B$n$ -- Ball$n$} &\n" +
                      "\\multicolumn{4}{c|}{B$n$ -- Ball$n$} \\\\ \n" +
                      "\\Xhline{3\\arrayrulewidth}\n");
                      //"\\hline\n");

            //sb.Append("Parametry & ");
            //sb.Append(GetRow(true,"S2","S3","S4","S5","C2","C3","C4","C5","B2L", "B3L", "B4L", "B5L", "B2Q", "B3Q", "B4Q", "B5Q"));
            sb.Append(GetHeaderRow(true, @"$\lambda$", "$g$", "$\\Omega$", "$\\Phi$", "S2","S3","S4","S5","C2","C3","C4","C5","B2", "B3", "B4", "B5", "B2", "B3", "B4", "B5"));

            foreach (var orderedExperimentsGrouping in groupedOrderedExperiments)
            {
                //var parameters = GetParamCell(orderedExperimentsGrouping.First().ExperimentParameters);
                //sb.Append(parameters);
                //sb.Append(" & ");               

                var parameters = GetParametersCellsContent(orderedExperimentsGrouping.First().ExperimentParameters).ToList();
                var values = orderedExperimentsGrouping.Select(experiment => experiment.Statistics.JaccardIndex).ToList();
                sb.Append(GetRow(true, maxJaccards, parameters, values));
            }

            sb.Append("\\hline\n" +
                      //"\\multicolumn{20}{|l|}{$\\Omega$ -- wykorzystanie algorytmu poprawiającego jakość początkowej populacji} \\\\ \n" +
                      //"\\multicolumn{20}{|l|}{$\\Phi$ -- wykorzystanie standaryzowanego zbioru uczącego} \\\\ \n" +
                      //"\\multicolumn{20}{|l|}{Kolumna 'Termy $T_L$' to wyniki dla modeli syntezowanych z wykorzystaniem zbioru termów $T_L$.} \\\\ \n" +
                      //"\\multicolumn{20}{|l|}{Kolumna 'Termy $T_Q$' to wyniki dla modeli syntezowanych z wykorzystaniem zbioru termów $T_Q$.} \\\\ \n" +
                      "\\multicolumn{20}{|l|}{Komórki zielone -- najwyższa wartość Indeksu Jaccard'a dla danego problemu na zbiorze testowym} \\\\ \n" +
                      "\\hline\n");

            sb.Append("\\end{tabular}\n" +
                      "}\n" +                    
                      "\\end{table}\n");

            return sb.ToString();
        }

        //private static Dictionary<int, bool> HasBeenColoured = new Dictionary<int, bool>();
        private static string GetRow(bool withHorizontalLine, List<double> jaccardIndexes, List<string> parameters, List<double> values)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < parameters.Count; i++)
            {
                sb.Append($"{parameters[i]}");
                sb.Append(@"&");
            }

            for (var i = 0; i < values.Count; i++)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (Math.Round(values[i], 2) == Math.Round(jaccardIndexes[i], 2) && Math.Round(values[i], 2) > 0.0)
                    sb.Append(@"\bc{" + $"{values[i]:F2}" + "}");
                else
                    sb.Append($"{values[i]:F2}");
                
                if (i == values.Count - 1)
                    sb.Append(@" \\");
                else
                    sb.Append(@"&");
            }

            if (withHorizontalLine)
                sb.Append(@" \hline");

            sb.Append("\n");

            return sb.ToString();
        }

        private static string GetHeaderRow(bool withHorizontalLine, params string[] values)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < values.Length; i++)
            {
                sb.Append($"{values[i]}");

                if (i == values.Length - 1)
                    sb.Append(@" \\");
                else
                    sb.Append(@"&");
            }

            if (withHorizontalLine)
                sb.Append(@" \hline \hline");

            sb.Append("\n");

            return sb.ToString();
        }

        private static IEnumerable<string> GetParametersCellsContent(ExperimentParameters experimentParameters)
        {
            yield return $"{experimentParameters.EvolutionParameters.OffspringPopulationSize,4}";
            yield return $"{experimentParameters.EvolutionParameters.NumberOfGenerations,4}";
            yield return experimentParameters.UseSeeding ? @"\cm" : "   ";
            yield return experimentParameters.UseDataNormalization ? @"\cm" : "   ";
        }

        private static string GetParamCell(ExperimentParameters experimentParameters)
        {
            var sb = new StringBuilder();
            sb.Append($"{experimentParameters.EvolutionParameters.OffspringPopulationSize,4}");
            sb.Append(" ");
            sb.Append($"{experimentParameters.EvolutionParameters.NumberOfGenerations,4}");
            sb.Append(" ");
            sb.Append(experimentParameters.UseSeeding ? "S" : " ");
            sb.Append(experimentParameters.UseDataNormalization ? "N" : " ");
            return sb.ToString();
        }

        private static string SerializeToString(List<Experiment> experiments)
        {
            var orderedExperiments = experiments
                .OrderBy(e => e.ExperimentParameters.TypeOfBenchmark)
                .ThenBy(e => e.ExperimentParameters.NumberOfDimensions)
                .ThenBy(e => e.ExperimentParameters.EvolutionParameters.OffspringPopulationSize)
                .ThenBy(e => e.ExperimentParameters.EvolutionParameters.NumberOfGenerations)
                .ThenBy(e => e.ExperimentParameters.AllowQuadraticTerms)
                .ThenBy(e => e.ExperimentParameters.UseRedundantConstraintsRemoving)
                .ThenBy(e => e.ExperimentParameters.UseSeeding)
                .ThenBy(e => e.ExperimentParameters.UseDataNormalization);

            var sb = new StringBuilder();

            sb.Append("Benchmark;");
            sb.Append("Dimensions;");
            sb.Append("OffspringPopSize;");
            sb.Append("GenerationsNumber;");
            sb.Append("AllowQuadraticTerms;");
            sb.Append("UseRedundantConstraintsRemoving;");
            sb.Append("UseSeeding;");
            sb.Append("UseDataNormalization;");

            sb.Append("ConstraintsNumber;");
            sb.Append("NonZeroTermsNumber;");
            sb.Append("JaccardTrainingSet;");
            sb.Append("JaccardTestSet;");
            sb.Append("Precision;");
            sb.Append("Recall;");
            sb.Append("F1Score;");
            sb.Append("MeanAngle;");

            sb.Append("LastFitnessScore;");
            sb.Append("NumberOfGenerationBestSolutionTakenFrom;");
            sb.Append("TruePositives;");
            sb.Append("FalsePositives;");
            sb.Append("TrueNegatives;");
            sb.Append("FalseNegatives;");

            sb.Append("Accuracy;");
            sb.Append("FallOut;");
            sb.Append("FalseDiscoveryRate;");
            sb.Append("FalseOmissionRate;");
            sb.Append("MissRate;");
            sb.Append("NegativePredictiveValue;");
            sb.Append("Specificity\n");

            foreach (var experiment in orderedExperiments)
            {
                sb.Append(experiment.ExperimentParameters.TypeOfBenchmark).Append(";");
                sb.Append(experiment.ExperimentParameters.NumberOfDimensions).Append(";");
                sb.Append(experiment.ExperimentParameters.EvolutionParameters.OffspringPopulationSize).Append(";");
                sb.Append(experiment.ExperimentParameters.EvolutionParameters.NumberOfGenerations).Append(";");
                sb.Append(experiment.ExperimentParameters.AllowQuadraticTerms).Append(";");
                sb.Append(experiment.ExperimentParameters.UseRedundantConstraintsRemoving).Append(";");
                sb.Append(experiment.ExperimentParameters.UseSeeding).Append(";");
                sb.Append(experiment.ExperimentParameters.UseDataNormalization).Append(";");

                sb.Append(experiment.Statistics.NumberOfConstraints).Append(";");
                sb.Append("---").Append(";");
                sb.Append(experiment.Statistics.EvolutionStatistics.BestFitnessScore).Append(";");
                sb.Append(experiment.Statistics.JaccardIndex).Append(";");
                sb.Append(experiment.Statistics.Precision).Append(";");
                sb.Append(experiment.Statistics.Recall).Append(";");
                sb.Append(experiment.Statistics.F1Score).Append(";");
                sb.Append(experiment.Statistics.MeanAngle).Append(";");

                sb.Append(experiment.Statistics.EvolutionStatistics.LastFitnessScore).Append(";");
                sb.Append(experiment.Statistics.EvolutionStatistics.NumberOfGenerationBestSolutionTakenFrom).Append(";");
                sb.Append(experiment.Statistics.TruePositives).Append(";");
                sb.Append(experiment.Statistics.FalsePositives).Append(";");
                sb.Append(experiment.Statistics.TrueNegatives).Append(";");
                sb.Append(experiment.Statistics.FalseNegatives).Append(";");

                sb.Append(experiment.Statistics.Accuracy).Append(";");
                sb.Append(experiment.Statistics.FallOut).Append(";");
                sb.Append(experiment.Statistics.FalseDiscoveryRate).Append(";");
                sb.Append(experiment.Statistics.FalseOmissionRate).Append(";");
                sb.Append(experiment.Statistics.MissRate).Append(";");
                sb.Append(experiment.Statistics.NegativePredictiveValue).Append(";");
                sb.Append(experiment.Statistics.Specificity).Append("\n");
            }

            return sb.ToString();
        }

        private static void SaveToFile(string fileName, string content)
        {
            var path = Path.GetFullPath($"../../../{fileName}");
            File.WriteAllText(path, content);            
        }

        private static MathModel GetMathModel()
        {
            return new MathModel()
            {
                SynthesizedModelInLpFormat = GetString(nameof(MathModel.SynthesizedModelInLpFormat)),
                SynthesizedModelInLpFormatSimplified = GetString(nameof(MathModel.SynthesizedModelInLpFormatSimplified)),
                ReferenceModelInLpFormat = GetString(nameof(MathModel.ReferenceModelInLpFormat)),
                ReferenceModelInLpFormatSimplified = GetString(nameof(MathModel.ReferenceModelInLpFormatSimplified))
            };
        }

        private static CSUSESStatistics GetStatistics(EvolutionStatistics evolutionStatistics)
        {
            return new CSUSESStatistics()
            {
                NumberOfConstraints = GetInt(nameof(CSUSESStatistics.NumberOfConstraints)),
                MeanAngle = GetDouble(nameof(CSUSESStatistics.MeanAngle)),
                TruePositives = GetInt(nameof(CSUSESStatistics.TruePositives)),
                FalsePositives = GetInt(nameof(CSUSESStatistics.FalsePositives)),
                TrueNegatives = GetInt(nameof(CSUSESStatistics.TrueNegatives)),
                FalseNegatives = GetInt(nameof(CSUSESStatistics.FalseNegatives)),
                TotalSynthesisTime = TimeSpan.FromMilliseconds(GetInt(nameof(CSUSESStatistics.TotalSynthesisTime))),
                RedundantConstraintsRemovingTime = TimeSpan.FromMilliseconds(GetInt(nameof(CSUSESStatistics.RedundantConstraintsRemovingTime))),
                ModelEvaluationTime = TimeSpan.FromMilliseconds(GetInt(nameof(CSUSESStatistics.ModelEvaluationTime))),
                PositiveTrainingPointsGenerationTime = TimeSpan.FromMilliseconds(GetInt(nameof(CSUSESStatistics.PositiveTrainingPointsGenerationTime))),
                NegativeTrainingPointsGenerationTime = TimeSpan.FromMilliseconds(GetInt(nameof(CSUSESStatistics.NegativeTrainingPointsGenerationTime))),
                TestPointsGenerationTime = TimeSpan.FromMilliseconds(GetInt(nameof(CSUSESStatistics.TestPointsGenerationTime))),
                EvolutionStatistics = evolutionStatistics
            };
        }

        private static EvolutionStatistics GetEvolutionStatistics()
        {
            return new EvolutionStatistics()
            {
                BestFitnessScore = GetDouble(nameof(EvolutionStatistics.BestFitnessScore)),
                LastFitnessScore = GetDouble(nameof(EvolutionStatistics.LastFitnessScore)),
                NumberOfGenerationBestSolutionTakenFrom = GetInt(nameof(EvolutionStatistics.NumberOfGenerationBestSolutionTakenFrom)),
                TotalEvolutionTime = TimeSpan.FromMilliseconds(GetInt(nameof(EvolutionStatistics.TotalEvolutionTime))),
                MeanSingleGenerationEvolutionTime = TimeSpan.FromMilliseconds(GetInt(nameof(EvolutionStatistics.MeanSingleGenerationEvolutionTime))),
                SeedingTime = TimeSpan.FromMilliseconds(GetInt(nameof(EvolutionStatistics.SeedingTime)))
            };
        }

        private static ExperimentParameters GetExperimentParameters(EvolutionParameters evolutionParameters)
        {
            return new ExperimentParameters(
                numberOfDimensions: GetInt(nameof(ExperimentParameters.NumberOfDimensions)),
                maximumNumberOfConstraints: GetInt(nameof(ExperimentParameters.MaximumNumberOfConstraints)),
                numberOfConstraintsCoefficients: GetInt(nameof(ExperimentParameters.NumberOfConstraintsCoefficients)),
                evolutionParameters: evolutionParameters,
                seed: GetInt(nameof(ExperimentParameters.Seed)),
                trackEvolutionSteps: GetBool(nameof(ExperimentParameters.TrackEvolutionSteps)),
                useRedundantConstraintsRemoving: GetBool(nameof(ExperimentParameters.UseRedundantConstraintsRemoving)),
                useDataNormalization: GetBool(nameof(ExperimentParameters.UseDataNormalization)),
                allowQuadraticTerms: GetBool(nameof(ExperimentParameters.AllowQuadraticTerms)),
                useSeeding: GetBool(nameof(ExperimentParameters.UseSeeding)),
                typeOfBenchmark: GetEnum<BenchmarkType>(nameof(ExperimentParameters.TypeOfBenchmark)),
                ballnBoundaryValue: GetDouble(nameof(ExperimentParameters.BallnBoundaryValue)),
                cubenBoundaryValue: GetDouble(nameof(ExperimentParameters.CubenBoundaryValue)),
                simplexnBoundaryValue: GetDouble(nameof(ExperimentParameters.SimplexnBoundaryValue)),
                numberOfDomainSamples: GetInt(nameof(ExperimentParameters.NumberOfDomainSamples)),
                numberOfTestPoints: GetInt(nameof(ExperimentParameters.NumberOfTestPoints)),
                numberOfPositivePoints: GetInt(nameof(ExperimentParameters.NumberOfPositivePoints)),
                numberOfNegativePoints: GetInt(nameof(ExperimentParameters.NumberOfNegativePoints)),
                defaultDomainLowerLimit: GetDouble(nameof(ExperimentParameters.DefaultDomainLowerLimit)),
                defaultDomainUpperLimit: GetDouble(nameof(ExperimentParameters.DefaultDomainUpperLimit)),
                maxNumberOfPointsInSingleArray: GetInt(nameof(ExperimentParameters.MaxNumberOfPointsInSingleArray))
                );
        }

        private static EvolutionParameters GetEvolutionParameters()
        {
            return new EvolutionParameters(
                objectVectorSize: GetInt(nameof(EvolutionParameters.ObjectVectorSize)),
                basePopulationSize: GetInt(nameof(EvolutionParameters.BasePopulationSize)),
                offspringPopulationSize: GetInt(nameof(EvolutionParameters.OffspringPopulationSize)),
                numberOfGenerations: GetInt(nameof(EvolutionParameters.NumberOfGenerations)),
                seed: GetInt(nameof(EvolutionParameters.Seed)),
                trackEvolutionSteps: GetBool(nameof(EvolutionParameters.TrackEvolutionSteps)),
                oneFifthRuleCheckInterval: GetInt(nameof(EvolutionParameters.OneFifthRuleCheckInterval)),
                oneFifthRuleScalingFactor: GetDouble(nameof(EvolutionParameters.OneFifthRuleScalingFactor)),
                numberOfParentsSolutionsToSelect: GetInt(nameof(EvolutionParameters.NumberOfParentsSolutionsToSelect)),
                typeOfParentsSelection: GetInt(nameof(EvolutionParameters.TypeOfParentsSelection)),
                typeOfSurvivorsSelection: GetInt(nameof(EvolutionParameters.TypeOfSurvivorsSelection)),

                globalLearningRate: GetDouble(nameof(EvolutionParameters.GlobalLearningRate)),
                individualLearningRate: GetDouble(nameof(EvolutionParameters.IndividualLearningRate)),
                stepThreshold: GetDouble(nameof(EvolutionParameters.StepThreshold)),
                rotationAngle: GetDouble(nameof(EvolutionParameters.RotationAngle)),
                typeOfMutation: GetInt(nameof(EvolutionParameters.TypeOfMutation)),

                useRecombination: GetBool(nameof(EvolutionParameters.UseRecombination)),
                typeOfObjectsRecombination: GetInt(nameof(EvolutionParameters.TypeOfObjectsRecombination)),
                typeOfStdDeviationsRecombination: GetInt(nameof(EvolutionParameters.TypeOfStdDeviationsRecombination)),
                typeOfRotationsRecombination: GetInt(nameof(EvolutionParameters.TypeOfRotationsRecombination))
                );
        }

        private static T Get<T>(object value) => (T)Convert.ChangeType(value, typeof(T));
        private static T GetEnum<T>(string columnName) => (T)Enum.Parse(typeof(T), Reader[columnName].ToString());
        private static int GetInt(string columnName) => Get<int>(Reader[columnName]);       
        private static string GetString(string columnName) => Get<string>(Reader[columnName]);
        private static bool GetBool(string columnName) => Get<bool>(Reader[columnName]);
        private static double GetDouble(string columnName)
            => Reader[columnName].ToString().Contains("NaN") ? double.NaN : Get<double>(Reader[columnName]);

        private const string GetExperimentsData = "SELECT * FROM experiments " +
                                   "WHERE ErrorCode is null AND( " +
                                   "(NumberOfGenerations= 100 AND OffspringPopulationSize = 1000) " +
                                   "OR(NumberOfGenerations= 200 AND OffspringPopulationSize = 500) " +
                                   "OR(NumberOfGenerations= 500 AND OffspringPopulationSize = 200) " +
                                   "OR(NumberOfGenerations= 1000 AND OffspringPopulationSize = 100)) " +
                                   "GROUP BY ImplementationVersion,NumberOfDimensions,MaximumNumberOfConstraints,NumberOfConstraintsCoefficients,Seed,TrackEvolutionSteps,UseRedundantConstraintsRemoving,UseDataNormalization,AllowQuadraticTerms,UseSeeding,TypeOfBenchmark,BallnBoundaryValue,CubenBoundaryValue,SimplexnBoundaryValue,NumberOfDomainSamples,NumberOfTestPoints,NumberOfPositivePoints,NumberOfNegativePoints,DefaultDomainLowerLimit,DefaultDomainUpperLimit,MaxNumberOfPointsInSingleArray,ObjectVectorSize,BasePopulationSize,OffspringPopulationSize,NumberOfGenerations,OneFifthRuleCheckInterval,OneFifthRuleScalingFactor,NumberOfParentsSolutionsToSelect,TypeOfParentsSelection,TypeOfSurvivorsSelection,GlobalLearningRate,IndividualLearningRate,StepThreshold,RotationAngle,TypeOfMutation,UseRecombination,TypeOfObjectsRecombination,TypeOfStdDeviationsRecombination,TypeOfRotationsRecombination";
    }
}
