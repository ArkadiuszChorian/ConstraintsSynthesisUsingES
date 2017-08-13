using System;
using ES.Core.Enums;
using ES.Core.Models;
using ES.Core.Selection;

namespace ES.Core.Factories
{
    public class ParentsSelectorsFactory : IGenericFactory<ParentsSelectorBase>
    {
        public ParentsSelectorBase Create(EvolutionParameters evolutionParameters)
        {
            switch ((ParentsSelectionType) evolutionParameters.TypeOfParentsSelection)
            {
                case ParentsSelectionType.Random:
                    return new ParentsRandomSelector();
                case ParentsSelectionType.Even:
                    return new ParentsEvenSelector(evolutionParameters);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}