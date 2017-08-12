using System;

namespace ES.Core.Models.Solutions
{
    public class Solution : IComparable<Solution>
    {
        public Solution(int vectorSize)
        {
            ObjectCoefficients = new double[vectorSize];
        }

        public double[] ObjectCoefficients { get; set; }
        public double OneStepStdDeviation { get; set; }
        public double[] StdDeviationsCoefficients { get; set; }
        public double[] RotationsCoefficients { get; set; }
        public double FitnessScore { get; set; }

        public int CompareTo(Solution other)
        {
            //Minus sign to sort descending
            return -FitnessScore.CompareTo(other.FitnessScore);
        }
    }
}
