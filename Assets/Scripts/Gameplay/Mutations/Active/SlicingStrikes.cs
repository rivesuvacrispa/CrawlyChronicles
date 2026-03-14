using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using Gameplay.Effects.PhantomPlayerAttack;
using Gameplay.Player;
using Hitboxes;
using Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Mutations.Active
{
    public class SlicingStrikes : ActiveAbility
    {
        [Header("Cleave Radius")] 
        [SerializeField, Range(0, 5f)] private float radiusLvl1;
        [SerializeField, Range(0, 5f)] private float radiusLvl10;
        [Header("Damage")] 
        [SerializeField, Range(0, 5f)] private float bonusDamageLvl1;
        [SerializeField, Range(0, 5f)] private float bonusDamageLvl10;
        [Header("Damage")] 
        [SerializeField, Range(0, 10f)] private int attacksAmountLvl1;
        [SerializeField, Range(0, 10f)] private int attacksAmountLvl10;
        
        private float radius;
        private int attacksAmount;
        private float bonusDamage;
        private static readonly List<Collider2D> OverlapResults = new(32);



        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            radius = LerpLevel(radiusLvl1, radiusLvl10, lvl);
            bonusDamage = LerpLevel(bonusDamageLvl1, bonusDamageLvl10, lvl);
            attacksAmount = LerpLevel(attacksAmountLvl1, attacksAmountLvl10, lvl);
        }

        public override void Activate(bool auto = false)
        {
            if (CollectTargets() > 0)
            {
                base.Activate(auto);
                ActivateTask(CreateCommonCancellationToken()).Forget();
            }
            else
            {
                SetOnCooldown(1f);
            }
        }

        private int CollectTargets()
        {
            OverlapResults.Clear();
            int contacts = Physics2D.OverlapCircle(
                transform.position,
                radius, GlobalDefinitions.EnemyPhysicsContactFilter, OverlapResults);
            return contacts;
        }

        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            for (int i = 0; i < attacksAmount; i++)
            {
                int contacts = CollectTargets();

                if (contacts == 0) return;
                var c = OverlapResults[Random.Range(0, contacts)];

                if (c.TryGetComponent(out IDamageableEnemy e))
                {
                    Vector3 targetPos = e.Transform.position;
                    Vector3 spawnPos = targetPos + (Vector3)Random.insideUnitCircle.normalized * 1.25f;
                    PoolManager.GetEffect<PhantomPlayerAttack>(
                        new PhantomPlayerAttackArguments(spawnPos, targetPos, bonusDamage)
                    );
                }

                if (attacksAmount > 1) 
                    await UniTask.Delay(TimeSpan.FromSeconds(1f / attacksAmount), cancellationToken: cancellationToken);
            }
        }

        protected override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            return null;
        }
    }
}