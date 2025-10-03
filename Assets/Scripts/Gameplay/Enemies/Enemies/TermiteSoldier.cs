using Gameplay.AI;
using Gameplay.AI.Locators;
using Gameplay.Breeding;
using Gameplay.Food;
using UnityEngine;

namespace Gameplay.Enemies.Enemies
{
    public class TermiteSoldier : Enemy
    {
        [SerializeField] private Locator locator;
        
        private TermiteWorker currentFollowTarget;
        
        
        
        private void OnEnable() => locator.OnTargetLocated += OnTargetLocated;
        private void OnDisable() => locator.OnTargetLocated -= OnTargetLocated;

        private void OnTargetLocated(ILocatorTarget target)
        {
            if (target is not TermiteWorkerLocatorTarget workerLocatorTarget) return;
            OnWorkerLocated(workerLocatorTarget.TermiteWorker);
        }

        private void OnWorkerLocated(TermiteWorker worker)
        {
            if (currentFollowTarget is not null) return;

            currentFollowTarget = worker;
            currentFollowTarget.OnProviderDestroy += OnFollowTargetDestroy;
            
            stateController.SetState(AIState.Follow, worker, reachDistance: 1f);
        }

        private void OnFollowTargetDestroy()
        {
            currentFollowTarget.OnProviderDestroy -= OnFollowTargetDestroy;
            currentFollowTarget = null;
        }

        public override void OnMapEntered()
        {
            stateController.SetState(AIState.Wander);
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