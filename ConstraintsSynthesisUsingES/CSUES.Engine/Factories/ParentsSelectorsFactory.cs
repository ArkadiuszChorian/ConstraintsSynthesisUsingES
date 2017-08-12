using System;
using ES.Core.Enums;
using ES.Core.Models;
using ES.Core.Selection;

namespace ES.Core.Factories
{
    public class ParentsSelectorsFactory : IGenericFactory<IParentsSelector>
    {
        public IParentsSelector Create(EvolutionParameters evolutionParameters)
        {
            switch ((ParentsSelectionType) evolutionParameters.TypeOfParentsSelection)
            {
                case ParentsSelectionType.Random:
                    return new ParentsRandomSelector(evolutionParameters);
                case ParentsSelectionType.Uniform:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}