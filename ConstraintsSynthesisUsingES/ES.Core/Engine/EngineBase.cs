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
            EvolutionSteps = new List<Solution>(evolutionParameters.NumberOfGenerations);                              
        }
        
        public EvolutionParameters Parameters { get; set; }       
        public Statistics Statistics { get; set; }      
        public IList<Solution> EvolutionSteps { get; }
            
        public Solution RunEvolution(IEvaluator evaluator)
        {
            var offspringPopulationSize = Parameters.OffspringPopulationSize;
            var numberOfGenerations = Parameters.NumberOfGenerations;

            BasePopulation = PopulationGenerator.GeneratePopulation(Parameters);

            var bestSolution = BasePopulation.First();
            var numberOfGenerationBestSolutionTakenFrom = 0;

            for (var i = 0; i < offspringPopulationSize; i++)
                OffspringPopulation[i] = SolutionsFactory.Create(Parameters);            
            
            Stoper.Restart();
          
            if (Parameters.TrackEvolutionSteps)
                EvolutionSteps.Add(BasePopulation.First());

            for (var i = 0; i < numberOfGenerations; i++)
            {
                //MutationRuleSupervisor.SaveBestFitness(BasePopulation.First());

                Evolve(evaluator);

                if (bestSolution.FitnessScore < BasePopulation.First().FitnessScore)
                {
                    bestSolution = BasePopulation.First();
                    numberOfGenerationBestSolutionTakenFrom = i;
                }
                //MutationRuleSupervisor.EnsureRuleFullfillment(BasePopulation);

                if (Parameters.TrackEvolutionSteps)
                    EvolutionSteps.Add(BasePopulation.First());
            }
            
            Console.WriteLine("###########################################################################");
            Console.WriteLine("###########################################################################");
            Console.WriteLine("###########################################################################");

            Console.WriteLine("Best solution taken from generation " + numberOfGenerationBestSolutionTakenFrom);

            Console.WriteLine("###########################################################################");
                        
            Console.Write("Best solution stdDevs = [");
            foreach (var stdDeviationsCoefficient in bestSolution.StdDeviationsCoefficients)
            {
                Console.Write(stdDeviationsCoefficient + ", ");
            }
            Console.WriteLine("]\n"); 
                      
            Console.WriteLine("###########################################################################");

            Console.Write("Last evolved best solution stdDevs = [");
            foreach (var stdDeviationsCoefficient in BasePopulation.First().StdDeviationsCoefficients)
            {
                Console.Write(stdDeviationsCoefficient + ", ");
            }
            Console.WriteLine("]\n");

            Console.WriteLine("###########################################################################");
            Console.WriteLine("###########################################################################");
            Console.WriteLine("###########################################################################");

            Stoper.Stop();

            Statistics.TotalEvolutionTime = Stoper.Elapsed;
            Statistics.MeanSingleGenerationEvolutionTime = TimeSpan.FromTicks(Statistics.TotalEvolutionTime.Ticks / numberOfGenerations);
            Statistics.BestFitnessScore = BasePopulation.First().FitnessScore;

            Stoper.Reset();
            
            //return BasePopulation.First();
            return bestSolution;
        }

        protected abstract void Evolve(IEvaluator evaluator);
    }
}
