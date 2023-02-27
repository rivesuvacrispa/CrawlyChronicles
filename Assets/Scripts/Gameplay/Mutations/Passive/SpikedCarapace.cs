using System.Text;
using Gameplay.Enemies;
using Player;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Abilities.Passive
{
    public class SpikedCarapace : BasicAbility
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField] private int damageCap;
        [Header("Trigger chance")]
        [SerializeField, Range(0, 1)] private float probabilityLvl1;
        [SerializeField, Range(0, 1)] private float probabilityLvl10;
        [Header("Particles amount")] 
        [SerializeField] private int amountLvl1;
        [SerializeField] private int amountLvl10;
        [Header("Stun and knockback")] 
        [SerializeField, Range(0, 1)] private float stunDuration = 0.5f;
        [SerializeField, Range(0, 10)] private float knockbackPower = 0.5f;
        [Header("Damage")]
        [SerializeField] private float damageLvl1;
        [SerializeField] private float damageLvl10;

        private float procRate;
        private float damage;
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(particleSystem.isPlaying) particleSystem.Stop();
            procRate = LerpLevel(probabilityLvl1, probabilityLvl10, lvl);
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            var emission = particleSystem.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, 
                LerpLevel(amountLvl1, amountLvl10, lvl)));
        }

        private void Activate()
        {
            particleSystem.Play();
        }
        
        private void OnDamageTaken(float dmg)
        {
            if(dmg >= damageCap || Random.value <= GetPassiveProcRate(procRate)) 
                Activate();
        }
        
        private void OnParticleCollision(GameObject other)
        {
            if (other.TryGetComponent(out Enemy enemy)) 
                enemy.Damage(GetAbilityDamage(damage), knockbackPower, stunDuration, Color.white);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerHitbox.OnDamageTaken -= OnDamageTaken;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerHitbox.OnDamageTaken += OnDamageTaken;
        }
        
        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            StringBuilder sb = new StringBuilder();
            
            float prob = LerpLevel(probabilityLvl1, probabilityLvl10, lvl);
            float prevProb = 0;
            float amount = LerpLevel(amountLvl1, amountLvl10, lvl);
            float prevAmount = 0;
            float dmg = LerpLevel(damageLvl1, damageLvl10, lvl);
            float prevDmg = 0;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevProb = LerpLevel(probabilityLvl1, probabilityLvl10, prevLvl);
                prevAmount = LerpLevel(amountLvl1, amountLvl10, prevLvl);
                prevDmg = LerpLevel(damageLvl1, damageLvl10, prevLvl);
            }

            sb.AddAbilityLine("Trigger chance", prob, prevProb, percent: true);
            sb.AddAbilityLine("Spikes amount", amount, prevAmount);
            sb.AddAbilityLine("Spikes damage", dmg, prevDmg);
            sb.AddAbilityLine("Damage cap", damageCap, 0);
            sb.AddAbilityLine("Stun duration", stunDuration, 0, suffix: "s");
            sb.AddAbilityLine("Knockback", knockbackPower, 0);
            return sb.ToString();
        }
    }
}