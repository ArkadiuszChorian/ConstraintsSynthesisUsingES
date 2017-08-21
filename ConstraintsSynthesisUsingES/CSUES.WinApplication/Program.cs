using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using CSUES.Engine.Enums;
using CSUES.Engine.Factories;
using CSUES.Engine.Measurement;
using CSUES.Engine.Models;
using CSUES.Engine.PointsGeneration;
using ES.Core.Enums;

namespace CSUES.WinApplication
{
    class Program
    {
        private const int NumberOfEvolutionStepsToShow = 100;
        static void Main(string[] args)
        {
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            //CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var experimentParameters = new ExperimentParameters(
                numberOfDimensions: 2,
                basePopulationSize: 100,
                offspringPopulationSize: 100,
                numberOfGenerations: 100,
                seed: new Random().Next(),
                //seed: 1,
                //seed: 1971749862,
                //oneFifthRuleCheckInterval: 10,
                //individualLearningRate: 0.1, //0.47287080450158792
                //globalLearningRate: 0.1, //0.31622776601683794
                typeOfParentsSelection: ParentsSelectionType.Even,
                typeOfStdDeviationsRecombination: RecombinationType.Intermediate,
                typeOfRotationsRecombination: RecombinationType.Intermediate,
                useRedundantConstraintsRemoving: true,
                useDataNormalization: true,
                allowQuadraticTerms: true,
                useRecombination: false,
                trackEvolutionSteps: true,
                numberOfParentsSolutionsToSelect: 5,
                numberOfPositivePoints: 500,
                numberOfNegativePoints: 1500,
                //ballnBoundaryValue: 10,
                typeOfBenchmark: BenchmarkType.Simplexn);

            var enginesFactory = new EnginesFactory();

            var engine = enginesFactory.Create(experimentParameters);

            var distanceCalculator = new CanberraDistanceCalculator();
            var positivePointsGenerator = new PositivePointsGenerator();
            var positiveTrainingPoints = positivePointsGenerator.GeneratePoints(experimentParameters.NumberOfPositivePoints, engine.Benchmark.Domains, engine.Benchmark.Constraints);

            var negativePointsGenerator = new NegativePointsGenerator(positiveTrainingPoints, distanceCalculator, new NearestNeighbourDistanceCalculator(distanceCalculator));
            //var negativeTrainingPoints = negativePointsGenerator.GeneratePoints(experimentParameters.NumberOfNegativePoints, engine.Benchmark.Domains);
            var negativeTrainingPoints = negativePointsGenerator.GeneratePoints(experimentParameters.NumberOfNegativePoints, engine.Benchmark.Domains, engine.Benchmark.Constraints);
            
            var trainingPoints = positiveTrainingPoints.Concat(negativeTrainingPoints).ToArray();

            var testPointsGenerator = new TestPointsGenerator();
            var testPoints = testPointsGenerator.GeneratePoints(experimentParameters.NumberOfTestPoints, engine.Benchmark.Domains, engine.Benchmark.Constraints);
            Console.WriteLine("Evolution starts!");
            var mathModel = engine.SynthesizeModel(trainingPoints);

            var statistics = engine.EvaluateModel(testPoints);

            var visualization = new Visualization(experimentParameters.TypeOfBenchmark);

            if (experimentParameters.TrackEvolutionSteps)
                visualization.PreparePlots(positiveTrainingPoints, negativeTrainingPoints, testPoints.Reduce(), mathModel, engine.EvolutionSteps, NumberOfEvolutionStepsToShow)
                    .Show();
            else
                visualization.PreparePlots(positiveTrainingPoints, negativeTrainingPoints, testPoints.Reduce(), mathModel)
                    .Show();
            
            Console.WriteLine("==========================================");
            Console.WriteLine("=============== PARAMETERS ===============");
            Console.WriteLine("==========================================");
            Console.WriteLine(experimentParameters.GetPrintableString());
            Console.WriteLine();
            Console.WriteLine("==========================================");
            Console.WriteLine("============= REFERENCE MODEL ============");
            Console.WriteLine("==========================================");
            Console.WriteLine(mathModel.ReferenceModelInLpFormat);
            Console.WriteLine();
            Console.WriteLine("==========================================");
            Console.WriteLine("============ SYNTHESIZED MODEL ===========");
            Console.WriteLine("==========================================");
            Console.WriteLine(mathModel.SynthesizedModelInLpFormat);
            Console.WriteLine();
            Console.WriteLine("==========================================");
            Console.WriteLine("=============== STATISTICS ===============");
            Console.WriteLine("==========================================");
            Console.WriteLine(statistics.GetPrintableString());
            Console.WriteLine();
            Console.ReadKey();
        }       
    }
}
