using System.Collections.Generic;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Mutations.EntityEffects.Poison;
using Gameplay.Player;
using UnityEngine;
using Util.Abilities;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
{
    public class FireBite : BasicAbility
    {
        [SerializeField] private Gradient effectGradient;
        [SerializeField] private LevelConst damagePortion;
        [SerializeField] private LevelConst effectDuration;

        private AttackEffect attackEffect;

        

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                damagePortion.UseKey(LevelFieldKeys.DAMAGE).UseFormatter(StatFormatter.PERCENT),
                effectDuration.UseKey(LevelFieldKeys.EFFECT_DURATION).UseFormatter(StatFormatter.SECONDS)
            };
        }

        protected override void Start()
        {
            attackEffect = new AttackEffect(effectGradient, OnImpact);
            base.Start();
        }

        private void OnImpact(IImpactable impactable, float damage)
        {
            if (impactable is IEffectAffectable affectable)
                affectable.AddEffect<FireBiteEntityEffect>(
                    new FireBiteEffectData(
                        Mathf.RoundToInt(effectDuration.Value),
                        damage * damagePortion.Value
                    )
                );
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