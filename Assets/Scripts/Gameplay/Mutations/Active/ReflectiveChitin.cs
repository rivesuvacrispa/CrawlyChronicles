using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;

namespace Gameplay.Mutations.Active
{
    public class ReflectiveChitin : ActiveAbility, IDamageSource
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField, MinMaxRange(0f, 2f)] private LevelFloat reflection = new LevelFloat(0.25f, 1f);
        [SerializeField, MinMaxRange(0f, 2f)] private LevelFloat bonusReflection = new LevelFloat(0.1f, 1f);
        [SerializeField, MinMaxRange(0f, 10f)] private LevelFloat duration = new LevelFloat(2.5f, 5f);

        
        private float currentReflection;
        private float currentBonusReflection = 1f;
        private float currentDuration;
        private ParticleSystem.MainModule main;
        private ParticleSystem.MinMaxCurve initialParticleSize;
        

        protected override void Awake()
        {
            base.Awake();
            main = particleSystem.main;
            initialParticleSize = main.startSize;
        }
        
        private void Update()
        {
            if (!particleSystem.isPlaying) return;
            
            // IDK why but this has to be divided by 57 (* 1 / 57)
            ParticleSystem.MinMaxCurve m =
                new ParticleSystem.MinMaxCurve(PlayerPhysicsBody.Rotation * -0.01754385964f);
            main.startRotation = m;
            main.startSize = new ParticleSystem.MinMaxCurve(
                initialParticleSize.constantMin * PlayerSizeManager.CurrentSize,
                initialParticleSize.constantMax * PlayerSizeManager.CurrentSize
            );
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerManager.Instance.OnDamageTaken += OnPlayerDamageTaken;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerManager.Instance.OnDamageTaken -= OnPlayerDamageTaken;
        }
        
        private void OnPlayerDamageTaken(IDamageable damageable, DamageInstance instance)
        {
            if (instance.source.owner is IDamageableEnemy enemy)
            {
                particleSystem.Play();
                enemy.Damage(new DamageInstance(
                    new DamageSource(this, Time.frameCount), 
                    instance.Damage * ((1 + currentBonusReflection) * currentReflection), piercing: true)
                );
            }
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentReflection = reflection.AtLvl(lvl);
            currentDuration = duration.AtLvl(lvl);
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(false);
            ActivateTask(CreateCommonCancellationToken()).Forget();
        }

        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            main.loop = true;
            particleSystem.Play();
            currentBonusReflection = bonusReflection.AtLvl(level);
            
            await UniTask.Delay(TimeSpan.FromSeconds(currentDuration), cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            
            currentBonusReflection = 0f;
            particleSystem.Stop();
            main.loop = false;
        }

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                reflection.UseKey(LevelFieldKeys.REFLECTION).UseFormatter(StatFormatter.PERCENT),
                bonusReflection.UseKey(LevelFieldKeys.BONUS_REFLECTION).UseFormatter(StatFormatter.PERCENT),
                duration.UseKey(LevelFieldKeys.EFFECT_DURATION).UseFormatter(StatFormatter.SECONDS)
            };
        }
    }
}