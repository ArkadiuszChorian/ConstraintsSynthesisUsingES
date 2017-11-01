using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ES.Core.Factories;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Mutation;
using ES.Core.MutationSupervison;
using ES.Core.PopulationGeneration;
using ES.Core.Selection;
using ES.Core.Utils;

namespace ES.Core.Engine
{
    public abstract class EngineBase : IEngine
    {
        protected PopulationGeneratorBase PopulationGenerator;
        protected MutatorBase ObjectMutator;
        protected MutatorBase StdDeviationsMutator;
        protected MutationRuleSupervisorBase MutationRuleSupervisor;
        protected ParentsSelectorBase ParentsSelector;
        protected SurvivorsSelectorBase SurvivorsSelector;
        protected IGenericFactory<Solution> SolutionsFactory;
        protected Solution[] BasePopulation;
        protected Solution[] OffspringPopulation;
        protected Stopwatch Stoper;

        protected EngineBase(EvolutionParameters evolutionParameters, IGenericFactory<Solution> solutionsFactory, PopulationGeneratorBase populationGenerator, MutatorBase objectMutator, MutatorBase stdDeviationsMutator, MutationRuleSupervisorBase mutationRuleSupervisor, ParentsSelectorBase parentsSelector, SurvivorsSelectorBase survivorsSelector, Statistics statistics, Stopwatch stoper)
        {
            Parameters = evolutionParameters;
            SolutionsFactory = solutionsFactory;
            PopulationGenerator = populationGenerator;
            ObjectMutator = objectMutator;
            StdDeviationsMutator = stdDeviationsMutator;
            MutationRuleSupervisor = mutationRuleSupervisor;
            ParentsSelector = parentsSelector;
            SurvivorsSelector = survivorsSelector;
            Statistics = statistics;
            Stoper = stoper;

            BasePopulation = new Solution[evolutionParameters.BasePopulationSize];
            OffspringPopulation = new Solution[evolutionParameters.OffspringPopulationSize]; 
            EvolutionStepsSimple = new List<Solution>(evolutionParameters.NumberOfGenerations); 
            EvolutionSteps = new Dictionary<int, EvolutionStep>(evolutionParameters.NumberOfGenerations);                           
        }
        
        public EvolutionParameters Parameters { get; set; }       
        public Statistics Statistics { get; set; }      
        public IList<Solution> EvolutionStepsSimple { get; }
        public IDictionary<int, EvolutionStep> EvolutionSteps { get; }

        protected EvolutionStep CurrentEvolutionStep;
        protected MutationStep CurrentMutation;

        public Solution RunEvolution(EvaluatorBase evaluator, ISeedingProcessor seedingProcessor)
        {
            BasePopulation = InitializePopulations(evaluator, seedingProcessor);

            return Evolution(evaluator);
        }

        public Solution RunEvolution(EvaluatorBase evaluator)
        {
            BasePopulation = InitializePopulations(evaluator);

            return Evolution(evaluator);
        }

        private Solution Evolution(EvaluatorBase evaluator)
        {            
            var bestSolution = BasePopulation.First();
            var numberOfGenerationBestSolutionTakenFrom = 0;                 

            //if (Parameters.TrackEvolutionSteps)
            //{
            //    //EvolutionStepsSimple.Add(BasePopulation.First());
            //}

            var numberOfGenerations = Parameters.NumberOfGenerations;

            Stoper.Restart();

            for (var i = 0; i < numberOfGenerations; i++)
            {
                //MutationRuleSupervisor.SaveBestFitness(BasePopulation.First());

                if (Parameters.TrackEvolutionSteps)
                    CurrentEvolutionStep = new EvolutionStep(BasePopulation, OffspringPopulation.Length);

                Evolve(evaluator);

                if (Parameters.TrackEvolutionSteps)
                    EvolutionSteps.Add(i, CurrentEvolutionStep);

                if (bestSolution.FitnessScore < BasePopulation.First().FitnessScore)
                {
                    bestSolution = BasePopulation.First();
                    numberOfGenerationBestSolutionTakenFrom = i;
                }
                //MutationRuleSupervisor.EnsureRuleFullfillment(BasePopulation);

                //if (Parameters.TrackEvolutionSteps)
                //    EvolutionStepsSimple.Add(BasePopulation.First());
                //Console.WriteLine(BasePopulation.First().FitnessScore);
            }
            
            //Console.WriteLine("###########################################################################");
            //Console.WriteLine("###########################################################################");
            //Console.WriteLine("###########################################################################");

            //Console.WriteLine("Best solution taken from generation " + numberOfGenerationBestSolutionTakenFrom);

            //Console.WriteLine("###########################################################################");
                        
            //Console.Write("Best solution stdDevs = [");
            //foreach (var stdDeviationsCoefficient in bestSolution.StdDeviationsCoefficients)
            //{
            //    Console.Write(stdDeviationsCoefficient + ", ");
            //}
            //Console.WriteLine("]\n"); 
                      
            //Console.WriteLine("###########################################################################");

            //Console.Write("Last evolved best solution stdDevs = [");
            //foreach (var stdDeviationsCoefficient in BasePopulation.First().StdDeviationsCoefficients)
            //{
            //    Console.Write(stdDeviationsCoefficient + ", ");
            //}
            //Console.WriteLine("]\n");

            //Console.WriteLine("###########################################################################");
            //Console.WriteLine("###########################################################################");
            //Console.WriteLine("###########################################################################");

            Stoper.Stop();

            Statistics.NumberOfGenerationBestSolutionTakenFrom = numberOfGenerationBestSolutionTakenFrom;
            Statistics.TotalEvolutionTime = Stoper.Elapsed;
            Statistics.MeanSingleGenerationEvolutionTime = TimeSpan.FromTicks(Statistics.TotalEvolutionTime.Ticks / numberOfGenerations);
            //Statistics.MeanStdDevsMutationTime = TimeSpan.FromTicks(Statistics.MeanStdDevsMutationTime.Ticks / numberOfGenerations);
            //Statistics.MeanRotationsMutationTime = TimeSpan.FromTicks(Statistics.MeanRotationsMutationTime.Ticks / numberOfGenerations);
            //Statistics.MeanObjectMutationTime = TimeSpan.FromTicks(Statistics.MeanObjectMutationTime.Ticks / numberOfGenerations);
            //Statistics.MeanEvaluationTime = TimeSpan.FromTicks(Statistics.MeanEvaluationTime.Ticks / numberOfGenerations);
            //Statistics.MeanSurvivorsSelectionTime = TimeSpan.FromTicks(Statistics.MeanSurvivorsSelectionTime.Ticks / numberOfGenerations);
            Statistics.LastFitnessScore = BasePopulation.First().FitnessScore;
            Statistics.BestFitnessScore = bestSolution.FitnessScore;

            Stoper.Reset();
            
            //return BasePopulation.First();
            return bestSolution;
        }

        protected Solution[] InitializePopulations(EvaluatorBase evaluator, ISeedingProcessor seedingProcessor = null)
        {
            var population = PopulationGenerator.GeneratePopulation(Parameters);

            if (seedingProcessor != null)
            {
                Stoper.Restart();
                population = seedingProcessor.Seed(population);
                Stoper.Stop();
                Statistics.SeedingTime = Stoper.Elapsed;
            }
                
            foreach (var solution in population)
                solution.FitnessScore = evaluator.Evaluate(solution);

            for (var i = 0; i < OffspringPopulation.Length; i++)
                OffspringPopulation[i] = SolutionsFactory.Create(Parameters);

            return population;
        }

        protected abstract void Evolve(EvaluatorBase evaluator);
    }
}
