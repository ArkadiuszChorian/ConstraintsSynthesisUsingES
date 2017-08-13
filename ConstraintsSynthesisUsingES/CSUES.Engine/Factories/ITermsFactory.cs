using CSUES.Engine.Models.Terms;

namespace CSUES.Engine.Factories
{
    public interface ITermsFactory
    {
        Term Create(int termType, double coefficient, double power = 1);
    }
}