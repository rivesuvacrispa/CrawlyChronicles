using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Player;
using SoundEffects;
using UnityEngine;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Mutations.Passive
{
    public class DeadlyStrike : BasicAbility
    {
        [SerializeField] private Gradient effectGradient;
        [SerializeField] private ParticleSystem strikeParticles;
        [SerializeField] private ParticleSystem bloodParticles;
        [SerializeField] private SimpleAudioSource attackSource;
        [SerializeField] private SimpleAudioSource hitSource;
        [Header("Proc Chance")] 
        [SerializeField, Range(0.01f, 1)] private float procChanceLvl1;
        [SerializeField, Range(0.01f, 1)] private float procChanceLvl10;
        [Header("Damage Multiplier")] 
        [SerializeField, Range(1f, 100f)] private float damageMultiplierLvl1;
        [SerializeField, Range(1f, 100f)] private float damageMultiplierLvl10;

        private float procChance;
        private float damageMultiplier;
        private AttackEffect attackEffect;

        private PlayerStats addedStats;
        private bool statsActive;
        
        
        protected override void Start()
        {
            attackEffect = new AttackEffect(effectGradient, OnImpact);
            base.Start();
        }
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);

            procChance = LerpLevel(procChanceLvl1, procChanceLvl10, lvl);
            damageMultiplier = LerpLevel(damageMultiplierLvl1, damageMultiplierLvl10, lvl);
        }
        
        private void OnAttackEffectCollectionRequested(List<AttackEffect> effects)
        {
            if (Random.value <= procChance)
            {
                attackSource.Play();
                effects.Add(attackEffect);
                AddStats();
            }
        }

        private void OnImpact(IImpactable impactable, float _)
        {
            PlayAnimation(impactable);
            RemoveStats();
        }

        private void PlayAnimation(IImpactable impactable)
        {
            if (!statsActive) return;
            
            strikeParticles.Play();
            bloodParticles.transform.position = impactable.Transform.position;
            bloodParticles.Play();
            TimeScaleTask(0.25f, gameObject.GetCancellationTokenOnDestroy()).Forget();
            hitSource.Play();
        }

        private async UniTask TimeScaleTask(float duration, CancellationToken cancellationToken)
        {
            Time.timeScale = 0.5f;
            await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: true, cancellationToken: cancellationToken);
            Time.timeScale = 1f;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerAttack.OnAttackEffectCollectionRequested += OnAttackEffectCollectionRequested;
            PlayerAttack.OnAttackEnd += OnPlayerAttackEnd;
            PlayerManager.Instance.OnDeath += OnPlayerDeath;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerAttack.OnAttackEffectCollectionRequested -= OnAttackEffectCollectionRequested;
            PlayerAttack.OnAttackEnd -= OnPlayerAttackEnd;
            PlayerManager.Instance.OnDeath -= OnPlayerDeath;
        }

        private void OnPlayerAttackEnd()
        {
            RemoveStats();
        }
        
        private void OnPlayerDeath(IDamageable damageable)
        {
            RemoveStats();
        }

        private void AddStats()
        {
            if (statsActive) return;
            
            float damageAdded = PlayerManager.PlayerStats.AttackDamage * (damageMultiplier - 1);
            addedStats = new PlayerStats(attackDamage: damageAdded);
            PlayerManager.Instance.AddStats(addedStats);
            statsActive = true;
        }

        private void RemoveStats()
        {
            if (!statsActive) return;

            PlayerManager.Instance.AddStats(addedStats.Negated());
            statsActive = false;
        }
    }
}