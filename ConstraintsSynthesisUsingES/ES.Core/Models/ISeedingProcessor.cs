using ES.Core.Models.Solutions;

namespace ES.Core.Models
{
    public interface ISeedingProcessor
    {
        Solution[] Seed(Solution[] solutions);
    }
}