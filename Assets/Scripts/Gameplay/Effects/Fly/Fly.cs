using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using Gameplay.Mutations;
using Hitboxes;
using UnityEngine;
using Util;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Effects.Fly
{
    public class Fly : FriendlyUnit<FlyArguments>
    {
        private static readonly List<Collider2D> Results = new(16);
        protected override List<Collider2D> OverlapResults => Results;
        protected override ContactFilter2D TargetSearchContactFilter => GlobalDefinitions.EnemyPhysicsContactFilter;
        protected override int ResultsCapacity => 16;
        protected override float TargetSearchRadius => 3f;
        

        
        protected override async UniTask MovementTask(CancellationToken cancellationToken)
        {
            Transform t = transform;
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);

                if (currentTarget is null) continue;

                Vector2 currentPos = t.position;
                Vector2 targetPos = currentTarget.Transform.position;
                rb.AddForce(t.up * currentArgs.flySpeed);
                rb.RotateTowardsPosition(targetPos + Random.insideUnitCircle * 0.25f, currentArgs.rotationSpeed);


                float sqrDist = Vector2.SqrMagnitude(currentPos - targetPos);
                if (canAttack && currentTarget is IDamageableEnemy enemy && sqrDist < 0.25f)
                {
                    enemy.Damage(
                        new DamageSource(this), 
                        BasicAbility.CalculateSummonDamage(currentArgs.damage), 
                        transform.position
                    );
                    AttackDelayTask(cancellationToken: cancellationToken).Forget();
                };
            }
        }
        
        private async UniTask AttackDelayTask(CancellationToken cancellationToken)
        {
            canAttack = false;
            await UniTask.Delay(TimeSpan.FromSeconds(currentArgs.attackCooldown), cancellationToken: cancellationToken);
            canAttack = true;
        }
    }
}