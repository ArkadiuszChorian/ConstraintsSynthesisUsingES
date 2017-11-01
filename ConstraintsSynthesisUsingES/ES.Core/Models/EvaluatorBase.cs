using ES.Core.Models.Solutions;

namespace ES.Core.Models
{
    public abstract class EvaluatorBase
    {
        public abstract double Evaluate(Solution solution);
    }
}
