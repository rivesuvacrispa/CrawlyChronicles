using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using Hitboxes;
using Pooling;
using UnityEngine;
using Util.Interfaces;
using Util.Particles;
using Random = UnityEngine.Random;

namespace Gameplay.Effects.ChainLightning
{
    public class ChainLightning : Poolable, IDamageSource
    {
        [SerializeField] private new ParticleSystemLineRenderer particleSystem;
        [SerializeField] private ParticleSystem singleTargetParticles;
        [SerializeField] private AudioSource audioSource;

        private ChainLightningArguments currentArgs;
        private static readonly List<Collider2D> Results = new(16);


        public override bool OnTakenFromPool(object data)
        {
            if (data is not ChainLightningArguments args) return false;
            singleTargetParticles.transform.position = args.position;
            currentArgs = args;
            Activate();
            return base.OnTakenFromPool(data);
        }

        private void Activate()
        {
            Debug.Log($"Lightning instantiated for {gameObject.name}");

            CancellationToken cancellationToken = gameObject.GetCancellationTokenOnDestroy();
            
            if (currentArgs.currentJump < currentArgs.maxNumberOfJumps && 
                TryCreateArc(out IDamageableEnemy target))
            {
                Debug.Log($"Arc created for {gameObject.name}");
                DamageTarget(target);

                if (TryGetTarget(target.Transform.position, currentArgs.chainRange, out IDamageableEnemy nextTarget) &&
                    !nextTarget.Equals(target))
                {
                    PropagateTask(nextTarget, cancellationToken).Forget();
                }
            }

            PoolTask(cancellationToken).Forget();
        }

        public static bool TryGetTarget(
            Vector3 pos,
            float range,
            out IDamageableEnemy target)
        {
            target = null;
            Results.Clear();
            Physics2D.OverlapCircle(pos, range, GlobalDefinitions.EnemyPhysicsContactFilter, Results);

            var targetCol = Results.Where(col => col.TryGetComponent(out IDamageableEnemy _))
                .OrderBy(_ => Random.value)
                .FirstOrDefault();

            if (targetCol is null) return false;

            target = targetCol.GetComponent<IDamageableEnemy>();
            return true;
        }

        private bool TryCreateArc(out IDamageableEnemy target)
        {
            target = null;
            DamageSource source = new DamageSource(this);


            if (!TryGetTarget(
                    currentArgs.position,
                    currentArgs.chainRange,
                    out IDamageableEnemy t
                ) ||
                t.Equals(currentArgs.currentTarget) ||
                t.Hitbox.ImmuneToSource(source))
            {
                particleSystem.PlayOnAwake = false;
                audioSource.playOnAwake = false;
                return false;
            }

            target = t;

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

        private void DamageTarget(IDamageableEnemy enemy)
        {
            enemy.Damage(new DamageInstance(
                new DamageSource(this),
                currentArgs.damage,
                transform.position,
                stunDuration: currentArgs.stunDuration,
                piercing: true));
        }

        private async UniTask PoolTask(CancellationToken cancellationToken)
        {
            await UniTask.WaitUntil(() =>
                    !particleSystem.IsPlaying &&
                    !singleTargetParticles.isPlaying,
                cancellationToken: cancellationToken);

            ((IPoolable)this).Pool();
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
                currentArgs.damage * currentArgs.dmgReduction,
                currentArgs.chainRange,
                currentArgs.maxNumberOfJumps,
                damageable,
                currentArgs.currentJump + 1,
                targetPos,
                currentArgs.stunDuration,
                currentArgs.dmgReduction));
        }
    }
}