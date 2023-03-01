using Definitions;
using UnityEngine;

namespace Gameplay.Food
{
    public class BlueFungi : FoodBed
    {
        protected override void OnEatenByPlayer()
        {
            Player.Manager.Instance.AddHealthPercent(0.1f);
            if(Random.value >= 0.5f) GlobalDefinitions.CreateRandomGeneDrop(Position);
        }
    }
}