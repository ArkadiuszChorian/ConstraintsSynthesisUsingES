using CSUES.Engine.Models.Constraints;

namespace CSUES.Engine.PrePostProcessing
{
    public abstract class StandardScoreConstraintsNormalizationProcessor : IProcessor<Constraint[]>
    {
        protected readonly double[] Means;
        protected readonly double[] StandardDeviations;

        protected StandardScoreConstraintsNormalizationProcessor(double[] means, double[] standardDeviations)
        {
            Means = means;
            StandardDeviations = standardDeviations;
        }

        public abstract Constraint[] ApplyProcessing(Constraint[] objectToProcess);
    }
}