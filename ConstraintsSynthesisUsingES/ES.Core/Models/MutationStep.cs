using ES.Core.Models.Solutions;
using ES.Core.Utils;

namespace ES.Core.Models
{
    public class MutationStep
    {
        public MutationStep(int parentIndex, Solution parent)
        {
            ParentIndex = parentIndex;
            Parent = parent.DeepCopyByExpressionTree();
        }

        public int ParentIndex { get; set; }
        public Solution Parent { get; set; }
        public Solution Offspring { get; set; }
    }
}