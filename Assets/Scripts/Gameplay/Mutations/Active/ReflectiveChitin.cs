using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using Hitboxes;
using UI.Menus;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.Active
{
    public class ReflectiveChitin : ActiveAbility, IDamageSource
    {
        [SerializeField] private new ParticleSystem particleSystem;
        
        [Header("Passive Reflection")]
        [SerializeField, Range(0f, 2f)] private float reflectionLvl1;
        [SerializeField, Range(0f, 2f)] private float reflectionLvl10;
        [Header("Active Reflection Multiplier")] 
        [SerializeField, Range(0f, 2f)] private float bonusReflectionLvl1;
        [SerializeField, Range(0f, 2f)] private float bonusReflectionLvl10;
        [Header("Active Effect Duration")] 
        [SerializeField, Range(1f, 10f)] private float activeEffectDurationLvl1;
        [SerializeField, Range(1f, 10f)] private float activeEffectDurationLvl10;

        private float reflection;
        private float bonusReflection = 1f;
        private float activeEffectDuration;
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
                    instance.Damage * ((1 + bonusReflection) * reflection), piercing: true)
                );
            }
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            reflection = LerpLevel(reflectionLvl1, reflectionLvl10, lvl);
            activeEffectDuration = LerpLevel(activeEffectDurationLvl1, activeEffectDurationLvl10, lvl);
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(false);
            ActivateTask(
                    CancellationTokenSource.CreateLinkedTokenSource(
                        gameObject.GetCancellationTokenOnDestroy(),
                        MainMenu.CancellationTokenOnReset).Token
                    ).Forget();
        }

        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            main.loop = true;
            particleSystem.Play();
            bonusReflection = LerpLevel(bonusReflectionLvl1, bonusReflectionLvl10, Level);
            
            await UniTask.Delay(TimeSpan.FromSeconds(activeEffectDuration), cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            
            bonusReflection = 0f;
            particleSystem.Stop();
            main.loop = false;
        }

        protected override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            return null;
        }
    }
}