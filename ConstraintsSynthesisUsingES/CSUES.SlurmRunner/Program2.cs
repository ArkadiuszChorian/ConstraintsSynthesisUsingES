using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using CSUES.Common;
using CSUES.Engine.Enums;
using CSUES.Engine.Models;
using ES.Core.Enums;
using EvolutionDefaults = ES.Core.Utils.Defaults;
using Defaults = CSUES.Engine.Utils.Defaults;


using System.Linq;
using CSUES.Engine.Factories;
using CSUES.Engine.Measurement;
using CSUES.Engine.PointsGeneration;
using CSUES.Engine.Utils;
using ES.Core.Models;
using Version = CSUES.Common.Version;

namespace CSUES.SlurmRunner
{
    class Program2
    {       
        private const string BatchPath = "/bin/sh";
        private static readonly bool IsUnix = Environment.OSVersion.Platform == PlatformID.Unix;
        private static readonly string ScriptsDirPath = IsUnix ? "../Scripts/" : "../../../Scripts/";
        private static readonly string DatabaseDirPath = IsUnix ? $"../Database/{DatabaseContext.DbFilename}" : $"../../../Database/{DatabaseContext.DbFilename}";
        private static readonly string LogsDirPath = IsUnix ? "../Logs/" : "../../../Logs/";
        private static readonly string MainScriptFilename = Path.GetFullPath(IsUnix ? "../main.sh" : "../../../main.sh");
        private static readonly string ConsoleAppPath = Path.GetFullPath("../ConsoleApp/CSUES.ConsoleApplication.exe");
        private static readonly string DatabaseFullPath = Path.GetFullPath(DatabaseDirPath);
        private static readonly List<string> ScriptsFullPaths = new List<string>();

        private static readonly int[] NumbersOfGenerations = { 100, 200, 500, 1000 };
        private static readonly int[] OffspringPopulationSizes = { 1000, 500, 200, 100 };
        private static readonly BenchmarkType[] BenchmarkTypes = { BenchmarkType.Simplexn, BenchmarkType.Cuben, BenchmarkType.Balln };
        private static readonly int[] NumbersOfPositivePoints = { 500 };
        private static readonly bool[] UseDataNormalization = { true, false };
        private static readonly bool[] UseSeeding = { true, false };

        static void Main2(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Console.WriteLine($">>> Database path: {DatabaseFullPath}");
            var database = new DatabaseContext(DatabaseFullPath);
            //DatabaseContext database = null;

            //for (var seed = 1; seed <= 30; seed++){
            for (var seed = 1; seed <= 2; seed++){
            for (var numberOfDimensions = 2; numberOfDimensions <= 5; numberOfDimensions++){                                          
            for (var i = 0; i < NumbersOfGenerations.Length; i++){                                          
            //for (var j = 0; j < OffspringPopulationSizes.Length; j++){                                          
            for (var k = 0; k < BenchmarkTypes.Length; k++){                                          
            for (var l = 0; l < NumbersOfPositivePoints.Length; l++){              
            for (var m = 0; m < UseDataNormalization.Length; m++){              
            for (var n = 0; n < UseSeeding.Length; n++){
                var tmp = seed;                                     
                seed = seed == 1 ? 13 : 28;                                     
                //Console.WriteLine($">>> Enter the loop <<<");                                                                               
                //database = database ?? new DatabaseContext(DatabaseFullPath);
                var experimentParameters = GetExperimentParameters(numberOfDimensions, OffspringPopulationSizes[i],
                    NumbersOfGenerations[i], seed, BenchmarkTypes[k], false, NumbersOfPositivePoints[l], UseDataNormalization[m], UseSeeding[n]);

                //Console.WriteLine($"Seed:{seed} NumOfDim:{numberOfDimensions} NumOfGen:{NumbersOfGenerations[i]} OffspringSize:{OffspringPopulationSizes[i]} Bench:{BenchmarkTypes[k]} UseNorm:{UseDataNormalization[m]} Seeding:{UseSeeding[n]}");

                if (database.ExperimentShouldBePrepared(experimentParameters))
                {
                    if (IsUnix)
                    {
                        var experimentName = experimentParameters.GetFileName();
                        Console.WriteLine($">>> Saving script <<<\n{experimentName}\n");
                        SaveSingleScript(experimentParameters);
                        Console.WriteLine($">>> Script saved  <<<\n{experimentName}\n");
                        Console.WriteLine($">>>>>>>>>>>>>><<<<<<<<<<<<<<\n");
                    }
                    else
                    {
                        //Thread.Sleep(10);
                        //database.Dispose();
                        //database = null;
                        var experimentName = experimentParameters.GetFileName();
                        Console.WriteLine($">>> Experiment   started <<<\n{experimentName}\n");
                        //RunExperiment(experimentParameters);
                        Console.WriteLine($">>> Experiment  finished <<<\n{experimentName}\n");
                        Console.WriteLine($">>>>>>>>>>>>>><<<<<<<<<<<<<<\n");
                        //Thread.Sleep(10);
                    }                        
                }

                if (BenchmarkTypes[k] == BenchmarkType.Balln)
                    experimentParameters = GetExperimentParameters(numberOfDimensions, OffspringPopulationSizes[i],
                        NumbersOfGenerations[i], seed, BenchmarkTypes[k], true, NumbersOfPositivePoints[l],
                        UseDataNormalization[m], UseSeeding[n]);
                else
                {
                    //database.Dispose();
                    continue;
                }

                //Thread.Sleep(10);
                //database = database ?? new DatabaseContext(DatabaseFullPath);
                if (database.ExperimentShouldBePrepared(experimentParameters))
                {
                    if (IsUnix)
                    {
                        var experimentName = experimentParameters.GetFileName();
                        Console.WriteLine($">>> Saving script <<<\n{experimentName}\n");
                        SaveSingleScript(experimentParameters);
                        Console.WriteLine($">>> Script saved  <<<\n{experimentName}\n");
                        Console.WriteLine($">>>>>>>>>>>>>><<<<<<<<<<<<<<\n");
                    }
                    else
                    {
                        //Thread.Sleep(10);
                        //database.Dispose();
                        //database = null;
                        var experimentName = experimentParameters.GetFileName();
                        Console.WriteLine($">>> Experiment   started <<<\n{experimentName}\n");
                        //RunExperiment(experimentParameters);
                        Console.WriteLine($">>> Experiment  finished <<<\n{experimentName}\n");
                        Console.WriteLine($">>>>>>>>>>>>>><<<<<<<<<<<<<<\n");
                        //Thread.Sleep(10);
                    }
                } 

                //database.Dispose();
                seed = tmp;

            }}}}}}}//}
            
            //database.Dispose();

            if (!IsUnix) return;

            var sb = new StringBuilder();

            ScriptsFullPaths.ForEach(sfp => sb.Append("sbatch ").Append(sfp).Append("\n"));

            var mainScriptContent = sb.ToString();

            File.WriteAllText(MainScriptFilename, mainScriptContent);

            Process.Start(BatchPath, MainScriptFilename);
        }

        private static ExperimentParameters GetExperimentParameters(int numberOfDimensions, int offspringPopulationSize, int numberOfGenerations, int seed, BenchmarkType typeOfBenchmark, bool allowQuadraticTerms, int numberOfPositivePoints, bool useDataNormalization, bool useSeeding)
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
                useDataNormalization: useDataNormalization,
                allowQuadraticTerms: allowQuadraticTerms,
                useSeeding: useSeeding,

                ballnBoundaryValue: 2.7,
                cubenBoundaryValue: 2.7,
                simplexnBoundaryValue: 2.7,

                numberOfDomainSamples: 100000,
                numberOfTestPoints: 100000,
                numberOfPositivePoints: numberOfPositivePoints,
                numberOfNegativePoints: numberOfPositivePoints * numberOfDimensions * numberOfDimensions,
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
            var arguments = experimentParameters.ToConsoleArgumentsString();
            var filename = experimentParameters.GetFileName(extension: ".sh");
            var singleScriptPath = Path.GetFullPath(ScriptsDirPath + filename);

            ScriptsFullPaths.Add(singleScriptPath);

            File.WriteAllText(singleScriptPath, ExperimentScriptContent(arguments));
        }

        private static string ExperimentScriptContent(string arguments)
        {
            return "#!/bin/bash\n" +
                   "#SBATCH -p lab-43-student,lab-44-student,lab-ci-student\n\n" +
                   "#SBATCH -x lab-ci-1,lab-43-5,lab-43-10,lab-43-16,lab-45-2,lab-45-3,lab-45-4\n\n" +
                   "export LD_LIBRARY_PATH=~/mgr/gurobi751/linux64/lib/\n" +
                   "export GRB_LICENSE_FILE=~/mgr/Licenses/gurobi-$(hostname).lic\n\n" +
                   $"srun mono {ConsoleAppPath} {arguments}";
        }

        private static void RunExperiment(ExperimentParameters experimentParameters)//, DatabaseContext databaseX)
        {
            var database = new DatabaseContext(DatabaseFullPath);
            var version = new Version(DateTime.Now);
            var stoper = new Stopwatch();

            //if (database.Exists(experimentParameters))
            //{
            //    Console.WriteLine("BYLEM WIDZIALEM");
            //    //database.Dispose();
            //    return;
            //}

            database.Insert(version);
            database.Insert(experimentParameters);
            database.Insert(experimentParameters.EvolutionParameters);

            IDictionary<int, EvolutionStep> evolutionSteps = null;

            try
            {
                var enginesFactory = new EnginesFactory();

                var engine = enginesFactory.Create(experimentParameters);

                var distanceCalculator = new CanberraDistanceCalculator();
                var positivePointsGenerator = new PositivePointsGenerator();
                stoper.Restart();
                var positiveTrainingPoints = positivePointsGenerator.GeneratePoints(experimentParameters.NumberOfPositivePoints, engine.Benchmark.Domains, engine.Benchmark.Constraints);
                stoper.Stop();
                engine.Statistics.PositiveTrainingPointsGenerationTime = stoper.Elapsed;

                var negativePointsGenerator = new NegativePointsGenerator(positiveTrainingPoints, distanceCalculator, new NearestNeighbourDistanceCalculator(distanceCalculator));
                //var negativeTrainingPoints = negativePointsGenerator.GeneratePoints(experimentParameters.NumberOfNegativePoints, engine.Benchmark.Domains);
                stoper.Restart();
                var negativeTrainingPoints = negativePointsGenerator.GeneratePoints(experimentParameters.NumberOfNegativePoints, engine.Benchmark.Domains, engine.Benchmark.Constraints);
                stoper.Stop();
                engine.Statistics.NegativeTrainingPointsGenerationTime = stoper.Elapsed;
                //Console.WriteLine("Evolution starts!");
                var trainingPoints = positiveTrainingPoints.Concat(negativeTrainingPoints).ToArray();

                stoper.Restart();
                var mathModel = engine.SynthesizeModel(trainingPoints);
                stoper.Stop();
                engine.Statistics.TotalSynthesisTime = stoper.Elapsed;

                database.Insert(mathModel);

                var testPointsGenerator = new TestPointsGenerator();
                stoper.Restart();
                var testPoints = testPointsGenerator.GeneratePoints(experimentParameters.NumberOfTestPoints, engine.Benchmark.Domains, engine.Benchmark.Constraints);
                stoper.Stop();
                engine.Statistics.TestPointsGenerationTime = stoper.Elapsed;

                var statistics = engine.EvaluateModel(testPoints);

                database.Insert(statistics);

                evolutionSteps = engine.CoreEvolutionSteps;
                //Logger.Instance.Log(experimentParameters);

                //Logger.Instance.Log(mathModel);
                //Logger.Instance.Log(statistics);

                //database.Insert(Logger.Instance.GetLogAsString());
            }
            catch (Exception exception)
            {
                database.Insert(exception);
            }

            database.Save();            

            if (evolutionSteps != null && experimentParameters.TrackEvolutionSteps)
            {
                Logger.Instance.Log(evolutionSteps);
                var logsFullPath = Path.GetFullPath(LogsDirPath + experimentParameters.GetFileName("Log", ".cmplog"));
                File.WriteAllText(logsFullPath, StringCompressor.CompressString(Logger.Instance.GetLogAsString()));
            }
            //GlobalStoper.Stop();
            //Console.WriteLine(GlobalStoper.ElapsedMilliseconds);
            //Console.ReadKey();
            database.Dispose();
        }
    }
}
