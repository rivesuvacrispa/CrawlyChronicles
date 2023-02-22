using System.Text;
using Gameplay.Enemies;
using UnityEngine;
using Util;

namespace Gameplay.Abilities.Active
{
    public class AcidSpray : ActiveAbility
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [Header("Particles amount")] 
        [SerializeField] private int amountLvl1;
        [SerializeField] private int amountLvl10;
        [Header("Spray angle")] 
        [SerializeField] private int angleLvl1;
        [SerializeField] private int angleLvl10;
        [Header("Stun and knockback")] 
        [SerializeField, Range(0, 1)] private float stunDuration = 0.5f;
        [SerializeField, Range(0, 10)]  private float knockbackPower = 0.5f;
        [Header("Damage")]
        [SerializeField] private float damageLvl1;
        [SerializeField] private float damageLvl10;

        
        private float damage;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(particleSystem.isPlaying) particleSystem.Stop();
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            var emission = particleSystem.emission;
            var shape = particleSystem.shape;
            shape.angle = LerpLevel(amountLvl1, amountLvl10, lvl);
            emission.SetBurst(0, new ParticleSystem.Burst(0, LerpLevel(angleLvl1, angleLvl10, lvl)));
        }

        public override void Activate() => particleSystem.Play();

        private void OnParticleCollision(GameObject other)
        {
            if (other.TryGetComponent(out Enemy enemy)) 
                enemy.Damage(damage, knockbackPower, stunDuration);
        }

        public override string GetLevelDescription(int lvl)
        {
            StringBuilder sb = new StringBuilder();

            float prevCd = 0;
            float width = LerpLevel(angleLvl1, angleLvl10, lvl) * 2;
            float prevWidth = 0;
            float amount = LerpLevel(amountLvl1, amountLvl10, lvl);
            float prevAmount = 0;
            float dmg = LerpLevel(damageLvl1, damageLvl10, lvl);
            float prevDmg = 0;

            if (lvl > 0)
            {
                var prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevWidth = LerpLevel(angleLvl1, angleLvl10, prevLvl) * 2;
                prevAmount = LerpLevel(amountLvl1, amountLvl10, prevLvl);
                prevDmg = LerpLevel(damageLvl1, damageLvl10, prevLvl);
            }
            
            sb.AddAbilityLine("Cooldown", Scriptable.GetCooldown(lvl), prevCd, false);
            sb.AddAbilityLine("Spray width", width, prevWidth);
            sb.AddAbilityLine("Spray amount", amount, prevAmount);
            sb.AddAbilityLine("Spray damage", dmg, prevDmg);
            sb.AddAbilityLine("Stun duration", stunDuration, 0);
            sb.AddAbilityLine("Knockback", knockbackPower, 0);
            
            return sb.ToString();
        }
    }
}