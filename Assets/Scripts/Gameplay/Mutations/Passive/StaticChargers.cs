using System.Collections.Generic;
using Gameplay.Effects;
using Gameplay.Effects.ChainLightning;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Mutations.EntityEffects.Poison;
using Gameplay.Player;
using Pooling;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
{
    public class StaticChargers : BasicAbility
    {
        [SerializeField] private Gradient effectGradient;
        [Header("Damage")]
        [SerializeField] private float damageLvl1;
        [SerializeField] private float damageLvl10;        
        [Header("Chain Range")]
        [SerializeField] private float chainRangeLvl1;
        [SerializeField] private float chainRangeLvl10;
        [Header("Duration")] 
        [SerializeField, Range(1, 10)] private int maxNumberOfJumpsLvl1;
        [SerializeField, Range(1, 10)] private int maxNumberOfJumpsLvl10;
        [Header("Duration")] 
        [SerializeField] private int chargeTimeLvl1;
        [SerializeField] private int chargeTimeLvl10;
        
        
        private AttackEffect attackEffect;
        private float damage;
        private float chainRange;
        private int maxNumberOfJumps;
        private float chargeTime;
        private bool onCooldown;


        
        protected override void Start()
        {
            attackEffect = new AttackEffect(effectGradient, OnImpact, true);
            base.Start();
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            chainRange = LerpLevel(chainRangeLvl1, chainRangeLvl10, lvl);
            maxNumberOfJumps = Mathf.RoundToInt(LerpLevel(maxNumberOfJumpsLvl1, maxNumberOfJumpsLvl10, lvl));
            chargeTime = LerpLevel(chargeTimeLvl1, chargeTimeLvl10, lvl);
        }

        private void OnImpact(IImpactable impactable, float _)
        {
            if (impactable is not IDamageable damageable) return;

            var targetPos = damageable.Transform.position;
            PoolManager.GetEffect<ChainLightning>(new ChainLightningArguments(
                damage, chainRange, maxNumberOfJumps, damageable, 0, targetPos));
        }

        private void OnAttackEffectCollectionRequested(List<AttackEffect> effects)
        {
            if (!onCooldown)
                effects.Add(attackEffect);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerAttack.OnAttackEffectCollectionRequested += OnAttackEffectCollectionRequested;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerAttack.OnAttackEffectCollectionRequested -= OnAttackEffectCollectionRequested;
        }
    }
}