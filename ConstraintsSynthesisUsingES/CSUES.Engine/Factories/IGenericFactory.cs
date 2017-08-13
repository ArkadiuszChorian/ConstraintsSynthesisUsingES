using CSUES.Engine.Models;

namespace CSUES.Engine.Factories
{
    public interface IGenericFactory<out T>
    {
        T Create(ExperimentParameters experimentParameters);
    }
}