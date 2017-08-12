using ES.Core.Models.Terms;

namespace ES.Core.Factories.Interfaces
{
    public interface ITermsFactory
    {
        Term Create(int termType, double coefficient, double power = 1);
    }
}