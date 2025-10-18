using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using Pooling;
using UnityEngine;
using Util.Interfaces;
using Util.Particles;
using Random = UnityEngine.Random;

namespace Gameplay.Effects.ChainLightning
{
    public class ChainLightning : Poolable
    {
        [SerializeField] private new ParticleSystemLineRenderer particleSystem;
        [SerializeField] private AudioSource audioSource;
        
        private ChainLightningArguments currentArgs;
        
        
        
        public override bool OnTakenFromPool(object data)
        {
            if (data is not ChainLightningArguments args) return false;
            // transform.position = Vector3.zero;
            currentArgs = args;
            Activate();
            return base.OnTakenFromPool(data);
        }

        private void Activate()
        {
            DamageTarget();
            CancellationToken cancellationToken = gameObject.GetCancellationTokenOnDestroy();

            if (currentArgs.currentJump < currentArgs.maxNumberOfJumps && TryCreateArc(out IDamageable target))
                PropagateTask(target, cancellationToken).Forget();

            PoolTask(cancellationToken).Forget();
        }

        private bool TryCreateArc(out IDamageable target)
        {
            target = null;
            var cols = Physics2D.OverlapCircleAll(currentArgs.position, currentArgs.chainRange, layerMask: GlobalDefinitions.EnemyPhysicsLayerMask);
            
            foreach (Collider2D col in cols.OrderBy((_) => Random.value))
            {
                if (!col.TryGetComponent(out IDamageableEnemy damageableEnemy) ||
                    damageableEnemy.Equals(currentArgs.currentTarget) ||
                    damageableEnemy.Immune) continue;

                target = damageableEnemy;
                break;
            }
            
            if (target is null)
            {
                particleSystem.PlayOnAwake = false;
                audioSource.playOnAwake = false;
                return false;
            }

            particleSystem.PlayOnAwake = true;
            audioSource.playOnAwake = true;
            audioSource.pitch = Random.Range(1, 1.2f);
            
            Vector3 fromPos = transform.TransformPoint(currentArgs.position);
            Vector3 toPos = transform.TransformPoint(target.Transform.position);
            float distance = Vector2.Distance(fromPos, toPos);
            
            int steps = Mathf.RoundToInt(distance / 0.25f);
            Vector3[] positions = new Vector3[steps + 2];
            positions[0] = fromPos;
            positions[steps + 1] = toPos;
            Vector3 current = fromPos;
            for (int i = 1; i <= steps; i++)
            {
                current = Vector3.MoveTowards(current, toPos, 0.25f);
                positions[i] = current;
            }

            particleSystem.Play(positions);
            return true;
        }

        private void DamageTarget()
        {
            currentArgs.currentTarget.Damage(
                currentArgs.damage, transform.position, 0f, 1f, default);
        }

        private async UniTask PoolTask(CancellationToken cancellationToken)
        {
            await UniTask.WaitUntil(() => !particleSystem.IsPlaying, cancellationToken: cancellationToken);
            Pool();
        }

        private async UniTask PropagateTask(IDamageable damageable, CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: cancellationToken);
            PropagateChain(damageable);
        }

        private void PropagateChain(IDamageable damageable)
        {
            var targetPos = damageable.Transform.position;
            PoolManager.GetEffect<ChainLightning>(new ChainLightningArguments(
                currentArgs.damage * 0.75f, 
                currentArgs.chainRange, 
                currentArgs.maxNumberOfJumps, 
                damageable, 
                currentArgs.currentJump + 1,
                targetPos));
        }
    }
}