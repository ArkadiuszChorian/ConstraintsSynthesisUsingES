namespace ES.Core.PrePostProcessing
{
    public interface IProcessor<T>
    {
        T ApplyProcessing(T objectToProcess);
    }
}
