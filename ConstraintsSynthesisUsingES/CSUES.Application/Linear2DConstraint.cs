namespace ES.Core.Constraints
{
    public class Linear2DConstraint : LinearConstraint
    {
        public Linear2DConstraint(double[] termsCoefficients, double limitingValue) : base(termsCoefficients, limitingValue)
        {
        }

        public Linear2DConstraint(double a, double b, InequalityValue inequalityValue) : this(new []{-a, (double)inequalityValue * 1.0}, (double)inequalityValue * b)
        {           
        }

        public enum InequalityValue
        {
            OverLine = -1,
            UnderLine = 1
        }
    }
}
