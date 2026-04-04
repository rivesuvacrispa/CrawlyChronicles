using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using DG.Tweening;
using Gameplay.Mutations;
using Gameplay.Mutations.Passive;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util;
using Util.Components;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Effects.BroodSpider
{
    public class BroodSpider : FriendlyUnit<BroodSpiderArguments>
    {
        [SerializeField] private BodyPainter bodyPainter;
        [SerializeField] private BroodSpiderAnimator animator;
        private float maxDistanceFromPlayer = 4;


        private static readonly List<Collider2D> Results = new(16);
        protected override List<Collider2D> OverlapResults => Results;
        protected override ContactFilter2D TargetSearchContactFilter => GlobalDefinitions.EnemyPhysicsContactFilter;
        protected override int ResultsCapacity => 16;
        protected override float TargetSearchRadius => 3f;

        private static int currentSpidersAmount = 0;
        public static bool CanSpawnSpider => currentSpidersAmount < BasicAbility.CalculateSummonsAmount(Brood.MAX_SPIDERS_AMOUNT);


        
        public override bool OnTakenFromPool(object data)
        {
            if (!base.OnTakenFromPool(data)) return false;
            currentSpidersAmount++;
            LifetimeTask(gameObject.CreateCommonCancellationToken()).Forget();
            return true;
        }

        public override void OnPool()
        {
            currentSpidersAmount--;
            if (currentSpidersAmount < 0) currentSpidersAmount = 0;
            base.OnPool();
        }

        private async UniTask LifetimeTask(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(currentArgs.lifetime), cancellationToken: cancellationToken);

            DisposeCts();

            await UniTask.NextFrame(cancellationToken: cancellationToken);
            
            animator.StopAttack();
            animator.ResetRotation();
            animator.PlayDead();
            
            bodyPainter.Paint(GlobalDefinitions.DeathGradient, 1f);
            await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: cancellationToken);
            bodyPainter.FadeOut(1f);
            PoolTask(1.1f, cancellationToken).Forget();
        }

        protected override async UniTask MovementTask(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Vector3 destination = PickDestination();
                animator.RotateTowards(destination);
                if (currentTarget is IDamageableEnemy enemy)
                {
                    await Attack(destination, enemy, cancellationToken);
                }
                else
                {
                    animator.PlayWalk();
                    await transform.DOMove(destination, currentArgs.speed)
                        .SetSpeedBased()
                        .SetEase(Ease.OutCubic)
                        .AsyncWaitForCompletion()
                        .AsUniTask()
                        .AttachExternalCancellation(cancellationToken: cancellationToken);
                }

                animator.PlayIdle();
                await UniTask.Delay(TimeSpan.FromSeconds(.45f + Random.value * 0.1f), cancellationToken: cancellationToken);
            }
        }

        private async UniTask Attack(Vector3 destination, IDamageableEnemy enemy, CancellationToken cancellationToken)
        {
            animator.PlayAttack();
            
            bool attacked = false;
            do
            {
                Vector3 pos = transform.position;
                if (!attacked && (pos - enemy.Transform.position).sqrMagnitude <= 1/8f)
                {
                    enemy.Damage(
                        new DamageSource(this),
                        BasicAbility.CalculateSummonDamage(currentArgs.damage), 
                        pos, 
                        piercing: true
                    );
                    attacked = true;
                }

                transform.position =
                    Vector3.MoveTowards(pos, destination, currentArgs.speed * 2f * Time.fixedDeltaTime);
                await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate, cancellationToken);

            } while ((transform.position - destination).sqrMagnitude >= 0.04f);
            
            animator.StopAttack();
        }

        private Vector3 PickDestination()
        {
            Vector3 currentPos = transform.position;
            Vector3 playerPos = PlayerPhysicsBody.Position;
            Vector3 wanderPos = Random.insideUnitCircle.normalized;

            if (PlayerManager.Instance.Equals(currentTarget))
            {
                if (Vector2.Distance(playerPos, currentPos) >= maxDistanceFromPlayer)
                    return currentPos + (playerPos - currentPos).normalized * 2f;

                return currentPos + wanderPos;
            }

            if (currentTarget is not null)
            {
                return currentPos + (currentTarget.Transform.position - currentPos).normalized * 2 + wanderPos * 0.05f;
            }

            return currentPos;
        }
    }
}