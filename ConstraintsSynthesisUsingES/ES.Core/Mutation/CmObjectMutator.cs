using System;
using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.Mutation
{
    public class CmObjectMutator : MutatorBase
    {
        private readonly double[] _zeroMeans;

        public CmObjectMutator(EvolutionParameters evolutionParameters)
        {
            _zeroMeans = new double[evolutionParameters.ObjectVectorSize];
        }

        public override Solution Mutate(Solution solution)
        {
            var vectorSize = solution.ObjectCoefficients.Length;
            var covarianceMatrix = new double[vectorSize, vectorSize];

            for (var i = 0; i < vectorSize; i++)
            {
                for (var j = 0; j < vectorSize; j++)
                {
                    if (i == j)
                    {
                        covarianceMatrix[i, j] = Math.Pow(solution.StdDeviationsCoefficients[i], 2);
                    }
                    else
                    {
                        covarianceMatrix[i, j] = (Math.Pow(solution.StdDeviationsCoefficients[i], 2) - Math.Pow(solution.StdDeviationsCoefficients[j], 2)) * Math.Tan(2 * solution.RotationsCoefficients[FromMatrixToVector(i, j, vectorSize)]) / 2;
                    }
                }
            }

            var mutationVector = new RobustMultivariateNormalDistribution(_zeroMeans, covarianceMatrix).Generate();

            for (var i = 0; i < solution.ObjectCoefficients.Length; i++)
            {
                solution.ObjectCoefficients[i] += mutationVector[i];
            }

            return solution;
        }

        private static int FromMatrixToVector(int i, int j, int n)
        {
            if (i <= j)
                return i * n - (i - 1) * i / 2 + j - i;
            return j * n - (j - 1) * j / 2 + i - j;
        }
    }
}
