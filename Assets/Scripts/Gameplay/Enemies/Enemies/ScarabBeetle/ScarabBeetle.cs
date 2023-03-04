using System.Collections;
using Gameplay.AI;
using Gameplay.Food;
using UnityEngine;
using Util;

namespace Gameplay.Enemies
{
    public class ScarabBeetle : Enemy
    {
        public override void OnMapEntered()
        {
            stateController.SetState(AIState.Wander);
        }

        public override void OnPlayerLocated()
        {
            StartCoroutine(DefenseRoutine(5f));
        }

        private IEnumerator DefenseRoutine(float duration)
        {
            stateController.TakeMoveControl();
            stateController.SetState(AIState.None);
            float t = duration;
            while (t > 0)
            {
                rb.RotateTowardsPosition(2 * rb.position - Player.Movement.Position, 5f);
                t -= Time.deltaTime;
                yield return null;
            }
            stateController.ReturnMoveControl();
            stateController.SetState(AIState.Wander);
        }

        public override void OnEggsLocated(EggBed eggBed) { }

        public override void OnFoodLocated(FoodBed foodBed) { }

        protected override void OnDamageTaken() { }
    }
}