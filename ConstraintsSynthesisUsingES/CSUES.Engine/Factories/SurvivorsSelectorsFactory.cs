using System;
using ES.Core.Enums;
using ES.Core.Models;
using ES.Core.Selection;

namespace ES.Core.Factories
{
    public class SurvivorsSelectorsFactory : IGenericFactory<SurvivorsSelectorBase>
    {
        public SurvivorsSelectorBase Create(EvolutionParameters evolutionParameters)
        {
            var typeOfSurvivorsSelection = (SurvivorsSelectionType) evolutionParameters.TypeOfSurvivorsSelection;

            switch (typeOfSurvivorsSelection)
            {
                case SurvivorsSelectionType.Distinct:
                    return new SurvivorsDistinctSelector(evolutionParameters);
                case SurvivorsSelectionType.Union:
                    return new SurvivorsUnionSelector(evolutionParameters);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}