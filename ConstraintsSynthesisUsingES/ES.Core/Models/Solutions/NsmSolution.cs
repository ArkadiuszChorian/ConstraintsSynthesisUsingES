namespace ES.Core.Models.Solutions
{
    public class NsmSolution : Solution
    {
        public NsmSolution(int vectorSize) : base(vectorSize)
        {
            StdDeviationsCoefficients = new double[vectorSize];
        }
    }
}