namespace ES.Core.Models
{
    public class Point
    {
        public Point(int numberOfDimensions)
        {
            Coordinates = new double[numberOfDimensions];
        }

        public Point(int numberOfDimensions, ClassificationType classificationType)
        {
            Coordinates = new double[numberOfDimensions];
            ClassificationType = classificationType;
        }

        public double[] Coordinates { get; set; }
        public double DistanceToNearestNeighbour { get; set; }
        public ClassificationType ClassificationType { get; set; }
    }
}
