using System.Collections;
using Gameplay.AI;
using Gameplay.Breeding;
using Gameplay.Food;
using UnityEngine;
using Util;

namespace Gameplay.Enemies.Enemies
{
    public class ScarabBeetle : Enemy
    {
        public override void OnMapEntered()
        {
            StateController.SetState(AIState.Wander);
        }

        public override void OnPlayerLocated()
        {
            StartCoroutine(DefenseRoutine(5f));
        }

        private IEnumerator DefenseRoutine(float duration)
        {
            StateController.TakeMoveControl();
            StateController.SetState(AIState.None);
            float t = duration;
            while (t > 0)
            {
                rb.RotateTowardsPosition(2 * rb.position - Player.PlayerPhysicsBody.Position, 5f);
                t -= Time.deltaTime;
                yield return null;
            }
            StateController.ReturnMoveControl();
            StateController.SetState(AIState.Wander);
        }

        public override void OnEggsLocated(EggBed eggBed) { }

        public override void OnFoodLocated(Foodbed foodBed) { }

        protected override void DamageTaken() { }
    }
}