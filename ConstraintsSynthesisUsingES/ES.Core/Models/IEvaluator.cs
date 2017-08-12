using ES.Core.Models.Solutions;

namespace ES.Core.Models
{
    public interface IEvaluator
    {
        double Evaluate(Solution solution);
    }
}
