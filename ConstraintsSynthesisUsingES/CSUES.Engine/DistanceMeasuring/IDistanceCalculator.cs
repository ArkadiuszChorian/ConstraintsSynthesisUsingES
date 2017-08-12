namespace ES.Core.DistanceMeasuring
{
    public interface IDistanceCalculator
    {
        double Calculate(double[] vector1, double[] vector2);
    }
}
