using System.Collections.Generic;
using System.Text;
using Gameplay.Abilities.EntityEffects;
using Gameplay.Enemies;
using Mutations.AttackEffects;
using Player;
using UnityEngine;
using Util;

namespace Gameplay.Abilities.Passive
{
    public class PoisoningBite : BasicAbility
    {
        [SerializeField] private Gradient effectGradient;
        [Header("Poison damage")]
        [SerializeField] private float totalDamageLvl1;
        [SerializeField] private float totalDamageLvl10;        
        [Header("Slow effect")]
        [SerializeField, Range(0, 1)] private float slowLvl1;
        [SerializeField, Range(0, 1)] private float slowLvl10;
        [Header("Duration")] 
        [SerializeField] private int durationLvl1;
        [SerializeField] private int durationLvl10;

        private AttackEffect attackEffect;
        private PoisonEffectData effectData;


        
        protected override void Start()
        {
            attackEffect = new AttackEffect(effectGradient, OnImpact);
            base.Start();
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            effectData = new PoisonEffectData
            ((int) LerpLevel(durationLvl1, durationLvl10, lvl),
                LerpLevel(slowLvl1, slowLvl10, lvl),
                LerpLevel(totalDamageLvl1, totalDamageLvl10, lvl));
        }

        private void OnImpact(Enemy enemy, float _) 
            => enemy.AddEffect<PoisonEntityEffect>(effectData);

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
            PlayerAttack.OnAttackEffectCollectionRequested -= OnAttackEffectCollectionRequested;
        }
        
        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            StringBuilder sb = new StringBuilder();
            
            float dmg = LerpLevel(totalDamageLvl1, totalDamageLvl10, lvl);
            float prevDmg = 0;
            float slw = LerpLevel(slowLvl1, slowLvl10, lvl);
            float prevSlw = 0;
            float dur = LerpLevel(durationLvl1, durationLvl10, lvl);
            float prevDur = 0;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevDmg = LerpLevel(totalDamageLvl1, totalDamageLvl10, prevLvl);
                prevSlw = LerpLevel(slowLvl1, slowLvl10, prevLvl);
                prevDur = LerpLevel(durationLvl1, durationLvl10, prevLvl);
            }

            sb.AddAbilityLine("Effect duration", dur, prevDur, suffix: "s");
            sb.AddAbilityLine("Total damage", dmg, prevDmg);
            sb.AddAbilityLine("Slow amount", slw, prevSlw, percent: true, prefix: "-");
            return sb.ToString();
        }
    }
}