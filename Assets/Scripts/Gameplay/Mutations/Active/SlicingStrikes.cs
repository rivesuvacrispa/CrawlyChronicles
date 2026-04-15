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
using Util.Abilities;
using Util.Attributes;
using Random = UnityEngine.Random;

namespace Gameplay.Mutations.Active
{
    public class SlicingStrikes : ActiveAbility
    {
        [SerializeField] private LevelConst attackInterval = new LevelConst(0.25f);
        [SerializeField, MinMaxRange(0f, 5f)] private LevelFloat radius = new LevelFloat(1.25f, 3f);
        [SerializeField, MinMaxRange(0f, 5f)] private LevelFloat bonusDamage = new LevelFloat(0.25f, 4f);
        [SerializeField, MinMaxRange(0f, 10f)] private LevelInt attacksAmount = new LevelInt(3, 10);

        private float currentRadius;
        private int currentAttacksAmount;
        private float currentBonusDamage;
        private static readonly List<Collider2D> OverlapResults = new(32);



        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentRadius = radius.AtLvl(lvl);
            currentBonusDamage = bonusDamage.AtLvl(lvl);
            currentAttacksAmount = attacksAmount.AtLvl(lvl);
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
                currentRadius, GlobalDefinitions.EnemyPhysicsContactFilter, OverlapResults);
            return contacts;
        }

        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            var interval = TimeSpan.FromSeconds(attackInterval.Value);
            
            for (int i = 0; i < currentAttacksAmount; i++)
            {
                int contacts = CollectTargets();

                if (contacts == 0) return;
                var c = OverlapResults[Random.Range(0, contacts)];

                if (c.TryGetComponent(out IDamageableEnemy e))
                {
                    Vector3 targetPos = e.Transform.position;
                    Vector3 spawnPos = targetPos + (Vector3)Random.insideUnitCircle.normalized * 1.25f;
                    PoolManager.GetEffect<PhantomPlayerAttack>(
                        new PhantomPlayerAttackArguments(targetPos, currentBonusDamage),
                        spawnPos
                    );
                }

                if (currentAttacksAmount > 1) 
                    await UniTask.Delay(interval, cancellationToken: cancellationToken);
            }
        }

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                attackInterval.UseKey(LevelFieldKeys.ATTACKS_INTERVAL).UseFormatter(StatFormatter.SECONDS),
                bonusDamage.UseKey(LevelFieldKeys.BONUS_DAMAGE),
                attacksAmount.UseKey(LevelFieldKeys.ATTACKS_AMOUNT)
            };
        }
    }
}