using System.Collections.Generic;
using Gameplay.Effects;
using Gameplay.Effects.BroodSpider;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Mutations.EntityEffects.BroodInfection;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
{
    public class Brood : BasicAbility
    {
        [SerializeField] private LevelConst maxSpiders = new LevelConst(20);
        [SerializeField] private LevelConst unitInfectionChance = new LevelConst(0.25f);
        [SerializeField, MinMaxRange(1, 10)] private LevelInt effectDuration = new LevelInt(3, 6);
        [SerializeField, MinMaxRange(1, 60)] private LevelFloat spiderLifetime = new LevelFloat(10, 20);
        [SerializeField, MinMaxRange(0.01f, 10)] private LevelFloat spiderSpeed = new LevelFloat(3, 6);
        [SerializeField, MinMaxRange(0.01f, 10f)] private LevelFloat spiderDamage = new LevelFloat(0.2f, 0.5f);
        [SerializeField, MinMaxRange(0, 1f)] private LevelFloat baseSpawnChance = new LevelFloat(0.35f, 0.75f);
        [SerializeField, MinMaxRange(1, 5)] private LevelInt baseSpiderAmount = new LevelInt(1, 3);
        
        private int currentEffectDuration;
        private float currentSpiderLifetime;
        private float currentSpiderSpeed;
        private float currentSpiderDamage;
        private float currentBaseSpawnChance;
        private float currentBaseSpiderAmount;
        
        private AttackEffect attackEffect;
        private BroodInfectionEffectData effectData;

        public static int MaxSpiders { get; private set; }


        protected override void Awake()
        {
            base.Awake();
            MaxSpiders = Mathf.RoundToInt(maxSpiders.Value);
        }

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
            currentEffectDuration = effectDuration.AtLvl(lvl);
            currentSpiderLifetime = spiderLifetime.AtLvl(lvl);
            currentSpiderSpeed = spiderSpeed.AtLvl(lvl);
            currentSpiderDamage = spiderDamage.AtLvl(lvl);
            currentBaseSpawnChance = baseSpawnChance.AtLvl(lvl);
            currentBaseSpiderAmount = baseSpiderAmount.AtLvl(lvl);

            effectData = new BroodInfectionEffectData(currentEffectDuration, currentBaseSpawnChance, currentBaseSpiderAmount,
                new BroodSpiderArguments(
                    currentSpiderLifetime,
                    currentSpiderSpeed,
                    currentSpiderDamage
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
                Random.value <= unitInfectionChance.Value)
                affectable.AddEffect<BroodInfectionEffect>(effectData);
        }
        
        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                maxSpiders.UseKey(LevelFieldKeys.MAX_SPIDERS),
                unitInfectionChance.UseKey(LevelFieldKeys.UNIT_INFECTION_CHANCE).UseFormatter(StatFormatter.PERCENT),
                effectDuration.UseKey(LevelFieldKeys.EFFECT_DURATION).UseFormatter(StatFormatter.SECONDS),
                spiderLifetime.UseKey(LevelFieldKeys.SPIDER_LIFETIME).UseFormatter(StatFormatter.SECONDS),
                spiderSpeed.UseKey(LevelFieldKeys.SPIDER_SPEED),
                spiderDamage.UseKey(LevelFieldKeys.SPIDER_DAMAGE),
                baseSpawnChance.UseKey(LevelFieldKeys.SPIDER_SPAWN_CHANCE).UseFormatter(StatFormatter.PERCENT),
                baseSpiderAmount.UseKey(LevelFieldKeys.SPIDER_SPAWN_AMOUNT)
            };
        }
    }
}