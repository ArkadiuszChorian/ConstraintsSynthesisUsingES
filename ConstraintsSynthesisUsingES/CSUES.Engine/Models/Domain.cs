namespace CSUES.Engine.Models
{
    public class Domain
    {
        public Domain(double lowerLimit, double upperLimit)
        {
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
        }

        public double LowerLimit { get; set; }
        public double UpperLimit { get; set; }
    }
}
