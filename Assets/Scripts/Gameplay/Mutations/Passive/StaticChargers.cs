using System.Collections.Generic;
using Gameplay.Effects.ChainLightning;
using Gameplay.Mutations.AttackEffects;
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
        [Header("Stun Duration")]
        [SerializeField, Range(0, 2)] private float stunDurationLvl1;
        [SerializeField, Range(0, 2)] private float stunDurationLvl10;
        [Header("Damage Reduction Over Jump")]
        [SerializeField, Range(0, 1)] private float dmgReductionLvl1;
        [SerializeField, Range(0, 1)] private float dmgReductionLvl10;
        
        
        private AttackEffect attackEffect;
        private float damage;
        private float chainRange;
        private int maxNumberOfJumps;
        private float stunDuration;
        private float dmgReduction;


        
        protected override void Start()
        {
            attackEffect = new AttackEffect(effectGradient, OnImpact);
            base.Start();
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            chainRange = LerpLevel(chainRangeLvl1, chainRangeLvl10, lvl);
            maxNumberOfJumps = Mathf.RoundToInt(LerpLevel(maxNumberOfJumpsLvl1, maxNumberOfJumpsLvl10, lvl));
            stunDuration = LerpLevel(stunDurationLvl1, stunDurationLvl10, lvl);
            dmgReduction = LerpLevel(dmgReductionLvl1, dmgReductionLvl10, lvl);
        }

        private void OnImpact(IImpactable impactable, float _)
        {
            if (impactable is not IDamageable damageable) return;

            var targetPos = damageable.Transform.position;
            PoolManager.GetEffect<ChainLightning>(new ChainLightningArguments(
                damage, chainRange, maxNumberOfJumps, damageable, 0, targetPos, stunDuration, dmgReduction));
        }

        private void OnAttackEffectCollectionRequested(List<AttackEffect> effects)
        {
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