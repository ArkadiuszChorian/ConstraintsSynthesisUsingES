using System;
using System.Collections.Generic;
using System.Linq;
using ES.Core.Benchmarks;
using ES.Core.Constraints;
using ES.Core.Models;
using ES.Core.PointsGeneration;

namespace ES.Core.PrePostProcessing
{
    public class RedundantConstraintsRemover : IProcessor<Constraint[]>
    {
        private readonly IPointsGenerator _domainSpaceSampler;
        private readonly IBenchmark _benchmark;
        private readonly long _numberOfPointsToGenerate;
        private readonly int _maxNumberOfPointsInSingleArray;

        public RedundantConstraintsRemover(IPointsGenerator pointsGenerator, IBenchmark benchmark, ExperimentParameters experimentParameters)
        {
            _domainSpaceSampler = pointsGenerator;
            _benchmark = benchmark;
            _maxNumberOfPointsInSingleArray = experimentParameters.MaxNumberOfPointsInSingleArray;
            
            var numberOfDimensions = benchmark.Domains.Length;
            var domains = benchmark.Domains;
            var domainSamplingStep = experimentParameters.DomainSamplingStep;
            var temp = 1.0;

            for (var i = 0; i < numberOfDimensions; i++)
            {
                temp *= (domains[i].UpperLimit - domains[i].LowerLimit) / domainSamplingStep;
            }

            _numberOfPointsToGenerate = (long) temp;
        }

        public Constraint[] ApplyProcessing(Constraint[] constraints)
        {
            var count = 1;
            var numberOfPointsInSingleArray = (int)_numberOfPointsToGenerate;

            if (_numberOfPointsToGenerate > _maxNumberOfPointsInSingleArray)
            {
                count = (int)Math.Ceiling((double)_numberOfPointsToGenerate / _maxNumberOfPointsInSingleArray);
                numberOfPointsInSingleArray = _maxNumberOfPointsInSingleArray;                
            }           
            
            var allConstraints = constraints.ToList();
            var reducedConstraints = new List<Constraint>();

            for (var i = 0; i < count; i++)
            {
                var points = _domainSpaceSampler.GeneratePoints(numberOfPointsInSingleArray, _benchmark);
                var numberOfPoints = points.Length;

                for (var j = 0; j < numberOfPoints; j++)
                {
                    var numberOfConstraints = allConstraints.Count;
                    var isCutByOneConstraint = false;
                    Constraint obligatoryConstraint = null;

                    for (var k = 0; k < numberOfConstraints; k++)
                    {
                        if (allConstraints[k].IsSatisfyingConstraint(points[j])) continue;

                        if (isCutByOneConstraint)
                        {
                            isCutByOneConstraint = false;
                            break;
                        }

                        isCutByOneConstraint = true;
                        obligatoryConstraint = allConstraints[k];
                    }

                    if (isCutByOneConstraint && !reducedConstraints.Contains(obligatoryConstraint))
                    {
                        reducedConstraints.Add(obligatoryConstraint);
                    }
                }
            }
            
            return reducedConstraints.ToArray();
        }
    }
}
