using System;
using System.IO;
using System.Linq;
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
        private static readonly string DatabasePath = Path.GetFullPath("../../../Database/" + DatabaseContext.DbFilename);

        static void Main(string[] args)
        {
            var database = new DatabaseContext(DatabasePath);
            var version = new Version(DateTime.Now);
            var experimentParameters = GetExperimentParameters();

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
                var positiveTrainingPoints = positivePointsGenerator.GeneratePoints(experimentParameters.NumberOfPositivePoints, engine.Benchmark.Domains, engine.Benchmark.Constraints);

                var negativePointsGenerator = new NegativePointsGenerator(positiveTrainingPoints, distanceCalculator, new NearestNeighbourDistanceCalculator(distanceCalculator));
                //var negativeTrainingPoints = negativePointsGenerator.GeneratePoints(experimentParameters.NumberOfNegativePoints, engine.Benchmark.Domains);
                var negativeTrainingPoints = negativePointsGenerator.GeneratePoints(experimentParameters.NumberOfNegativePoints, engine.Benchmark.Domains, engine.Benchmark.Constraints);
                Console.WriteLine("Evolution starts!");
                var trainingPoints = positiveTrainingPoints.Concat(negativeTrainingPoints).ToArray();     
                var mathModel = engine.SynthesizeModel(trainingPoints);

                database.Insert(mathModel);

                var testPointsGenerator = new TestPointsGenerator();
                var testPoints = testPointsGenerator.GeneratePoints(experimentParameters.NumberOfTestPoints, engine.Benchmark.Domains, engine.Benchmark.Constraints);

                var statistics = engine.EvaluateModel(testPoints);

                database.Insert(statistics);
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

                trackEvolutionSteps: Arguments.Get(nameof(ExperimentParameters.UseRedundantConstraintsRemoving), EvolutionDefaults.TrackEvolutionSteps),
                useRedundantConstraintsRemoving: Arguments.Get(nameof(ExperimentParameters.UseRedundantConstraintsRemoving), Defaults.UseRedundantConstraintsRemoving),
                useDataNormalization: Arguments.Get(nameof(ExperimentParameters.UseDataNormalization), Defaults.UseDataNormalization),
                allowQuadraticTerms: Arguments.Get(nameof(ExperimentParameters.AllowQuadraticTerms), Defaults.AllowQuadraticTerms),
                        
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
                rotationAngle: Arguments.Get(nameof(EvolutionParameters.RotationAngle), EvolutionDefaults.RotationAngle),
                typeOfMutation: Arguments.Get(nameof(EvolutionParameters.TypeOfMutation), EvolutionDefaults.TypeOfMutation),

                numberOfParentsSolutionsToSelect: Arguments.Get(nameof(EvolutionParameters.NumberOfParentsSolutionsToSelect), EvolutionDefaults.NumberOfParentsSolutionsToSelect),
                typeOfParentsSelection: Arguments.Get(nameof(EvolutionParameters.TypeOfParentsSelection), ParentsSelectionType.Even),
                typeOfSurvivorsSelection: Arguments.Get(nameof(EvolutionParameters.TypeOfSurvivorsSelection), EvolutionDefaults.TypeOfSurvivorsSelection),

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
