using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameCycle;
using Gameplay.Player;
using Hitboxes;
using Scriptable.Enemies;
using UnityEngine;
using Util;
using Util.Abilities;
using Util.Interfaces;
using NeutralAnt = Gameplay.Enemies.Enemies.NeutralAnt;

namespace Gameplay.Mutations.Passive
{
    public class Leafcutter : BasicAbility, IDamageSource
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField] private LevelConst baseDamage = new LevelConst(0.25f);
        [SerializeField] private LevelConst damagePerFood = new LevelConst(0.01f);

        private static readonly TimeSpan ActivationDelay = TimeSpan.FromSeconds(0.15f);


        
        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                baseDamage.UseKey(LevelFieldKeys.BASE_DAMAGE),
                damagePerFood.UseKey(LevelFieldKeys.DAMAGE_PER_PLANTS),
                new LevelConst(GetBonusDamage()).UseKey(LevelFieldKeys.BONUS_DAMAGE)
            };
        }

        protected override bool CacheLevelFields => false;

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            if (damageable is NeutralAnt { Aggressive: false }) 
                return;
            
            damageable.Damage(new DamageSource(this, collisionID),
                baseDamage.Value + GetBonusDamage());
        }

        private float GetBonusDamage() => damagePerFood.Value * StatRecorder.PlantsEaten;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            AttackController.OnAttackStart += OnAttackStart;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            AttackController.OnAttackStart -= OnAttackStart;
        }

        private void OnAttackStart()
        {
           ActivateTask(gameObject.CreateCommonCancellationToken()).Forget();
        }

        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            await UniTask.Delay(ActivationDelay, cancellationToken: cancellationToken);
            particleSystem.Play();
        }
    }
}