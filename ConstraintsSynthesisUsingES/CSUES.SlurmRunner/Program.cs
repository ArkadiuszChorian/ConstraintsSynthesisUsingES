using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CSUES.Common;
using CSUES.Engine.Enums;
using CSUES.Engine.Models;
using ES.Core.Enums;
using EvolutionDefaults = ES.Core.Utils.Defaults;
using Defaults = CSUES.Engine.Utils.Defaults;

namespace CSUES.SlurmRunner
{
    class Program
    {
        //private const string ScriptsDirPath = "../Scripts/";
        private const string ScriptsDirPath = "../../../Scripts/";
        //private const string DatabaseDirPath = "../Database/";
        private const string DatabaseDirPath = "../../../Database/";
        private const string BatchPath = "/bin/sh";
        //private static string MainScriptFilename = "../main.sh";
        private static readonly string MainScriptFilename = Path.GetFullPath("../../../main.sh");
        private static readonly string ConsoleAppPath = Path.GetFullPath("../ConsoleApp/CSUES.ConsoleApplication.exe");
        private static readonly List<string> ScriptsFullPaths = new List<string>();

        private static readonly int[] NumbersOfGenerations = { 100, 200, 500, 1000 };
        private static readonly int[] OffspringPopulationSizes = { 1000, 500, 200, 100 };
        private static readonly BenchmarkType[] BenchmarkTypes = { BenchmarkType.Simplexn, BenchmarkType.Cuben, BenchmarkType.Balln };
        private static readonly int[] NumbersOfPositivePoints = { 500 };

        static void Main(string[] args)
        {
            var path = Path.GetFullPath(DatabaseDirPath + DatabaseContext.DbFilename);
            var database = new DatabaseContext(path);           

            for (var seed = 1; seed <= 30; seed++){
            for (var numberOfDimensions = 1; numberOfDimensions <= 7; numberOfDimensions++){                                          
            for (var i = 0; i < NumbersOfGenerations.Length; i++){                                          
            for (var j = 0; j < OffspringPopulationSizes.Length; j++){                                          
            for (var k = 0; k < BenchmarkTypes.Length; k++){                                          
            for (var l = 0; l < NumbersOfPositivePoints.Length; l++){              
                                                                                
                var experimentParameters = GetExperimentParameters(numberOfDimensions, OffspringPopulationSizes[j],
                    NumbersOfGenerations[i], seed, BenchmarkTypes[k], false, NumbersOfPositivePoints[l],
                    NumbersOfPositivePoints[l]);

                if (database.Exists(experimentParameters) == false)
                    SaveSingleScript(experimentParameters);

                if (BenchmarkTypes[k] == BenchmarkType.Balln)
                    experimentParameters.AllowQuadraticTerms = true;

                if (database.Exists(experimentParameters) == false)
                    SaveSingleScript(experimentParameters);                                                                 
            }}}}}}

            var sb = new StringBuilder();

            ScriptsFullPaths.ForEach(sfp => sb.Append("sbatch ").Append(sfp).Append("\n"));

            var mainScriptContent = sb.ToString();

            File.WriteAllText(MainScriptFilename, mainScriptContent);

            Process.Start(BatchPath, MainScriptFilename);
        }

        private static ExperimentParameters GetExperimentParameters(int numberOfDimensions, int offspringPopulationSize, int numberOfGenerations, int seed, BenchmarkType typeOfBenchmark, bool allowQuadraticTerms, int numberOfPositivePoints, int numberOfNegativePoints)
        {
            return new ExperimentParameters(
                numberOfDimensions: numberOfDimensions,
                basePopulationSize: 100,
                offspringPopulationSize: offspringPopulationSize,
                numberOfGenerations: numberOfGenerations,
                seed: seed,
                typeOfBenchmark: typeOfBenchmark,

                trackEvolutionSteps: false,
                useRedundantConstraintsRemoving: true,
                useDataNormalization: true,
                allowQuadraticTerms: allowQuadraticTerms,

                ballnBoundaryValue: 2.7,
                cubenBoundaryValue: 2.7,
                simplexnBoundaryValue: 2.7,

                numberOfDomainSamples: 100000,
                numberOfTestPoints: 100000,
                numberOfPositivePoints: numberOfPositivePoints,
                numberOfNegativePoints: numberOfNegativePoints,
                maxNumberOfPointsInSingleArray: 800000,

                globalLearningRate: EvolutionDefaults.GlobalLearningRate(numberOfDimensions),
                individualLearningRate: EvolutionDefaults.IndividualLearningRate(numberOfDimensions),
                stepThreshold: 0.0001,
                rotationAngle: EvolutionDefaults.RotationAngle,
                typeOfMutation: MutationType.Correlated,

                numberOfParentsSolutionsToSelect: 5,
                typeOfParentsSelection: ParentsSelectionType.Even,
                typeOfSurvivorsSelection: SurvivorsSelectionType.Distinct,

                //oneFifthRuleCheckInterval: 5,
                //oneFifthRuleScalingFactor: 0.9,

                useRecombination: false,
                typeOfObjectsRecombination: RecombinationType.Discrete,
                typeOfStdDeviationsRecombination: RecombinationType.Intermediate,
                typeOfRotationsRecombination: RecombinationType.Intermediate
                );
        }

        private static void SaveSingleScript(ExperimentParameters experimentParameters)
        {
            var sb = new StringBuilder();
            var arguments = experimentParameters.ToConsoleArgumentsString();

            sb.Append(experimentParameters.TypeOfBenchmark).Append("_");
            sb.Append(experimentParameters.Seed).Append("_");
            sb.Append(experimentParameters.NumberOfDimensions).Append("_");
            sb.Append(experimentParameters.EvolutionParameters.NumberOfGenerations).Append("_");
            sb.Append(experimentParameters.EvolutionParameters.OffspringPopulationSize).Append("_");
            sb.Append(experimentParameters.NumberOfPositivePoints).Append("_");
            sb.Append(experimentParameters.NumberOfNegativePoints).Append("_");
            sb.Append(experimentParameters.AllowQuadraticTerms);
            var singleScriptPath = Path.GetFullPath(ScriptsDirPath + sb.Append(".sh"));

            ScriptsFullPaths.Add(singleScriptPath);

            File.WriteAllText(singleScriptPath, ExperimentScriptContent(arguments));
        }

        private static string ExperimentScriptContent(string arguments)
        {
            return "#!/bin/bash\n" +
                   "#SBATCH -p lab-43-student,lab-44-student,lab-ci-student\n\n" +
                   "export LD_LIBRARY_PATH=~/gurobi751/linux64/lib/\n" +
                   "export GRB_LICENSE_FILE=~/gurobi-$(hostname).lic\n\n" +
                   $"srun mono {ConsoleAppPath} {arguments}";
        }
    }
}
