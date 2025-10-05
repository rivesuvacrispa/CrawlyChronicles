using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.AI;
using Gameplay.Breeding;
using Gameplay.Food;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Enemies.Enemies
{
    public class Scorpion : Enemy
    {
        [SerializeField] private ScorpionSting sting;
        [SerializeField] private ScorpionTail tail;
        
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
            
        }
        
        protected override async UniTask PerformAttack(CancellationToken cancellationToken)
        {
            await sting.AttackTask(cancellationToken);
        }

        protected override void AttackPlayer(float reachDistance = 0)
        {
            base.AttackPlayer(reachDistance);
            tail.TrackPlayer = true;
            sting.TrackPlayer = true;
        }

        public override void Die()
        {
            base.Die();
            tail.gameObject.SetActive(false);
        }
    }
}