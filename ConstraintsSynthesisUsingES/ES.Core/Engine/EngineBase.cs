using System;
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
        protected IPopulationGenerator PopulationGenerator;
        protected MutatorBase ObjectMutator;
        protected MutatorBase StdDeviationsMutator;
        //protected IMutationRuleSupervisor MutationRuleSupervisor;
        protected ParentsSelectorBase ParentsSelector;
        protected SurvivorsSelectorBase SurvivorsSelector;
        protected IGenericFactory<Solution> SolutionsFactory;
        protected Solution[] BasePopulation;
        protected Solution[] OffspringPopulation;
        protected Stopwatch Stoper;

        protected EngineBase(EvolutionParameters evolutionParameters, IGenericFactory<Solution> solutionsFactory, IPopulationGenerator populationGenerator, MutatorBase objectMutator, MutatorBase stdDeviationsMutator, ParentsSelectorBase parentsSelector, SurvivorsSelectorBase survivorsSelector, EvolutionStatistics evolutionStatistics, Stopwatch stoper)
        {
            Parameters = evolutionParameters;
            SolutionsFactory = solutionsFactory;
            PopulationGenerator = populationGenerator;
            ObjectMutator = objectMutator;
            StdDeviationsMutator = stdDeviationsMutator;
            ParentsSelector = parentsSelector;
            SurvivorsSelector = survivorsSelector;
            Statistics = evolutionStatistics;
            Stoper = stoper;

            BasePopulation = new Solution[evolutionParameters.BasePopulationSize];
            OffspringPopulation = new Solution[evolutionParameters.OffspringPopulationSize]; 
              
            MersenneTwister.Initialize(evolutionParameters.Seed);       
        }
        
        public EvolutionParameters Parameters { get; set; }       
        public EvolutionStatistics Statistics { get; set; }
               
        public Solution RunEvolution(IEvaluator evaluator)
        {
            var offspringPopulationSize = Parameters.OffspringPopulationSize;
            var numberOfGenerations = Parameters.NumberOfGenerations;

            BasePopulation = PopulationGenerator.GeneratePopulation(Parameters);

            for (var i = 0; i < offspringPopulationSize; i++)
                OffspringPopulation[i] = SolutionsFactory.Create(Parameters);            

            Stoper.Restart();

            for (var i = 0; i < numberOfGenerations; i++)
                Evolve(evaluator);

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
