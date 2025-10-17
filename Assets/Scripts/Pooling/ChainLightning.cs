using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using UnityEngine;
using Util.Interfaces;
using Util.Particles;
using Random = UnityEngine.Random;

namespace Pooling
{
    public class ChainLightning : Poolable
    {
        [SerializeField] private new ParticleSystemLineRenderer particleSystem;

        private ChainLightningArguments currentArgs;
        private static readonly Collider[] OverlapResults = new Collider[8];
        private static readonly IDamageableEnemy[] DamageableEnemies = new IDamageableEnemy[8];
        
        
        
        public override bool OnTakenFromPool(object data)
        {
            if (data is not ChainLightningArguments args) return false;
            currentArgs = args;
            Activate();
            return base.OnTakenFromPool(data);
        }

        private void Activate()
        {
            DamageTarget();
            IDamageable target = CreateArc();

            CancellationToken cancellationToken = gameObject.GetCancellationTokenOnDestroy();
            
            if (target is not null && currentArgs.currentJump < currentArgs.maxNumberOfJumps)
                PropagateTask(target, cancellationToken).Forget();

            PoolTask(cancellationToken).Forget();
        }

        private IDamageable CreateArc()
        {
            var size = Physics.OverlapSphereNonAlloc(
                transform.position, 
                currentArgs.chainRange, 
                OverlapResults, 
                layerMask: GlobalDefinitions.EnemyPhysicsLayerMask);

            int damageables = 0;
            for (var i = 0; i < size; i++)
            {
                if (!OverlapResults[i].TryGetComponent(out IDamageableEnemy damageableEnemy) ||
                    damageableEnemy.Equals(currentArgs.currentTarget)) continue;
                
                DamageableEnemies[damageables] = damageableEnemy;
                damageables++;
            }

            if (damageables == 0) return null;

            IDamageable target = DamageableEnemies[Random.Range(0, damageables)];

            Vector3 fromPos = transform.position;
            Vector3 toPos = target.Transform.position;
            float distance = Vector2.Distance(fromPos, toPos);
            int steps = Mathf.RoundToInt(distance / 0.5f);
            Vector3[] positions = new Vector3[steps + 2];
            positions[0] = fromPos;
            positions[steps + 1] = toPos;
            for (int i = 1; i < steps; i++) 
                positions[i] = Vector3.MoveTowards(fromPos, toPos, i / distance);

            particleSystem.Play(positions);
            return target;
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
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: cancellationToken);
            PropagateChain(damageable);
        }

        private void PropagateChain(IDamageable damageable)
        {
            PoolManager.GetEffect<ChainLightning>(new ChainLightningArguments(
                currentArgs.damage * 0.75f, 
                currentArgs.chainRange, 
                currentArgs.maxNumberOfJumps, 
                damageable, 
                currentArgs.currentJump + 1), damageable.Transform.position);
        }
    }
}