using ES.Core.Models;

namespace ES.Core.Factories
{
    public interface IGenericFactory<out T>
    {
        T Create(EvolutionParameters evolutionParameters);
    }
}