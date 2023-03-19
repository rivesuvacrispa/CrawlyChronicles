using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mutations.AttackEffects;
using Player;
using Scripts.Util.Interfaces;
using UnityEngine;
using Util;

namespace Gameplay.Abilities.Passive
{
    public class LifestealingСhelicerae : BasicAbility
    {
        [SerializeField] private Gradient effectGradient;
        [Header("Cooldown")]
        [SerializeField] private float cooldownLvl1;
        [SerializeField] private float cooldownLvl10;
        [Header("Lifesteal")] 
        [SerializeField] private float lifestealLvl1;
        [SerializeField] private float lifestealLvl10;
        
        private AttackEffect attackEffect;
        private float cooldown;
        private float lifesteal;
        
        
        protected override void Start()
        {
            attackEffect = new AttackEffect(effectGradient, OnImpact);
            base.Start();
        }
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            cooldown = LerpLevel(cooldownLvl1, cooldownLvl10, lvl);
            lifesteal = LerpLevel(lifestealLvl1, lifestealLvl10, lvl);
        }

        private void OnImpact(IImpactable enemy, float damage)
        {
            PlayerManager.Instance.AddHealth(damage * lifesteal);
            StartCoroutine(CooldownRoutine());
        }

        private IEnumerator CooldownRoutine()
        {
            Debug.Log("Lifesteal cooldown ON");
            PlayerAttack.OnAttackEffectCollectionRequested -= OnAttackEffectCollectionRequested;
            yield return new WaitForSeconds(cooldown);
            PlayerAttack.OnAttackEffectCollectionRequested += OnAttackEffectCollectionRequested;
            Debug.Log("Lifesteal cooldown OFF");
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
            StringBuilder sb = new StringBuilder();
            
            float cd = LerpLevel(cooldownLvl1, cooldownLvl10, lvl);
            float prevCd = 0;
            float ls = LerpLevel(lifestealLvl1, lifestealLvl10, lvl);
            float prevLs = 0;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevCd = LerpLevel(cooldownLvl1, cooldownLvl10, prevLvl);
                prevLs = LerpLevel(lifestealLvl1, lifestealLvl10, prevLvl);
            }

            sb.AddAbilityLine("Cooldown", cd, prevCd, withUpgradePlus: false, suffix: "s");
            sb.AddAbilityLine("Lifesteal amount", ls, prevLs, percent: true);
            return sb.ToString();
        }
    }
}