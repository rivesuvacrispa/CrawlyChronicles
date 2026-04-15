using System.Collections.Generic;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Player;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
{
    public class LifestealingСhelicerae : BasicAbility
    {
        [SerializeField] private Gradient effectGradient;
        [SerializeField, MinMaxRange(0, 1f)] private LevelFloat lifesteal = new LevelFloat(0.5f, 1f);
        
        private AttackEffect attackEffect;
        private float currentLifesteal;

        

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                lifesteal.UseKey(LevelFieldKeys.LIFESTEAL).UseFormatter(StatFormatter.PERCENT)
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
            currentLifesteal = lifesteal.AtLvl(lvl);
        }

        private void OnImpact(IImpactable enemy, float damage)
        {
            PlayerManager.Instance.AddHealth(damage * currentLifesteal);
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
            StopAllCoroutines();
            BasePlayerAttack.OnAttackEffectCollectionRequested -= OnAttackEffectCollectionRequested;
        }
    }
}