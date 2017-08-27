using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CSUES.Engine.Core;
using CSUES.Engine.Enums;
using CSUES.Engine.Factories;
using CSUES.Engine.Measurement;
using CSUES.Engine.Models;
using CSUES.Engine.PointsGeneration;
using ES.Core.Enums;
using OxyPlot;

namespace CSUES.WinApplication
{
    class Program
    {
        private const int NumberOfEvolutionStepsToShow = 100;
        private static bool ShowNormalizaton = true;
        private static bool ShowEvolutionSteps = true;
        private static bool ShowReferenceModelOnTest = false;
        static void Main(string[] args)
        {
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            //CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var experimentParameters = new ExperimentParameters(
                numberOfDimensions: 2,
                basePopulationSize: 10,
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
                allowQuadraticTerms: false,
                useRecombination: false,
                trackEvolutionSteps: true,
                numberOfParentsSolutionsToSelect: 5,
                numberOfPositivePoints: 500,
                numberOfNegativePoints: 1500,
                //ballnBoundaryValue: 10,
                typeOfBenchmark: BenchmarkType.Balln);

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

            var visualization = PrepareVisualisation(experimentParameters, engine, positiveTrainingPoints, negativeTrainingPoints,
                testPoints);

            visualization.Show();
            
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

        private static Visualization PrepareVisualisation(ExperimentParameters experimentParameters, IEngine engine, IList<Point> positiveTrainingPoints, IList<Point> negativeTrainingPoints, IList<Point> testPoints, int numberOfEvolutionStepsToShow = NumberOfEvolutionStepsToShow)
        {
            var visualization = new Visualization(experimentParameters.TypeOfBenchmark);

            testPoints = testPoints.Reduce();
            var positiveTestPoints = testPoints.Where(tp => tp.ClassificationType == ClassificationType.Positive).ToList();
            var negativeTestPoints = testPoints.Where(tp => tp.ClassificationType == ClassificationType.Negative).ToList();
            var synthesizedConstraints = engine.MathModel.SynthesizedModel;
            var referenceConstraints = engine.MathModel.ReferenceModel;

            visualization
                .AddNextPlot("Reference - Training")
                .AddPoints(positiveTrainingPoints, OxyColors.Green)
                .AddPoints(negativeTrainingPoints, OxyColors.DarkRed)
                .AddConstraints(referenceConstraints, OxyPalettes.Rainbow);

            visualization
                .AddNextPlot("Synthesized - Training")
                .AddPoints(positiveTrainingPoints, OxyColors.Green)
                .AddPoints(negativeTrainingPoints, OxyColors.DarkRed)
                .AddConstraints(synthesizedConstraints, OxyPalettes.Rainbow);

            if (experimentParameters.UseDataNormalization && ShowNormalizaton)
            {
                var normalizedPositiveTrainingPoints = 
                    engine.NormalizedTrainingPoints.Where(tp => tp.ClassificationType == ClassificationType.Positive).ToList();
                var normalizedNegativeTrainingPoints = 
                    engine.NormalizedTrainingPoints.Where(tp => tp.ClassificationType == ClassificationType.Negative).ToList();
                var normalizedSynthesizedConstraints = engine.NormalizedSynthesizedConstraints;

                visualization
                    .AddNextPlot("Synthesized - Training [Normalized]")
                    .AddPoints(normalizedPositiveTrainingPoints, OxyColors.Green)
                    .AddPoints(normalizedNegativeTrainingPoints, OxyColors.DarkRed)
                    .AddConstraints(normalizedSynthesizedConstraints, OxyPalettes.Rainbow);

                if (experimentParameters.TrackEvolutionSteps && ShowEvolutionSteps)
                {
                    var normalizedEvolutionSteps = engine.NormalizedEvolutionSteps;

                    visualization
                        .AddNextPlot("Evolution steps [Normalized]")
                        .AddPoints(normalizedPositiveTrainingPoints, OxyColors.Green)
                        .AddPoints(normalizedNegativeTrainingPoints, OxyColors.DarkRed)
                        .AddEvolutionSteps(normalizedEvolutionSteps, numberOfEvolutionStepsToShow);
                }                
            }

            if (experimentParameters.TrackEvolutionSteps && ShowEvolutionSteps)
            {
                var evolutionSteps = engine.EvolutionSteps;

                visualization
                    .AddNextPlot("Evolution steps")
                    .AddPoints(positiveTrainingPoints, OxyColors.Green)
                    .AddPoints(negativeTrainingPoints, OxyColors.DarkRed)
                    .AddEvolutionSteps(evolutionSteps, numberOfEvolutionStepsToShow);
            }

            if (ShowReferenceModelOnTest)
            {
                visualization
                .AddNextPlot("Reference - Test")
                .AddPoints(positiveTestPoints, OxyColors.Green)
                .AddPoints(negativeTestPoints, OxyColors.DarkRed)
                .AddConstraints(referenceConstraints, OxyPalettes.Rainbow);
            }
            
            visualization
                .AddNextPlot("Synthesized - Test")
                .AddPoints(positiveTestPoints, OxyColors.Green)
                .AddPoints(negativeTestPoints, OxyColors.DarkRed)
                .AddConstraints(synthesizedConstraints, OxyPalettes.Rainbow);

            return visualization;
        }
    }
}
