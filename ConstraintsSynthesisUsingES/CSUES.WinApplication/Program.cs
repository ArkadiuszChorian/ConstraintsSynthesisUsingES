using System;
using System.Linq;
using CSUES.Engine.Enums;
using CSUES.Engine.Factories;
using CSUES.Engine.Measurement;
using CSUES.Engine.Models;
using CSUES.Engine.PointsGeneration;
using CSUES.Engine.PrePostProcessing;

namespace CSUES.WinApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //var database = new DatabaseContext("Test.db");

            var experimentParameters = new ExperimentParameters(
                numberOfDimensions: 2,
                basePopulationSize: 15,
                offspringPopulationSize: 100,
                numberOfGenerations: 1000,
                //numberOfPositivePoints: 1,
                useRedundantConstraintsRemoving: true,
                useRecombination: false,
                trackEvolutionSteps: true,
                numberOfParentsSolutionsToSelect: 5,
                numberOfPositivePoints: 500,
                numberOfNegativePoints: 500,
                typeOfBenchmark: BenchmarkType.Balln);

            var enginesFactory = new EnginesFactory();

            var engine = enginesFactory.Create(experimentParameters);

            var distanceCalculator = new CanberraDistanceCalculator();
            var positivePointsGenerator = new PositivePointsGenerator();
            var positiveTrainingPoints = positivePointsGenerator.GeneratePoints(experimentParameters.NumberOfPositivePoints, engine.Benchmark.Domains, engine.Benchmark.Constraints);

            var negativePointsGenerator = new NegativePointsGenerator(positiveTrainingPoints, distanceCalculator, new NearestNeighbourDistanceCalculator(distanceCalculator));
            var negativeTrainingPoints = negativePointsGenerator.GeneratePoints(experimentParameters.NumberOfNegativePoints, engine.Benchmark.Domains);
            
            var trainingPoints = positiveTrainingPoints.Concat(negativeTrainingPoints).ToArray();

            //FOR TEST TODO

            var normalizer = new StandardScorePointsNormalizer();
            var normalizedPoints = normalizer.ApplyProcessing(negativeTrainingPoints);
            var means = normalizer.Means(negativeTrainingPoints);
            var denormalizer = new StandardScorePointsDenormalizer(means, normalizer.StandardDeviations(negativeTrainingPoints, means));
            var denormalizedPoints = denormalizer.ApplyProcessing(normalizedPoints);

            //

            var mathModel = engine.SynthesizeModel(trainingPoints);

            var visualization = new Visualization(experimentParameters.TypeOfBenchmark);

            if (experimentParameters.TrackEvolutionSteps)
                visualization.PrepareThreePlots(positiveTrainingPoints, negativeTrainingPoints, mathModel,
                    engine.EvolutionSteps, 10, 10).Show();
            else
                visualization.PrepareTwoPlots(positiveTrainingPoints, negativeTrainingPoints, mathModel)
                    .Show();

            Console.WriteLine(mathModel.ReferenceModelInLpFormat);
            Console.WriteLine();
            Console.WriteLine(mathModel.SynthesizedModelInLpFormat);
            Console.WriteLine();
            Console.WriteLine("FINISHED!");
            Console.ReadKey();
        }
    }
}
