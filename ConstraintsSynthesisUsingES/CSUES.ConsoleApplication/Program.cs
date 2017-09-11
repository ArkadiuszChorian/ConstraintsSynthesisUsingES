using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CSUES.Common;
using CSUES.Engine.Enums;
using CSUES.Engine.Factories;
using CSUES.Engine.Measurement;
using CSUES.Engine.Models;
using CSUES.Engine.PointsGeneration;
using ES.Core.Enums;
using ES.Core.Models;
using EvolutionDefaults = ES.Core.Utils.Defaults;
using Defaults = CSUES.Engine.Utils.Defaults;
using Version = CSUES.Common.Version;

namespace CSUES.ConsoleApplication
{
    class Program
    {
        //private static readonly string DatabaseDirPath = Path.GetFullPath("../Database/" + DatabaseContext.DbFilename);
        private static readonly bool IsUnix = Environment.OSVersion.Platform == PlatformID.Unix;
        private static readonly string LogsPathString = IsUnix ? "../Logs/" : "../../../Logs/";
        private static readonly string DatabasePathString = IsUnix ? "../Database/" : "../../../Database/";       
        private static readonly string DatabaseFullPath = Path.GetFullPath(DatabasePathString + DatabaseContext.DbFilename);

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var database = new DatabaseContext(DatabaseFullPath);
            var version = new Version(DateTime.Now);
            var experimentParameters = GetExperimentParameters();
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

                //Logger.Instance.Log(experimentParameters);
                Logger.Instance.Log(engine.CoreEvolutionSteps);
                //Logger.Instance.Log(mathModel);
                //Logger.Instance.Log(statistics);

                var logsFullPath = Path.GetFullPath(LogsPathString + experimentParameters.GetFileName("Log", ".txt"));

                File.WriteAllText(logsFullPath, Logger.Instance.GetLogAsString());

                //database.Insert(Logger.Instance.GetLogAsString());
            }
            catch (Exception exception)
            {
                database.Insert(exception);
            }

            database.Save();
            database.Dispose();
        }

        private static ExperimentParameters GetExperimentParameters()
        {
            var numberOfDimensions = Arguments.Get<int>(nameof(ExperimentParameters.NumberOfDimensions));

            return new ExperimentParameters(
                numberOfDimensions: numberOfDimensions,
                basePopulationSize: Arguments.Get<int>(nameof(EvolutionParameters.BasePopulationSize)),
                offspringPopulationSize: Arguments.Get<int>(nameof(EvolutionParameters.OffspringPopulationSize)),
                numberOfGenerations: Arguments.Get<int>(nameof(EvolutionParameters.NumberOfGenerations)),
                seed: Arguments.Get<int>(nameof(ExperimentParameters.Seed)),
                typeOfBenchmark: Arguments.Get<BenchmarkType>(nameof(ExperimentParameters.TypeOfBenchmark)),

                trackEvolutionSteps: Arguments.Get(nameof(ExperimentParameters.TrackEvolutionSteps), EvolutionDefaults.TrackEvolutionSteps),
                useRedundantConstraintsRemoving: Arguments.Get(nameof(ExperimentParameters.UseRedundantConstraintsRemoving), Defaults.UseRedundantConstraintsRemoving),
                useDataNormalization: Arguments.Get(nameof(ExperimentParameters.UseDataNormalization), Defaults.UseDataNormalization),
                allowQuadraticTerms: Arguments.Get(nameof(ExperimentParameters.AllowQuadraticTerms), Defaults.AllowQuadraticTerms),
                useSeeding: Arguments.Get(nameof(ExperimentParameters.UseSeeding), Defaults.UseSeeding),
                        
                ballnBoundaryValue: Arguments.Get(nameof(ExperimentParameters.BallnBoundaryValue), Defaults.BallnBoundaryValue),
                cubenBoundaryValue: Arguments.Get(nameof(ExperimentParameters.CubenBoundaryValue), Defaults.CubenBoundaryValue),
                simplexnBoundaryValue: Arguments.Get(nameof(ExperimentParameters.SimplexnBoundaryValue), Defaults.SimplexnBoundaryValue),

                numberOfDomainSamples: Arguments.Get(nameof(ExperimentParameters.NumberOfDomainSamples), Defaults.NumberOfDomainSamples),
                numberOfTestPoints: Arguments.Get(nameof(ExperimentParameters.NumberOfTestPoints), Defaults.NumberOfTestPoints),
                numberOfPositivePoints: Arguments.Get(nameof(ExperimentParameters.NumberOfPositivePoints), Defaults.NumberOfPositivePoints),
                numberOfNegativePoints: Arguments.Get(nameof(ExperimentParameters.NumberOfNegativePoints), Defaults.NumberOfNegativePoints),
                maxNumberOfPointsInSingleArray: Arguments.Get(nameof(ExperimentParameters.MaxNumberOfPointsInSingleArray), Defaults.MaxNumberOfPointsInSingleArray),

                globalLearningRate: Arguments.Get(nameof(EvolutionParameters.GlobalLearningRate), EvolutionDefaults.GlobalLearningRate(numberOfDimensions)),
                individualLearningRate: Arguments.Get(nameof(EvolutionParameters.IndividualLearningRate), EvolutionDefaults.IndividualLearningRate(numberOfDimensions)),
                stepThreshold: Arguments.Get(nameof(EvolutionParameters.StepThreshold), 0.0001),
                //stepThreshold: Arguments.Get(nameof(EvolutionParameters.StepThreshold), double.Epsilon * 1.0E+170),
                rotationAngle: Arguments.Get(nameof(EvolutionParameters.RotationAngle), EvolutionDefaults.RotationAngle),
                //rotationAngle: Arguments.Get(nameof(EvolutionParameters.RotationAngle), double.Epsilon * 1.0E+170),//EvolutionDefaults.RotationAngle),
                typeOfMutation: Arguments.Get(nameof(EvolutionParameters.TypeOfMutation), MutationType.Correlated),//EvolutionDefaults.TypeOfMutation),
                //typeOfMutation: Arguments.Get(nameof(EvolutionParameters.TypeOfMutation), MutationType.Correlated),//EvolutionDefaults.TypeOfMutation),

                numberOfParentsSolutionsToSelect: Arguments.Get(nameof(EvolutionParameters.NumberOfParentsSolutionsToSelect), EvolutionDefaults.NumberOfParentsSolutionsToSelect),
                typeOfParentsSelection: Arguments.Get(nameof(EvolutionParameters.TypeOfParentsSelection), ParentsSelectionType.Even),
                typeOfSurvivorsSelection: Arguments.Get(nameof(EvolutionParameters.TypeOfSurvivorsSelection), SurvivorsSelectionType.Distinct),//EvolutionDefaults.TypeOfSurvivorsSelection),

                oneFifthRuleCheckInterval: Arguments.Get(nameof(EvolutionParameters.OneFifthRuleCheckInterval), EvolutionDefaults.OneFifthRuleCheckInterval),
                oneFifthRuleScalingFactor: Arguments.Get(nameof(EvolutionParameters.OneFifthRuleScalingFactor), EvolutionDefaults.OneFifthRuleScalingFactor),

                useRecombination: Arguments.Get(nameof(EvolutionParameters.UseRecombination), EvolutionDefaults.UseRecombination),
                typeOfObjectsRecombination: Arguments.Get(nameof(EvolutionParameters.TypeOfObjectsRecombination), EvolutionDefaults.TypeOfObjectsRecombination),
                typeOfStdDeviationsRecombination: Arguments.Get(nameof(EvolutionParameters.TypeOfStdDeviationsRecombination), EvolutionDefaults.TypeOfStdDeviationsRecombination),
                typeOfRotationsRecombination: Arguments.Get(nameof(EvolutionParameters.TypeOfRotationsRecombination), EvolutionDefaults.TypeOfRotationsRecombination)
                );
        }      
    }
}
