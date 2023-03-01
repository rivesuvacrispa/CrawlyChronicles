using Gameplay.AI;
using Gameplay.Food;
using UnityEngine;

namespace Gameplay.Enemies
{
    public class DiggingWasp : Enemy
    {
        
        private static readonly int AttackAnimHash = Animator.StringToHash("DiggingWaspAttack");
        // private readonly int AttackAnimHash = Animator.StringToHash("DiggingWaspAttack");

        private float prevX;
        
        public override void OnMapEntered()
        {
            stateController.SetState(AIState.Wander);
        }

        public override void OnPlayerLocated()
        {
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
        }

        public override void OnFoodLocated(FoodBed foodBed)
        {
        }

        protected override void OnDamageTaken()
        {
        }

        private void LateUpdate()
        {
            float currentX = rb.position.x;
            float diff = currentX - prevX;
            if (diff < -0.01) spriteRenderer.flipX = true;
            else if (diff > 0.01) spriteRenderer.flipX = false;
            prevX = currentX;
        }

        private void SaveXPosition() => prevX = rb.position.x;
    }
}