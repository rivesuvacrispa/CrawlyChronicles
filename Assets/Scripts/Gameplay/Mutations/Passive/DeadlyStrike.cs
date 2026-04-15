using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Player;
using SoundEffects;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
{
    public class DeadlyStrike : BasicAbility
    {
        [SerializeField] private Gradient effectGradient;
        [SerializeField] private ParticleSystem strikeParticles;
        [SerializeField] private ParticleSystem bloodParticles;
        [SerializeField] private SimpleAudioSource attackSource;
        [SerializeField] private SimpleAudioSource hitSource;
        [SerializeField, MinMaxRange(0.01f, 1f)] private LevelFloat procChance = new LevelFloat(0.05f, 0.125f);
        [SerializeField, MinMaxRange(1f, 100f)] private LevelFloat damageMultiplier = new LevelFloat(5f, 15f);

        private float currentProcChance;
        private float currentDamageMultiplier;
        private CancellationTokenSource cancellationTokenSource;


        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                procChance.UseKey(LevelFieldKeys.PROC_CHANCE).UseFormatter(StatFormatter.PERCENT),
                damageMultiplier.UseKey(LevelFieldKeys.BONUS_DAMAGE).UseFormatter(StatFormatter.PERCENT)
            };
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);

            currentProcChance = procChance.AtLvl(lvl);
            currentDamageMultiplier = damageMultiplier.AtLvl(lvl);
        }
        
        private void OnAttackEffectCollectionRequested(List<AttackEffect> effects)
        {
            if (TryProc(currentProcChance))
            {
                attackSource.Play();
                effects.Add(new DeadlyStrikeAttackEffect(effectGradient, OnImpact,
                    PlayerManager.PlayerStats.AttackDamage * (currentDamageMultiplier - 1)));
            }
        }

        private void OnImpact(IImpactable impactable, float _)
        {
            PlayAnimation(impactable);
        }

        private void PlayAnimation(IImpactable impactable)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();

            var position = impactable.Transform.position;
            strikeParticles.transform.position = position;
            strikeParticles.Play();
            bloodParticles.transform.position = position;
            bloodParticles.Play();
            TimeScaleTask(0.25f, CreateCommonCancellationToken(cancellationTokenSource.Token))
                .Forget();
            hitSource.Play();
        }

        private async UniTask TimeScaleTask(float duration, CancellationToken cancellationToken)
        {
            Time.timeScale = 0.5f;
            await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: true, cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            Time.timeScale = 1f;
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            BasePlayerAttack.OnAttackEffectCollectionRequested += OnAttackEffectCollectionRequested;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            BasePlayerAttack.OnAttackEffectCollectionRequested -= OnAttackEffectCollectionRequested;
        }
    }
}