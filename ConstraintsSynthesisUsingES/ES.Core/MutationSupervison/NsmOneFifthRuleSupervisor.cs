using System.Collections.Generic;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.MutationSupervison
{
    public class NsmOneFifthRuleSupervisor : IMutationRuleSupervisor
    {
        private const double OneFifthRatio = 0.2;

        private double _rememberedFitnessScore;

        //public NsmOneFifthRuleSupervisor(ExperimentParameters experimentParameters)
        //{
        //    StdDeviationsScalingFactor = experimentParameters.OneFifthRuleScalingFactor;
        //    OneFifthRuleCheckInterval = experimentParameters.OneFifthRuleCheckInterval;
        //}

        public int SuccesfulMutationsNumber { get; set; }
        public int MutationsNumber { get; set; }
        public int GenerationsNumber { get; set; }
        public int OneFifthRuleCheckInterval { get; set; }
        public double StdDeviationsScalingFactor { get; set; }

        public IList<Solution> EnsureRuleFullfillment(IList<Solution> solutions)
        {
            if (GenerationsNumber < OneFifthRuleCheckInterval)
                return solutions;

            var succesfulMutationsRatio = (double)SuccesfulMutationsNumber / MutationsNumber;

            if (succesfulMutationsRatio > OneFifthRatio)
            {
                foreach (var solution in solutions)
                {
                    var stdDeviationsCoefficientsVectorSize = solution.StdDeviationsCoefficients.Length;

                    for (var i = 0; i < stdDeviationsCoefficientsVectorSize; i++)
                    {
                        solution.StdDeviationsCoefficients[i] /= StdDeviationsScalingFactor;
                    }
                }
            }

            if (succesfulMutationsRatio < OneFifthRatio)
            {
                foreach (var solution in solutions)
                {
                    var stdDeviationsCoefficientsVectorSize = solution.StdDeviationsCoefficients.Length;

                    for (var i = 0; i < stdDeviationsCoefficientsVectorSize; i++)
                    {
                        solution.StdDeviationsCoefficients[i] *= StdDeviationsScalingFactor;
                    }
                }
            }

            GenerationsNumber = 0;
            MutationsNumber = 0;
            SuccesfulMutationsNumber = 0;

            return solutions;
        }

        public void RemeberSolutionParameters(Solution solution)
        {
            _rememberedFitnessScore = solution.FitnessScore;
        }

        public void IncrementMutationsNumber()
        {
            MutationsNumber++;
        }

        public void IncrementGenerationNumber()
        {
            GenerationsNumber++;
        }

        public void CompareNewSolutionParameters(Solution solution)
        {
            if (solution.FitnessScore > _rememberedFitnessScore)
            {
                SuccesfulMutationsNumber++;
            }
        }
    }
}
