using System;
using Gameplay.AI;
using Gameplay.AI.Locators;
using Gameplay.Breeding;
using Gameplay.Food;
using UnityEngine;

namespace Gameplay.Enemies.Enemies
{
    public class AntDefender : Enemy
    {
        public override void OnMapEntered()
        {
            StateController.SetState(AIState.Wander);
        }

        public override void OnPlayerLocated()
        {
            AttackPlayer();
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
        }

        public override void OnFoodLocated(Foodbed foodBed)
        {
        }

        protected override void OnDamageTaken()
        {
            AttackPlayer();
        }
    }
}