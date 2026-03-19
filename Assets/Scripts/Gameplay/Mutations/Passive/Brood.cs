using System.Collections.Generic;
using Gameplay.Effects;
using Gameplay.Effects.BroodSpider;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Mutations.EntityEffects.BroodInfection;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
{
    public class Brood : BasicAbility
    {
        [Header("Effect Duration")]
        [SerializeField, Range(1, 10)] private int effectDurationLvl1;
        [SerializeField, Range(1, 10)] private int effectDurationLvl10;
        [Header("Brood lifetime")]
        [SerializeField, Range(1, 60f)] private float spiderLifetimeLvl1;
        [SerializeField, Range(1, 60f)] private float spiderLifetimeLvl10;
        [Header("Brood movespeed")]
        [SerializeField, Range(0.01f, 10f)] private float spiderSpeedLvl1;
        [SerializeField, Range(0.01f, 10f)] private float spiderSpeedLvl10;
        [Header("Brood attack damage")]
        [SerializeField, Range(0.01f, 10f)] private float spiderDamageLvl1;
        [SerializeField, Range(0.01f, 10f)] private float spiderDamageLvl10;
        [Header("Brood spawn chance")]
        [SerializeField, Range(0, 1f)] private float baseSpawnChanceLvl1;
        [SerializeField, Range(0, 1f)] private float baseSpawnChanceLvl10;
        [Header("Brood spawn amount")]
        [SerializeField, Range(1, 5)] private int baseSpiderAmountLvl1;
        [SerializeField, Range(1, 5)] private int baseSpiderAmountLvl10;
        
        private int effectDuration;
        private float spiderLifetime;
        private float spiderSpeed;
        private float spiderDamage;
        private float baseSpawnChance;
        private float baseSpiderAmount;
        
        private AttackEffect attackEffect;
        private BroodInfectionEffectData effectData;

        public const int MAX_SPIDERS_AMOUNT = 20;
        public const float UNIT_AFFECTION_CHANCE = 0.25f;
        
        protected override void Start()
        {
            attackEffect = new AttackEffect(new Gradient().FastGradient(Color.wheat, Color.whiteSmoke), OnImpact);
            base.Start();
        }

        private void OnImpact(IImpactable impactable, float damage)
        {
            if (damage > 0 && impactable is IEffectAffectable affectable)
                affectable.AddEffect<BroodInfectionEffect>(effectData);
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            effectDuration = LerpLevel(effectDurationLvl1, effectDurationLvl10, lvl);
            spiderLifetime = LerpLevel(spiderLifetimeLvl1, spiderLifetimeLvl10, lvl);
            spiderSpeed = LerpLevel(spiderSpeedLvl1, spiderSpeedLvl10, lvl);
            spiderDamage = LerpLevel(spiderDamageLvl1, spiderDamageLvl10, lvl);
            baseSpawnChance = LerpLevel(baseSpawnChanceLvl1, baseSpawnChanceLvl10, lvl);
            baseSpiderAmount = LerpLevel((float) baseSpiderAmountLvl1, (float) baseSpiderAmountLvl10, lvl);

            effectData = new BroodInfectionEffectData(effectDuration, baseSpawnChance, baseSpiderAmount,
                new BroodSpiderArguments(
                    spiderLifetime,
                    spiderSpeed,
                    spiderDamage
                ));
        }
        
        private void OnAttackEffectCollectionRequested(List<AttackEffect> effects) 
            => effects.Add(attackEffect);
        
        protected override void OnEnable()
        {
            base.OnEnable();
            IDamageable.OnDamageTakenGlobal += OnEnemyDamageTakenGlobal;
            BasePlayerAttack.OnAttackEffectCollectionRequested += OnAttackEffectCollectionRequested;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BasePlayerAttack.OnAttackEffectCollectionRequested -= OnAttackEffectCollectionRequested;
        }

        private void OnEnemyDamageTakenGlobal(IDamageable damageable, DamageInstance instance)
        {
            if (instance.source.owner is IFriendlyUnit && 
                damageable is IDamageableEnemy and IEffectAffectable affectable &&
                Random.value <= UNIT_AFFECTION_CHANCE)
                affectable.AddEffect<BroodInfectionEffect>(effectData);
        }
    }
}