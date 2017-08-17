using System.Diagnostics;
using System.Linq;
using ES.Core.Factories;
using ES.Core.Models;
using ES.Core.Models.Solutions;
using ES.Core.Mutation;
using ES.Core.MutationSupervison;
using ES.Core.PopulationGeneration;
using ES.Core.Selection;

namespace ES.Core.Engine
{
    public class CmEngineWithoutRecombination : UmEngineWithoutRecombination
    {
        protected MutatorBase RotationsMutator;

        public CmEngineWithoutRecombination(EvolutionParameters evolutionParameters, IGenericFactory<Solution> solutionsFactory, PopulationGeneratorBase populationGenerator, MutatorBase objectMutator, MutatorBase stdDeviationsMutator, MutationRuleSupervisorBase mutationRuleSupervisor, ParentsSelectorBase parentsSelector, SurvivorsSelectorBase survivorsSelector, Statistics statistics, Stopwatch stoper, MutatorBase rotationsMutator) : base(evolutionParameters, solutionsFactory, populationGenerator, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsSelector, survivorsSelector, statistics, stoper)
        {
            RotationsMutator = rotationsMutator;
        }
        //HACKS TODO
        protected override void Evolve(IEvaluator evaluator)
        {
            var offspringPopulationSize = Parameters.OffspringPopulationSize;

            for (var i = 0; i < offspringPopulationSize; i++)
            {
                OffspringPopulation[i] = ParentsSelector.Select(BasePopulation);
                if (AnyNan(OffspringPopulation[i]))
                    Debugger.Break();

                OffspringPopulation[i] = StdDeviationsMutator.Mutate(OffspringPopulation[i]);
                if (AnyNan(OffspringPopulation[i]))
                    Debugger.Break();
                OffspringPopulation[i] = RotationsMutator.Mutate(OffspringPopulation[i]);
                if (AnyNan(OffspringPopulation[i]))
                    Debugger.Break();
                OffspringPopulation[i] = ObjectMutator.Mutate(OffspringPopulation[i]);
                if (AnyNan(OffspringPopulation[i]))
                    Debugger.Break();

                OffspringPopulation[i].FitnessScore = evaluator.Evaluate(OffspringPopulation[i]);
                if (AnyNan(OffspringPopulation[i]))
                    Debugger.Break();
            }

            BasePopulation = SurvivorsSelector.Select(BasePopulation, OffspringPopulation);
        }

        private bool AnyNan(Solution solution)
        {
            return solution.ObjectCoefficients.Any(oc => oc.Equals(double.NaN));
        }
    }
}
