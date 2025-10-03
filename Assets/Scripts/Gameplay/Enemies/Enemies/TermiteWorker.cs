using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.AI;
using Gameplay.Breeding;
using Gameplay.Food;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Enemies.Enemies
{
    public class TermiteWorker : AntEggStealer
    {
        [SerializeField] private SpriteRenderer foodRenderer;
        
        private bool holdingFood;
        
        public override void OnMapEntered()
        {
            stateController.DismissLocator();
            stateController.SetState(AIState.Wander);
            EnableLocateTask(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask EnableLocateTask(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(4 + Random.value * 2), cancellationToken: cancellationToken);

            stateController.UndismissLocator();
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
            if (holdingFood || holdingEgg is not null) return;
            
            base.OnEggsLocated(eggBed);
        }

        public override void OnPlayerLocated() { }

        public override void OnFoodLocated(Foodbed foodBed)
        {
            if (holdingFood || holdingEgg is not null) return;
            
            stateController.SetState(AIState.Follow, 
                followTarget: foodBed,
                onTargetReach: () =>
                {
                    if (foodBed.Eat())
                        PickupFood();
                    else
                        stateController.SetState(AIState.Wander);
                },
                reachDistance: 1f);
        }

        private void PickupFood()
        {
            holdingFood = true;
            foodRenderer.enabled = true;
            stateController.SetState(AIState.Flee);
        }

        protected override void OnDamageTaken()
        {
            if (holdingFood) return;
            base.OnDamageTaken();
        }

        public override void Die()
        {
            base.Die();
            if (holdingFood)
            {
                foodRenderer.enabled = false;
            }
        }
    }
}