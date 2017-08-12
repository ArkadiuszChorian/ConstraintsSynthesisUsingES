namespace ES.Core.Models.Solutions
{
    public class CmSolution : NsmSolution
    {
        public CmSolution(int vectorSize) : base(vectorSize)
        {
            RotationsCoefficients = new double[vectorSize * (vectorSize - 1) / 2];
        }
    }
}