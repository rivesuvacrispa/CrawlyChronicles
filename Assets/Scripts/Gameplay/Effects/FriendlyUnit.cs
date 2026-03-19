using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using Hitboxes;
using Pooling;
using UnityEngine;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Effects
{
    public abstract class FriendlyUnit<T> : Poolable, IDamageSource, IFriendlyUnit
    {
        [SerializeField] protected Rigidbody2D rb;
        
        protected IUnitTarget currentTarget;
        protected CancellationTokenSource cts;
        protected bool canAttack;
        protected T currentArgs;

        protected abstract List<Collider2D> OverlapResults { get; }
        protected abstract ContactFilter2D TargetSearchContactFilter { get; }
        protected abstract int ResultsCapacity { get; }
        protected abstract float TargetSearchRadius { get; }
        protected abstract UniTask MovementTask(CancellationToken cancellationToken);
        
        
        public override bool OnTakenFromPool(object data)
        {
            if (data is not T args) return false;
            currentArgs = args;
            
            DropTarget(true);
            DisposeCts();
            cts = new CancellationTokenSource();

            canAttack = true;
            MovementTask(cts.Token).Forget();
            TargetChangeTask(cts.Token).Forget();
            return base.OnTakenFromPool(data);
        }
        
        private async UniTask TargetChangeTask(CancellationToken cancellationToken)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(0.5f);
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(timeSpan, cancellationToken: cancellationToken);
                if (PlayerManager.Instance.Equals(currentTarget))
                {
                    FindTarget();
                }
            }
        }

        private void FindTarget()
        {
            OverlapResults.Clear();
            int contacts = Physics2D.OverlapCircle(
                transform.position,
                TargetSearchRadius,
                TargetSearchContactFilter,
                OverlapResults
            );

            IUnitTarget target = null;
            DamageSource source = new DamageSource(this);

            int resLen = Mathf.Min(contacts, ResultsCapacity);
            for (var i = 0; i < resLen; i++)
            {
                var col = OverlapResults[i];
                if (!col.TryGetComponent(out IUnitTarget newTarget))
                    continue;

                bool immune = newTarget is IDamageable d && d.Hitbox.ImmuneToSource(source);
                if (immune || !newTarget.CanAggroUnit)
                    continue;
                
                if (Random.value <= 0.25f)
                {
                    SetTarget(newTarget);
                    return;
                }
                
                target ??= newTarget;
            }

            if (target is not null)
            {
                SetTarget(target);
            }
        }

        private void SetTarget(IUnitTarget target)
        {
            if (target is null) return;
            
            currentTarget = target;
            target.OnUnitDetach += OnTargetUnitDetached;
        }

        private void DropTarget(bool toPlayer)
        {
            if (currentTarget is not null) 
                currentTarget.OnUnitDetach -= OnTargetUnitDetached;

            if (toPlayer)
                SetTarget(PlayerManager.Instance);
        }

        private void OnTargetUnitDetached()
        {
            DropTarget(true);
        }

        protected void DisposeCts()
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
    }
}