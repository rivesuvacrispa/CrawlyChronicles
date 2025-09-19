using Definitions;
using Gameplay.Genes;
using Timeline;
using UnityEngine;

namespace Gameplay.Food.Foodbeds
{
    public class BlueFungi : Fungi
    {
        protected override void OnEatenByPlayer()
        {
            GlobalDefinitions.DropGenesRandomly(Position, (GeneType)Random.Range(0, 3), 1);
        }

        public override bool CanSpawn(float rnd) => !TimeManager.IsDay;
    }
}