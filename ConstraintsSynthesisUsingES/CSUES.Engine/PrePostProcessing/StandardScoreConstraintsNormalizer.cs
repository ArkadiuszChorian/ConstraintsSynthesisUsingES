using CSUES.Engine.Models.Constraints;

namespace CSUES.Engine.PrePostProcessing
{
    public class StandardScoreConstraintsNormalizer : StandardScoreConstraintsNormalizationProcessor
    {
        public StandardScoreConstraintsNormalizer(double[] means, double[] standardDeviations) : base(means, standardDeviations)
        {
        }

        public override Constraint[] ApplyProcessing(Constraint[] constraints)
        {
            throw new System.NotImplementedException();
        }
    }
}