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
        //protected IMutationRuleSupervisor MutationRuleSupervisor;
        protected ParentsSelectorBase ParentsSelector;
        protected SurvivorsSelectorBase SurvivorsSelector;
        protected IGenericFactory<Solution> SolutionsFactory;
        protected Solution[] BasePopulation;
        protected Solution[] OffspringPopulation;
        protected Stopwatch Stoper;

        protected EngineBase(EvolutionParameters evolutionParameters, IGenericFactory<Solution> solutionsFactory, PopulationGeneratorBase populationGenerator, MutatorBase objectMutator, MutatorBase stdDeviationsMutator, ParentsSelectorBase parentsSelector, SurvivorsSelectorBase survivorsSelector, Statistics statistics, Stopwatch stoper)
        {
            Parameters = evolutionParameters;
            SolutionsFactory = solutionsFactory;
            PopulationGenerator = populationGenerator;
            ObjectMutator = objectMutator;
            StdDeviationsMutator = stdDeviationsMutator;
            ParentsSelector = parentsSelector;
            SurvivorsSelector = survivorsSelector;
            Statistics = statistics;
            Stoper = stoper;

            BasePopulation = new Solution[evolutionParameters.BasePopulationSize];
            OffspringPopulation = new Solution[evolutionParameters.OffspringPopulationSize]; 
            EvolutionSteps = new List<Solution>(evolutionParameters.NumberOfGenerations);
              
            MersenneTwister.Initialize(evolutionParameters.Seed);       
        }
        
        public EvolutionParameters Parameters { get; set; }       
        public Statistics Statistics { get; set; }      
        public IList<Solution> EvolutionSteps { get; }
            
        public Solution RunEvolution(IEvaluator evaluator)
        {
            var offspringPopulationSize = Parameters.OffspringPopulationSize;
            var numberOfGenerations = Parameters.NumberOfGenerations;

            BasePopulation = PopulationGenerator.GeneratePopulation(Parameters);

            for (var i = 0; i < offspringPopulationSize; i++)
                OffspringPopulation[i] = SolutionsFactory.Create(Parameters);            
            
            Stoper.Restart();
          
            if (Parameters.TrackEvolutionSteps)
                EvolutionSteps.Add(BasePopulation.First());

            for (var i = 0; i < numberOfGenerations; i++)
            {
                Evolve(evaluator);

                if (Parameters.TrackEvolutionSteps)
                    EvolutionSteps.Add(BasePopulation.First());
            }
                
            Stoper.Stop();

            Statistics.TotalEvolutionTime = Stoper.Elapsed;
            Statistics.MeanSingleGenerationEvolutionTime = TimeSpan.FromTicks(Statistics.TotalEvolutionTime.Ticks / numberOfGenerations);
            Statistics.BestFitnessScore = BasePopulation.First().FitnessScore;

            Stoper.Reset();
            
            return BasePopulation.First();
        }

        protected abstract void Evolve(IEvaluator evaluator);
    }
}
