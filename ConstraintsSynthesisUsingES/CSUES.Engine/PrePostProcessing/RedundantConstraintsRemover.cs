using System;
using System.Collections.Generic;
using System.Linq;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using CSUES.Engine.PointsGeneration;

namespace CSUES.Engine.PrePostProcessing
{
    public class RedundantConstraintsRemover : IProcessor<Constraint[]>
    {
        private readonly PointsGenerator _domainSpaceSampler;
        private readonly Domain[] _domains;
        private readonly long _numberOfDomainSamples;
        private readonly int _maxNumberOfPointsInSingleArray;

        public RedundantConstraintsRemover(PointsGenerator pointsGenerator, Domain[] domains, ExperimentParameters experimentParameters)
        {
            _domainSpaceSampler = pointsGenerator;
            _domains = domains;
            _maxNumberOfPointsInSingleArray = experimentParameters.MaxNumberOfPointsInSingleArray;
            _numberOfDomainSamples = experimentParameters.NumberOfDomainSamples;

            //var numberOfDimensions = benchmark.Domains.Length;
            //var domains = benchmark.Domains;
            //var domainSamplingStep = experimentParameters.DomainSamplingStep;
            //var temp = 1.0;

            //for (var i = 0; i < numberOfDimensions; i++)
            //{
            //    temp *= (domains[i].UpperLimit - domains[i].LowerLimit) / domainSamplingStep;
            //}

            //_numberOfPointsToGenerate = (long) temp;
        }

        public Constraint[] ApplyProcessing(Constraint[] constraints)
        {
            var count = 1;
            var numberOfPointsInSingleArray = (int)_numberOfDomainSamples;

            if (_numberOfDomainSamples > _maxNumberOfPointsInSingleArray)
            {
                count = (int)Math.Ceiling((double)_numberOfDomainSamples / _maxNumberOfPointsInSingleArray);
                numberOfPointsInSingleArray = _maxNumberOfPointsInSingleArray;                
            }           
            
            var allConstraints = constraints.ToList();
            var reducedConstraints = new List<Constraint>();

            for (var i = 0; i < count; i++)
            {
                var points = _domainSpaceSampler.GeneratePoints(numberOfPointsInSingleArray, _domains);
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
