using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using Gameplay.AI;
using Gameplay.Enemies;
using Gameplay.Enemies.Enemies;
using Gameplay.Player;
using Pooling;
using UnityEngine;
using Util;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Effects.Fly
{
    public class Fly : Poolable
    {
        [SerializeField] private Rigidbody2D rb;
        
        private ITransformProvider followTarget;
        private static readonly List<Collider2D> OverlapResults = new(8);

        private CancellationTokenSource cts;
        private bool canAttack;
        private FlyArguments args;
        
        private void FindTarget()
        {
            OverlapResults.Clear();
            int contacts = Physics2D.OverlapCircle(transform.position, 3f,  GlobalDefinitions.EnemyPhysicsContactFilter, OverlapResults);

            IDamageableEnemy fallbackEnemy = null;
            for (var i = 0; i < Mathf.Min(contacts, 8); i++)
            {
                var t = OverlapResults[i];
                if (t.TryGetComponent(out IDamageableEnemy enemy) && !enemy.Immune && enemy is not NeutralAnt)
                {
                    fallbackEnemy ??= enemy;
                    if (Random.value <= 0.25f)
                    {
                        SetTarget(enemy);
                    }
                }
            }

            if (fallbackEnemy is not null)
            {
                SetTarget(fallbackEnemy);
            }
        }

        private void SetTarget(ITransformProvider target)
        {
            if (target is null) return;
            
            followTarget = target;
            target.OnProviderDestroy += OnTargetDestroy;
            if (target is Enemy enemy) 
                enemy.StateController.OnBecomeEtherial += OnTargetBecomeEtherial;
            if (target is IDamageable damageable)
                damageable.OnDeath += OnTargetDeath;
        }

        private void DropTarget(bool toPlayer)
        {
            if (followTarget is not null)
            {
                if (followTarget is IDestructionEventProvider eventProvider)
                    eventProvider.OnProviderDestroy -= OnTargetDestroy;

                if (followTarget is Enemy enemy)
                    enemy.StateController.OnBecomeEtherial -= OnTargetBecomeEtherial;

                if (followTarget is IDamageable damageable)
                    damageable.OnDeath -= OnTargetDeath;
            }

            if (toPlayer)
                SetTarget(PlayerManager.Instance);
        }   

        private void OnTargetDestroy(IDestructionEventProvider eventProvider)
        {
            DropTarget(true);
        }

        private void OnTargetBecomeEtherial(AIStateController stateController)
        {
            if (stateController.Etherial) DropTarget(true);
        }

        private void OnTargetDeath(IDamageable target)
        {
            DropTarget(true);
        }

        public override bool OnTakenFromPool(object data)
        {
            if (data is not FlyArguments arguments) return false;
            args = arguments;
            
            DropTarget(true);
            DisposeCts();
            cts = new CancellationTokenSource();

            canAttack = true;
            FlyTask(cts.Token).Forget();
            TargetChangeTask(cts.Token).Forget();
            return base.OnTakenFromPool(data);
        }

        private void DisposeCts()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

        private void OnDestroy()
        {
            DisposeCts();
            DropTarget(false);
        }

        public override void OnPool()
        {
            DropTarget(false);
            DisposeCts();
            base.OnPool();
        }

        private async UniTask TargetChangeTask(CancellationToken cancellationToken)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(0.5f);
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(timeSpan, cancellationToken: cancellationToken);
                if (PlayerManager.Instance.Equals(followTarget))
                {
                    FindTarget();
                }
            }
        }

        private async UniTask FlyTask(CancellationToken cancellationToken)
        {
            Transform t = transform;
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);

                if (followTarget is null) continue;

                Vector2 currentPos = t.position;
                Vector2 targetPos = followTarget.Transform.position;
                rb.AddForce(t.up * args.flySpeed);
                rb.RotateTowardsPosition(targetPos + Random.insideUnitCircle * 0.25f, args.rotationSpeed);


                float sqrDist = Vector2.SqrMagnitude(currentPos - targetPos);
                if (canAttack && followTarget is IDamageableEnemy enemy && sqrDist < 0.25f)
                {
                    enemy.Damage(args.damage, transform.position, 0f, 0f, default);
                    AttackDelayTask(cancellationToken: cancellationToken).Forget();
                };
            }
        }

        private async UniTask AttackDelayTask(CancellationToken cancellationToken)
        {
            canAttack = false;
            await UniTask.Delay(TimeSpan.FromSeconds(args.attackCooldown), cancellationToken: cancellationToken);
            canAttack = true;
        }
    }
}