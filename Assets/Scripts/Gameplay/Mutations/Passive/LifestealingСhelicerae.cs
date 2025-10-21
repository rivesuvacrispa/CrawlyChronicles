using System.Collections.Generic;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Player;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
{
    public class LifestealingСhelicerae : BasicAbility
    {
        [SerializeField] private Gradient effectGradient;
        [Header("Lifesteal")] 
        [SerializeField, Range(0f, 1f)] private float lifestealLvl1;
        [SerializeField, Range(0f, 1f)] private float lifestealLvl10;
        
        private AttackEffect attackEffect;
        private float lifesteal;
        
        
        protected override void Start()
        {
            attackEffect = new AttackEffect(effectGradient, OnImpact);
            base.Start();
        }
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            lifesteal = LerpLevel(lifestealLvl1, lifestealLvl10, lvl);
        }

        private void OnImpact(IImpactable enemy, float damage)
        {
            PlayerManager.Instance.AddHealth(damage * lifesteal);
        }


        
        private void OnAttackEffectCollectionRequested(List<AttackEffect> effects) 
            => effects.Add(attackEffect);
        
        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerAttack.OnAttackEffectCollectionRequested += OnAttackEffectCollectionRequested;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopAllCoroutines();
            PlayerAttack.OnAttackEffectCollectionRequested -= OnAttackEffectCollectionRequested;
        }
        
        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            float cd = 0;
            float prevCd = cd;
            int ls = (int) (LerpLevel(lifestealLvl1, lifestealLvl10, lvl) * 100);
            int prevLs = ls;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevCd = 0;
                prevLs = (int) (LerpLevel(lifestealLvl1, lifestealLvl10, prevLvl) * 100);
            }

            var args = new object[]
            {
                cd,          ls,
                cd - prevCd, ls - prevLs
            };
            
            return scriptable.GetStatDescription(args);
        }
    }
}