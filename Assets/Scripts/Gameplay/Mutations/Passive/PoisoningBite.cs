using System.Collections.Generic;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Mutations.EntityEffects.Poison;
using Gameplay.Player;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
{
    public class PoisoningBite : BasicAbility
    {
        [SerializeField] private Gradient effectGradient;
        [SerializeField, MinMaxRange(0, 10)] private LevelFloat totalDamage = new LevelFloat(1.5f, 10f);
        [SerializeField, MinMaxRange(0, 1)] private LevelFloat slow = new LevelFloat(0.25f, 0.85f);
        [SerializeField, MinMaxRange(0, 10)] private LevelInt duration = new LevelInt(4, 8);

        private AttackEffect attackEffect;
        private PoisonEffectData effectData;

        

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                totalDamage.UseKey(LevelFieldKeys.DAMAGE),
                slow.UseKey(LevelFieldKeys.MOVEMENT_SLOW).UseFormatter(StatFormatter.PERCENT),
                duration.UseKey(LevelFieldKeys.EFFECT_DURATION).UseFormatter(StatFormatter.SECONDS)
            };
        }

        protected override void Start()
        {
            attackEffect = new AttackEffect(effectGradient, OnImpact);
            base.Start();
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            int currentDuration = duration.AtLvl(lvl);
            effectData = new PoisonEffectData
                (currentDuration, slow.AtLvl(lvl), totalDamage.AtLvl(lvl) / currentDuration);
        }

        private void OnImpact(IImpactable impactable, float _)
        {
            if(impactable is IEffectAffectable affectable)
                affectable.AddEffect<PoisonEntityEffect>(effectData);
        }

        private void OnAttackEffectCollectionRequested(List<AttackEffect> effects) 
            => effects.Add(attackEffect);
        
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