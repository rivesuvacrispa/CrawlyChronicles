using System.Collections.Generic;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Mutations.EntityEffects.Poison;
using Gameplay.Player;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
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
            int duration = (int) LerpLevel(durationLvl1, durationLvl10, lvl);
            effectData = new PoisonEffectData
                (duration,
                LerpLevel(slowLvl1, slowLvl10, lvl),
                LerpLevel(totalDamageLvl1, totalDamageLvl10, lvl) / duration);
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
            PlayerAttack.OnAttackEffectCollectionRequested += OnAttackEffectCollectionRequested;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerAttack.OnAttackEffectCollectionRequested -= OnAttackEffectCollectionRequested;
        }
        
        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            float dmg = LerpLevel(totalDamageLvl1, totalDamageLvl10, lvl);
            float prevDmg = dmg;
            int slw = (int) (LerpLevel(slowLvl1, slowLvl10, lvl) * 100);
            int prevSlw = slw;
            float dur = LerpLevel(durationLvl1, durationLvl10, lvl);
            float prevDur = dur;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevDmg = LerpLevel(totalDamageLvl1, totalDamageLvl10, prevLvl);
                prevSlw = (int) (LerpLevel(slowLvl1, slowLvl10, prevLvl) * 100);
                prevDur = LerpLevel(durationLvl1, durationLvl10, prevLvl);
            }
            
            var args = new object[]
            {
                dur,           dmg,           slw,         
                dur - prevDur, dmg - prevDmg, slw - prevSlw
            };
            
            return scriptable.GetStatDescription(args);
        }
    }
}