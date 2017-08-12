using ES.Core.Models;
using ES.Core.Models.Solutions;

namespace ES.Core.Evaluation
{
    public interface IEvaluator
    {
        Point[] PositivePoints { get; set; }
        Point[] NegativePoints { get; set; }
        //Point[] Points { get; set; }

        double Evaluate(Solution solution);
    }
}
