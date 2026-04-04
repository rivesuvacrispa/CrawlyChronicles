using Definitions;
using Gameplay.Genes;
using UnityEngine;

namespace Gameplay.Food.Foodbeds
{
    public class GeneDroppingFoodbed : Foodbed
    {
        protected override void OnEatenByPlayer()
        {
            GlobalDefinitions.DropGenesRandomly(Position, (GeneType)Random.Range(0, 3), 1);
        }
    }
}