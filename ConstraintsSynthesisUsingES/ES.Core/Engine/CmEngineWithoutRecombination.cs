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
    public class CmEngineWithoutRecombination : UmEngineWithoutRecombination
    {
        protected MutatorBase RotationsMutator;

        public CmEngineWithoutRecombination(EvolutionParameters evolutionParameters, IGenericFactory<Solution> solutionsFactory, PopulationGeneratorBase populationGenerator, MutatorBase objectMutator, MutatorBase stdDeviationsMutator, MutationRuleSupervisorBase mutationRuleSupervisor, ParentsSelectorBase parentsSelector, SurvivorsSelectorBase survivorsSelector, Statistics statistics, Stopwatch stoper, MutatorBase rotationsMutator) : base(evolutionParameters, solutionsFactory, populationGenerator, objectMutator, stdDeviationsMutator, mutationRuleSupervisor, parentsSelector, survivorsSelector, statistics, stoper)
        {
            RotationsMutator = rotationsMutator;
        }
        //HACKS TODO
        protected override void Evolve(EvaluatorBase evaluator)
        {
            var offspringPopulationSize = Parameters.OffspringPopulationSize;

            //var stdDevsMutationTime = TimeSpan.Zero;
            //var rotationsMutationTime = TimeSpan.Zero;
            //var objectMutationTime = TimeSpan.Zero;
            //var evaluationTime = TimeSpan.Zero;

            //var stoper = new Stopwatch();

            for (var i = 0; i < offspringPopulationSize; i++)
            {
                OffspringPopulation[i] = ParentsSelector.Select(BasePopulation);

                if (Parameters.TrackEvolutionSteps)
                    CurrentMutation = new MutationStep(ParentsSelector.LastSelectedParentIndex, OffspringPopulation[i]);

                //stoper.Restart();
                OffspringPopulation[i] = StdDeviationsMutator.Mutate(OffspringPopulation[i]);
                //stoper.Stop();
                //stdDevsMutationTime += stoper.Elapsed;

                //stoper.Restart();
                OffspringPopulation[i] = RotationsMutator.Mutate(OffspringPopulation[i]);
                //stoper.Stop();
                //rotationsMutationTime += stoper.Elapsed;

                //stoper.Restart();
                OffspringPopulation[i] = ObjectMutator.Mutate(OffspringPopulation[i]);
                //stoper.Stop();
                //objectMutationTime += stoper.Elapsed;

                //stoper.Restart();
                OffspringPopulation[i].FitnessScore = evaluator.Evaluate(OffspringPopulation[i]);
                //stoper.Stop();
                //evaluationTime += stoper.Elapsed;

                if (Parameters.TrackEvolutionSteps)
                {
                    CurrentMutation.Offspring = OffspringPopulation[i].DeepCopyByExpressionTree();
                    CurrentEvolutionStep.Mutations.Add(CurrentMutation);
                }                   
            }

            //stoper.Restart();
            BasePopulation = SurvivorsSelector.Select(BasePopulation, OffspringPopulation);
            //stoper.Stop();

            //Statistics.MeanSurvivorsSelectionTime += stoper.Elapsed;
            //Statistics.MeanStdDevsMutationTime += TimeSpan.FromTicks(stdDevsMutationTime.Ticks / offspringPopulationSize);
            //Statistics.MeanRotationsMutationTime += TimeSpan.FromTicks(rotationsMutationTime.Ticks / offspringPopulationSize);
            //Statistics.MeanObjectMutationTime += TimeSpan.FromTicks(objectMutationTime.Ticks / offspringPopulationSize);
            //Statistics.MeanEvaluationTime += TimeSpan.FromTicks(evaluationTime.Ticks / offspringPopulationSize);

            if (Parameters.TrackEvolutionSteps)
                CurrentEvolutionStep.NewPopulation = BasePopulation.DeepCopyByExpressionTree();
        }
    }
}
