using Definitions;
using Gameplay.Genes;
using UnityEngine;

namespace Gameplay.Food.Foodbeds
{
    public class Succulent : Foodbed
    {
        protected override void OnEatenByPlayer()
        {
            GlobalDefinitions.DropGenesRandomly(Position, (GeneType)Random.Range(0, 3), 1);
        }

        public override bool CanSpawn(float random) => true;
    }
}