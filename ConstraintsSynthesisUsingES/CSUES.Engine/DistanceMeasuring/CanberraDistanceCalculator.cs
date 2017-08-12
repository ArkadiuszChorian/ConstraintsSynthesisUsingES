using System;

namespace ES.Core.DistanceMeasuring
{
    public class CanberraDistanceCalculator : IDistanceCalculator
    {
        public double Calculate(double[] vector1, double[] vector2)
        {
            var distance = 0.0;
            var numberOfDimensions = vector1.Length;

            for (var i = 0; i < numberOfDimensions; i++)
            {
                var divider = Math.Abs(vector1[i]) + Math.Abs(vector2[i]);

                if(divider == 0.0) continue;

                distance += Math.Abs(vector1[i] - vector2[i]) / divider;
            }

            return distance;
        }
    }
}
